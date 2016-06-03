using UnityEngine;
using System.Threading;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;  

public class ClientTCP {
    private const string serverAddr = "127.0.0.1";
    private const int serverPort = 9080;

    private static Socket serverConn;
    private static IPAddress serverIP;
    private static IPEndPoint serverEndpoint;

    private NetworkStream socketStream;

    private int connRetryAttempts = 0;
    private bool isConnected = false;
    private bool hasFailedToConnect = false;

    /* Default constructor */
    public ClientTCP () {
        serverIP = IPAddress.Parse ( serverAddr );
        serverEndpoint = new IPEndPoint ( serverIP , serverPort );
    }

    public bool EstablishConnection ( ) {
        serverConn = OpenTCPSocket ( );

        try {
            Debug.Log ( "Establishing connection to remote server ... " );
            serverConn.Connect ( serverEndpoint );
        } catch {
            Debug.Log ( "Error, unable to establish a connection with the server, retrying ... attempt # " + connRetryAttempts );

            if ( connRetryAttempts > 3 ) {
                Debug.Log ( "Failed to connect after " + connRetryAttempts + " retry attempts, terminating." );
                connRetryAttempts = 0;
                return false;
            }

            ++connRetryAttempts;
            EstablishConnection ( );
        }

        socketStream = new NetworkStream ( serverConn );

        return true;
    }

    public void CloseConnection( ) {
        socketStream.Close ( );
        serverConn.Shutdown ( SocketShutdown.Both ); // try with this commented out to see what effects it has
        serverConn.Close ( );

        Debug.Log ( "Disconnected from server." );
    }

    public string SendAndReceiveData ( string data ) {
        byte[] bufferedData = Encoding.ASCII.GetBytes ( data );

        WriteStream ( socketStream, bufferedData );
        byte[] serverReply = ReadStream(socketStream);

        return Encoding.ASCII.GetString ( serverReply );
    }

    private Socket OpenTCPSocket() {
        return new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
    }

    private static void WriteStream(NetworkStream stream, byte[] bufferedData) {
        if ( stream.CanWrite ) {
            stream.Write ( bufferedData, 0, bufferedData.Length );
            stream.Flush ( );
        }
    }

    private static byte[] ReadStream(NetworkStream stream) {
        if (stream.CanRead) {
            int readCount = 0;
            int startIndex = 0;
            int totalBytesRead = 0;

            byte[] buffer = new byte[4096];
            byte[] tmpBuffer = new byte[32];

            using (MemoryStream writer = new MemoryStream()) {
                do {
                    readCount++;
                    
                    int numBytesRead = stream.Read(tmpBuffer, 0, tmpBuffer.Length);
                    totalBytesRead += numBytesRead;

                    Debug.Log ( numBytesRead + " " + readCount + " " + totalBytesRead + " " + startIndex);
                    if ( numBytesRead <= 0 ) {
                        if ( stream.ReadByte() == -1 ) {
                            break;
                        }
                    }

                    writer.Write ( tmpBuffer, 0, numBytesRead ); // write to the tmpBuffer
                    System.Buffer.BlockCopy ( tmpBuffer, 0, buffer, startIndex, numBytesRead ); // copy tmpBuffer to buffer

                    startIndex = totalBytesRead;
                } while ( stream.DataAvailable );

                writer.Close ( );
                return buffer;
            }
        }
        return new byte[0];
    }
}