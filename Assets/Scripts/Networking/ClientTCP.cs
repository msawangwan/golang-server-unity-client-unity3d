using UnityEngine;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;  

public class ClientTCP {
    private const string serverAddr = "10.0.0.76";

    private static Socket serverConn;
    private static IPAddress serverIP;
    private static IPEndPoint serverEndpoint;

    private NetworkStream stream;

    private int connRetryAttempts = 0;
    private bool isConnected = false;
    private bool hasFailedToConnect = false;

    /* Default constructor */
    public ClientTCP () {
        serverIP = IPAddress.Parse ( serverAddr );
        serverEndpoint = new IPEndPoint ( serverIP , 9080 );
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

        stream = new NetworkStream ( serverConn );
        return true;
    }

    public void CloseConnection( ) {
        stream.Close ( );
        serverConn.Shutdown ( SocketShutdown.Both ); // try with this commented out to see what effects it has
        serverConn.Close ( );
    }

    public void SendData ( ) {
        string str = "tessssssticles spectacles wallets and watch";

        if ( stream.CanWrite ) {
            stream.Write ( Encoding.ASCII.GetBytes ( str ) , 0 , str.Length );
            stream.Flush ( );
        }
    }

    private Socket OpenTCPSocket() {
        return new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
    }

    private int SendDataVar ( Socket conn , byte[] data ) {
        int total = 0;
        int sent = 0;

        int bufferSize = data.Length;
        int dataLeft = bufferSize;

        byte[] datasize = new byte[4];
        datasize = System.BitConverter.GetBytes ( bufferSize );
        sent = conn.Send ( datasize );

        while ( total < bufferSize ) {
            sent = conn.Send ( data , total , dataLeft , 0 );
            total += sent;
            dataLeft -= sent;
        }

        return total;
    }
}
