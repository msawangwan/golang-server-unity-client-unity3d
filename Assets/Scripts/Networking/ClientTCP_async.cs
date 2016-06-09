using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

public enum ClientConnState {
    None = 0,
    Connected = 1,
    Disconnected = 2,
    Idle = 3,
    Timeout = 4,
}

public class ClientTCP_async {
    sealed class ClientState {
        public Socket WorkSocket = null;
        public StringBuilder SB;
        public byte[] Buffer;
        public int NumReads = 0;
        public int MsgSize = 0; // TODO: Replace with a Packet Objct
        private const int bufferSize = 256;

        public string dataString = null;

        public ClientState() {
            SB = new StringBuilder ( );
            Buffer = new byte[bufferSize];
        }
    }

    sealed class Packet {
        public readonly DateTime TimeStamp;
        public byte[] PacketBuffer;

        private const int packetSize = 4096;

        public Packet(DateTime timestamp, byte[] buffer) {
            TimeStamp = timestamp;
            PacketBuffer = new byte[packetSize];
            Buffer.BlockCopy ( buffer , 0 , PacketBuffer , 0 , packetSize );
        }

        public void AddToBuffer(byte[] buffer) {
            Buffer.BlockCopy ( buffer , 0 , PacketBuffer , PacketBuffer.Length , packetSize );
        }
    }

    private const string serverAddr = "127.0.0.1";
    private const int serverPort = 9080;

    private static Socket serverSocket;
    private static IPAddress serverIP;
    private static IPEndPoint serverEndpoint;

    private NetworkStream socketStream;

    private int connRetryAttempts = 0;
    private bool isConnected = false;
    private bool hasFailedToConnect = false;

    private String recvdData = string.Empty;

    /* Default constructor */
    public ClientTCP_async ( ) {
        serverIP = IPAddress.Parse ( serverAddr );
        serverEndpoint = new IPEndPoint ( serverIP, serverPort );
    }

    public void Connect ( ) {
        serverSocket = OpenTCPSocket ( );
        serverSocket.BeginConnect ( serverEndpoint, new AsyncCallback ( OnConnect ), null );
    }

    public void Disconnect() {
        OnDisconnect ( );
    }

    public void Send ( string data ) {
        try {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            serverSocket.BeginSend ( buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback ( OnSend ), null );
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    public void Listen ( ) {
        try {
            ClientState clientState = new ClientState();
            clientState.WorkSocket = serverSocket;

            recvdData = string.Empty;

            serverSocket.BeginReceive ( clientState.Buffer, 0, clientState.Buffer.Length, SocketFlags.None, new AsyncCallback ( OnReceive ), clientState );
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnConnect ( IAsyncResult ar ) {
        try {
            serverSocket.EndConnect ( ar );
            isConnected = true;
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnDisconnect ( ) {
        try {
            serverSocket.Close ( );
            isConnected = false;
        } catch ( Exception e ) {
            Debug.Log ( "[Client:OnDisconnect] Error on disconnect: " + e );
        }
    }

    private void OnSend ( IAsyncResult ar ) {
        try {
            serverSocket.EndSend ( ar );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnReceive ( IAsyncResult ar ) {
       // recvdData = String.Empty;
        try {
            ClientState clientState = (ClientState) ar.AsyncState;
            int bytesRead = clientState.WorkSocket.EndReceive(ar);
            clientState.MsgSize += bytesRead;
            Debug.Log ( "[Client:OnReceive] bytesRead = " + bytesRead );
            Debug.Log ( "[Client:OnReceive] numReads = " + clientState.NumReads );
            Debug.Log ( "[Client:OnReceive] size of clientState stringbuilder: " + clientState.SB.Length );



            if ( clientState.MsgSize > 216) {
                Debug.Log ( "[Client:OnReceive] From Server: " + clientState.SB );
                Debug.Log ( "[Client:OnReceive] From Server: <EOM>" );
                //clientState.WorkSocket.EndReceive ( ar );

                clientState = new ClientState ( );
                clientState.WorkSocket = serverSocket;
                clientState.WorkSocket.BeginReceive ( clientState.Buffer, 0, clientState.Buffer.Length, 0, new AsyncCallback ( OnReceive ), clientState );
            }

            else if ( bytesRead > 0 ) {
                clientState.NumReads++;               
                clientState.SB.Append ( Encoding.ASCII.GetString ( clientState.Buffer , 0 , bytesRead ) );
                //Debug.Log ( "[Client:OnReceive] data in buffer: " + Encoding.ASCII.GetString ( clientState.Buffer, 0, bytesRead ) );              
                clientState.WorkSocket.BeginReceive ( clientState.Buffer , 0 , clientState.Buffer.Length , 0 , new AsyncCallback ( OnReceive ) , clientState );
            } else if (bytesRead == 0) {
                Debug.Log ( "[] Recieved Zero Bytes" );
            }

           // if ( clientState.SB.Length > 1 ) {
          //      Debug.Log ( "[Client:OnReceive] adding to string builder ..." );
          //      recvdData += clientState.SB.ToString ( );
           // }

        } catch ( ObjectDisposedException ode ) {
            Debug.Log ( ode );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnReceive_Naive ( IAsyncResult ar ) {
        try {
            byte[] readBuffer = new byte[1024];
            Socket remoteConn = (Socket) ar.AsyncState;
            int recv = serverSocket.EndReceive ( ar );
            string receivedData = Encoding.ASCII.GetString(readBuffer, 0, recv);
            Debug.Log ( "ON RECIEVE " + receivedData );
            recvdData = receivedData;
        } catch ( Exception e ) {
            recvdData = null;
            Debug.Log ( e );
        }
    }

    private static byte[] ReadStream ( NetworkStream stream ) {
        if ( stream.CanRead ) {
            int readCount = 0;
            int startIndex = 0;
            int totalBytesRead = 0;

            byte[] buffer = new byte[4096];
            byte[] tmpBuffer = new byte[32];

            using ( MemoryStream writer = new MemoryStream ( ) ) {
                do {
                    readCount++;

                    int numBytesRead = stream.Read(tmpBuffer, 0, tmpBuffer.Length);
                    totalBytesRead += numBytesRead;

                    Debug.Log ( numBytesRead + " " + readCount + " " + totalBytesRead + " " + startIndex );
                    if ( numBytesRead <= 0 ) {
                        if ( stream.ReadByte ( ) == -1 ) {
                            break;
                        }
                    }

                    writer.Write ( tmpBuffer , 0 , numBytesRead ); // write to the tmpBuffer
                    Buffer.BlockCopy ( tmpBuffer , 0 , buffer , startIndex , numBytesRead ); // copy tmpBuffer to buffer

                    startIndex = totalBytesRead;
                } while ( stream.DataAvailable );

                writer.Close ( );
                return buffer;
            }
        }
        return new byte[0];
    }

    private static Socket OpenTCPSocket ( ) {
        return new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
    }
}
