using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class ClientTCP_async {
    sealed class ClientConnection {
        public Socket ClientSocket = null;
        public StringBuilder SB;

        public int BufferSize;
        public byte[] Buffer;

        public string dataString = null;

        public ClientConnection() {
            BufferSize = 1024;

            SB = new StringBuilder ( );
            Buffer = new byte[BufferSize];
        }
    }

    private const string serverAddr = "10.0.0.76";
    private const int serverPort = 9080;

    private static Socket serverSocket;
    private static IPAddress serverIP;
    private static IPEndPoint serverEndpoint;

    private NetworkStream socketStream;

    private int connRetryAttempts = 0;
    private bool isConnected = false;
    private bool hasFailedToConnect = false;

    private string recvdData = null;

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

    public void SendData ( string data ) {
        try {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            serverSocket.BeginSend ( buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback ( OnSend ), null );
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    public string SendAndRecvData ( string data ) {
        try {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            serverSocket.BeginSend ( buffer , 0 , buffer.Length , SocketFlags.None , new AsyncCallback ( OnSend ) , null );
            try {
                ClientConnection client = new ClientConnection();
                client.ClientSocket = serverSocket;

                serverSocket.BeginReceive ( client.Buffer , 0 , client.BufferSize , SocketFlags.None , new AsyncCallback ( OnReceive ) , client );
                return recvdData;
            } catch ( SocketException se ) {
                Debug.Log ( se );
            } catch ( Exception e ) {
                Debug.Log ( e );
            }
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( System.Exception e ) {
            Debug.Log ( e );
        }
        return null;
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
            Debug.Log ( e );
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
        try {
            ClientConnection client = (ClientConnection) ar.AsyncState;
            Socket clientSocket = client.ClientSocket;

            int bytesRead = clientSocket.EndReceive(ar);

            if ( bytesRead > 0 ) {
                Debug.Log ( "BYTES READ GREATER THAN 0" );
                Debug.Log ( Encoding.ASCII.GetString ( client.Buffer , 0 , bytesRead ) );
                client.SB.Append ( Encoding.ASCII.GetString ( client.Buffer , 0 , bytesRead ) );
                clientSocket.BeginReceive ( client.Buffer , 0 , client.BufferSize , 0 , new AsyncCallback ( OnReceive ) , client );
            } else {
                if ( client.SB.Length > 1 ) {
                    Debug.Log ( "TO STRING" );
                    recvdData = client.SB.ToString ( );
                }
            }
        } catch ( Exception e ) {
            recvdData = null;
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
