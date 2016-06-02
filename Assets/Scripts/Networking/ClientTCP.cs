using UnityEngine;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;  

public class ClientTCP {
    private static Socket serverConn;
    private static IPAddress serverAddr;
    private static IPEndPoint serverEndpoint;

    private int connRetryAttempts = 0;
    private bool isConnected = false;
    private bool hasFailedToConnect = false;

    /* Default constructor */
    public ClientTCP () {
        Config ( );
    }

    public void EstablishConnection ( ) {
        serverConn = OpenTCPSocket ( );

        try {
            Debug.Log ( "Establishing connection to remote server ... " );
            serverConn.Connect ( serverEndpoint );
        } catch {
            Debug.Log ( "Error, unable to establish a connection with the server, retrying ... attempt # " + connRetryAttempts );

            if ( connRetryAttempts > 3 ) {
                Debug.Log ( "Failed to connect after " + connRetryAttempts + " retry attempts, terminating." );
                connRetryAttempts = 0;
                return;
            }

            ++connRetryAttempts;
            EstablishConnection ( );
        }

        bool isReadComplete = ReadWriteStream ( serverConn );

        if ( isReadComplete ) {
            return;
        }
        //byte[] outputBuffer1 = Encoding.ASCII.GetBytes("testicles specticles wallets and watch");
        //byte[] outputBuffer2 = Encoding.ASCII.GetBytes("WTF FUCK ARE YOU THINKING");
        //byte[] outputBuffer3 = Encoding.ASCII.GetBytes("HAHAHAHAHATESTER");

        //byte[] inputBuffer = new byte[1024];

        //SendData ( serverConn, outputBuffer1 );
        //SendData ( serverConn, outputBuffer2 );
        //SendData ( serverConn, outputBuffer3 );

        //int recv = serverConn.Receive(inputBuffer);
        //string input = Encoding.ASCII.GetString(inputBuffer,0, recv);

        //Debug.Log ( input );

        //serverConn.Close ( );
    }

    private void Config() {
        serverAddr = IPAddress.Parse ( "127.0.0.1" );
        serverEndpoint = new IPEndPoint ( serverAddr, 8080 );
    }

    private Socket OpenTCPSocket() {
        return new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
    }

    private int SendData(Socket conn, byte[] data) {
        int total = 0;
        int sent = 0;

        int bufferSize = data.Length;
        int dataLeft = bufferSize;

        byte[] datasize = new byte[4];
        datasize = System.BitConverter.GetBytes ( bufferSize );
        sent = conn.Send ( datasize );

        while ( total < bufferSize ) {
            sent = conn.Send ( data, total, dataLeft, 0 );
            total += sent;
            dataLeft -= sent;
        }

        return total;      
    }

    private bool ReadWriteStream(Socket conn) {
        NetworkStream stream = new NetworkStream(conn);

        int recv = 0;
        string stringData = "";
        byte[] data = new byte[1024];

        if ( stream.CanRead ) {
            recv = stream.Read ( data, 0, data.Length );
            stringData = Encoding.ASCII.GetString ( data, 0, recv );
            Debug.Log ( stringData );
        } else {
            Debug.Log ( "Error: cannot read from socket." );
            stream.Close ( );
            serverConn.Close ( );
            return false;
        }

        // should be in a while loop
        string str = "tessssssticles spectacles wallets and watch";

        //if ( i > 1000 ) {
          //  break;
        //}

        if ( stream.CanWrite ) {
            stream.Write ( Encoding.ASCII.GetBytes ( str ), 0, str.Length );
            stream.Flush ( );
        }

        recv = stream.Read ( data, 0, data.Length );
        stringData = Encoding.ASCII.GetString ( data, 0, recv );
        Debug.Log ( stringData );

        stream.Close ( );
        serverConn.Close ( );

        return true;
    }
}
