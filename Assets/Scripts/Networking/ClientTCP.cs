using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Asynchronous TCP Client.
/// </summary>

public enum ClientConnState {
    None = 0,
    Connected = 1,
    Disconnected = 2,
    Idle = 3,
    Timeout = 4,
}

public class ClientTCP {
    /* Maintains socket state between socket reads. */
    sealed class SocketState {
        public Socket WorkSocket { get; set; }
        public Packet DataFrame { get; set; }

        public int TotalBytesRead { get; set; }
        public byte[] Buffer { get; private set; }
        public byte[] FrameSize { get; set; }

        private const int bufferSize = 16;

        public SocketState() {
            Buffer = new byte[bufferSize];
            FrameSize = new byte[4];

            DataFrame = new Packet ( );

            TotalBytesRead = 0;
        }
    }

    /* Represents a data frame. */
    sealed class Packet {
        public byte[] Buffer { get; private set; }
        public int FrameSize { get; set; }

        private const int packetSize = 4096;
        private int bufferPosition = 0;

        public Packet() {
            Buffer = new byte[packetSize];
            FrameSize = 0;
        }

        public void AddData(byte[] buffer, int bufferOffset) {
            System.Buffer.BlockCopy ( buffer, 0, Buffer, bufferPosition, buffer.Length );
            bufferPosition += bufferOffset;
        }
    }

    private ClientConnState connectionState = ClientConnState.None;

    //private const string serverAddr = "10.0.0.76";
    private const string serverAddr = "127.0.0.1";
    private const int serverPort = 9080;

    private static Socket serverSocket;
    private static IPAddress serverIP;
    private static IPEndPoint serverEndpoint;

    /* Default constructor */
    public ClientTCP ( ) {
        serverIP = IPAddress.Parse ( serverAddr );
        serverEndpoint = new IPEndPoint ( serverIP, serverPort );
    }

    public void Connect ( ) {
        serverSocket = OpenTCPSocket ( );
        serverSocket.BeginConnect ( serverEndpoint, new AsyncCallback ( ConnectCallback ), null );
    }

    public void Disconnect() {
        DisconnectCallback ( );
    }

