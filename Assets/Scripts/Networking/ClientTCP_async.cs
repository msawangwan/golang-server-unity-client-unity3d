using UnityEngine;
using System.Threading;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class ClientTCP_async {
    private const string serverAddr = "127.0.0.1";
    private const int serverPort = 9080;

    private static Socket serverSocket;
    private static IPAddress serverIP;
    private static IPEndPoint serverEndpoint;

    private NetworkStream socketStream;

    //private byte[] buffer;
    //private byte[] bufferTmp;

    private int connRetryAttempts = 0;
    private bool isConnected = false;
    private bool hasFailedToConnect = false;

    /* Default constructor */
    public ClientTCP_async ( ) {
        serverIP = IPAddress.Parse ( serverAddr );
        serverEndpoint = new IPEndPoint ( serverIP, serverPort );
    }

    public void Connect ( ) {
        serverSocket = OpenTCPSocket ( );
        serverSocket.BeginConnect ( serverEndpoint, new System.AsyncCallback ( OnConnect ), null );
    }

    public void SendData ( string data ) {
        try {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            serverSocket.BeginSend ( buffer, 0, buffer.Length, SocketFlags.None, new System.AsyncCallback ( OnSend ), null );
        } catch (SocketException se) {
            Debug.Log ( se );
        } catch (System.Exception e) {
            Debug.Log ( e );
        }
    }

    private void OnConnect ( System.IAsyncResult ar ) {
        try {
            serverSocket.EndConnect ( ar );
            isConnected = true;
        } catch ( System.Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnSend ( System.IAsyncResult ar ) {
        try {
            serverSocket.EndSend ( ar );
        } catch (System.Exception e) {
            Debug.Log ( e );
        }
    }

    private static Socket OpenTCPSocket ( ) {
        return new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
    }
}
