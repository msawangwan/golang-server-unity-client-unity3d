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

        private const int bufferSize = 16;

        public SocketState() {
            Buffer = new byte[bufferSize];
            DataFrame = new Packet ( );

            TotalBytesRead = 0;
        }
    }

    /* Represents a data frame. */
    sealed class Packet {
        public byte[] Buffer { get; private set; }

        private const int packetSize = 4096;
        private int bufferPosition = 0;

        public Packet() {
            Buffer = new byte[packetSize];
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
            Debug.Log ( buffer.Length );
            Debug.Log ( BitConverter.ToInt32 ( buffer , 0 ) );
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

            serverSocket.BeginReceive ( clientState.Buffer, 0, clientState.Buffer.Length, SocketFlags.None, new AsyncCallback ( RecvCallback ), clientState );
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
                socketState.WorkSocket.BeginReceive ( socketState.Buffer, 0, socketState.Buffer.Length, 0, new AsyncCallback ( RecvCallback ), socketState );
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