    public void Send ( byte[] buffer ) {
        try {
            serverSocket.BeginSend ( buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback ( SendCallback ), null );
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    public void Listen ( ) {
        try {
            SocketState clientState = new SocketState();
            clientState.WorkSocket = serverSocket;

            serverSocket.BeginReceive ( clientState.FrameSize, 0, clientState.FrameSize.Length, SocketFlags.None, new AsyncCallback ( RecvCallback ), clientState ); // wait for incoming data
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void ConnectCallback ( IAsyncResult ar ) {
        try {
            serverSocket.EndConnect ( ar );
            connectionState = ClientConnState.Connected;
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void DisconnectCallback ( ) {
        try {
            serverSocket.Close ( );
            connectionState = ClientConnState.Disconnected;
        } catch ( Exception e ) {
            Debug.Log ( "[Client:DisconnectCallback] Error on disconnect: " + e );
        }
    }

    private void SendCallback ( IAsyncResult ar ) {
        try {
            serverSocket.EndSend ( ar );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void RecvCallback ( IAsyncResult ar ) {
        SocketState socketState = (SocketState) ar.AsyncState;

        try {
            int bytesRead = socketState.WorkSocket.EndReceive(ar); // read bytes from stream

            if ( bytesRead <= 0 ) {
                Debug.Log ( "Zero bytes recvd" );
                // handle ...
            }

            if ( bytesRead <= socketState.FrameSize.Length && socketState.DataFrame.FrameSize == 0 ) { // check for message size first
                Debug.Log ( "first read ... " + bytesRead );
                int encodedSize = BitConverter.ToInt32(socketState.FrameSize, 0);
                socketState.DataFrame.FrameSize = IPAddress.NetworkToHostOrder(encodedSize);
                serverSocket.BeginReceive ( socketState.Buffer, 0, socketState.Buffer.Length, 0, new AsyncCallback ( RecvCallback ), socketState );
            }

            socketState.TotalBytesRead += bytesRead;

           // Debug.Log ( "total read " + socketState.TotalBytesRead );
           // Debug.Log ( "current bytes read: " + bytesRead );
            if ( socketState.TotalBytesRead >= socketState.DataFrame.FrameSize ) {
                Debug.Log ( "RECVD: " + BitConverter.ToString ( socketState.DataFrame.Buffer ) );
                Debug.Log ( "[Client:RecvCallback] <EOM> New DataFrame <EOM>" );
                SocketState newSocket = new SocketState ( );
                newSocket.WorkSocket = serverSocket;
                serverSocket.BeginReceive ( newSocket.FrameSize, 0, newSocket.FrameSize.Length, SocketFlags.None, new AsyncCallback ( RecvCallback ), newSocket );
            } else {
                //serverSocket.BeginReceive ( socketState.FrameSize, 0, socketState.FrameSize.Length, SocketFlags.None, new AsyncCallback ( RecvCallback ), socketState );
                //  socketState.DataFrame.AddData ( socketState.Buffer, bytesRead );
                // socketState.TotalBytesRead += bytesRead;
                socketState.DataFrame.AddData ( socketState.Buffer, bytesRead );
                serverSocket.BeginReceive ( socketState.Buffer, 0, socketState.Buffer.Length, 0, new AsyncCallback ( RecvCallback ), socketState );
                //socketState.TotalBytesRead += bytesRead;
            }
            
            //serverSocket.BeginReceive ( socketState.Buffer, 0, socketState.Buffer.Length, 0, new AsyncCallback ( RecvCallback ), socketState );
        } catch ( ObjectDisposedException ode ) {
            Debug.Log ( ode );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void RecvCallback_delimted ( IAsyncResult ar ) {
        SocketState socketState = (SocketState) ar.AsyncState;

        try {
            int bytesRead = socketState.WorkSocket.EndReceive(ar); // read bytes from strem
            bool isDataFrame = false;

            socketState.TotalBytesRead += bytesRead;

            if ( bytesRead > 0 ) {
                int bufferSize = bytesRead;
                string currentBuffer = Encoding.ASCII.GetString ( socketState.Buffer );

                for ( int i = 0; i < currentBuffer.Length; i++ ) { // read through the buffer and look for delimiting char, '\n'
                    if ( currentBuffer[i] == '\n' ) {
                        isDataFrame = true;
                        bufferSize = i + 1;
                        break;
                    }
                }

                if ( isDataFrame ) { // if we reached a delimiting char, copy any remaining bytes before into the current DataFrame
                    byte[] temp = new byte[bufferSize];

                    for ( int i = 0; i < bufferSize; i++ ) {
                        temp[i] = socketState.Buffer[i];
                    }

                    socketState.DataFrame.AddData ( temp, bufferSize );
                } else { // else just copy the entire buffer
                    socketState.DataFrame.AddData ( socketState.Buffer, bufferSize );
                }

                Debug.Log ( Encoding.ASCII.GetString ( socketState.DataFrame.Buffer ) ); // print for debug

                if ( isDataFrame ) { // if we reached a delimiting char, reset client state and copy any remaining bytes from the previous buffer state into new buffer state
                    int bufferRemainder = socketState.Buffer.Length - bufferSize;
                    byte[] temp = new byte[bufferRemainder];

                    for ( int i = 0; i < bufferRemainder; i++ ) {
                        temp[i] = socketState.Buffer[bufferSize + i];
                    }

                    // SEND DATAFRAME OFF ...

                    Debug.Log ( "[Client:RecvCallback] <EOM> New DataFrame <EOM>" );
                    socketState = new SocketState ( );
                    socketState.WorkSocket = serverSocket;

                    socketState.DataFrame.AddData ( temp, bufferRemainder );
                }
            }

            if ( bytesRead == 0 ) { // if no bytes read, the connection is over, todo: handle
                Debug.Log ( "[Client:RecvCallback] No Bytes Read" );
            } else { // else keep listening for more
                socketState.WorkSocket.BeginReceive ( socketState.Buffer, 0, socketState.Buffer.Length, 0, new AsyncCallback ( RecvCallback ), socketState ); // THIS SHOULD BE SERVER SOCKET INSTEAD OF THE STATE SOCKET
            }
        } catch ( ObjectDisposedException ode ) {
            Debug.Log ( ode );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private static Socket OpenTCPSocket ( ) {
        return new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
    }
}
