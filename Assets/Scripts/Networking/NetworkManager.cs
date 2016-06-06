using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    private ClientTCP clientConnection;
    private ClientTCP_async clientConnection_async;

	void Start () {
        clientConnection = new ClientTCP ( );
        clientConnection_async = new ClientTCP_async ( );

    }

	void Update () {
	
	}

    bool isInitialConnection = true;
    bool hasConnected = false;

    public void Test_SendDataToServer() {
        if ( isInitialConnection ) {
            hasConnected = clientConnection.EstablishConnection ( );
            if ( hasConnected ) {
                isInitialConnection = false;
            }
        }

        if ( hasConnected ) {
            string serverResponse = clientConnection.SendAndReceiveData ( "tessssssticles spectacles wallets and watch" );
            Debug.Log ( "Response From Server: " + serverResponse );
        }       
    }

    public void Test_CloseConnection() {
        if (hasConnected) {
            clientConnection.CloseConnection ( );
            isInitialConnection = true;
            hasConnected = false;
        }
    }

    public void Test_async_connect() {
        clientConnection_async.Connect ( );
    }

    public void Test_async_disconnect() {
        clientConnection_async.Disconnect ( );
    }
    
    public void Test_async_send() {
        clientConnection_async.SendData ( "CLIENT SENT ASYNCHH" );
    }

    public void Test_async_sendAndRecv() {
        string recvd = clientConnection_async.SendAndRecvData ( "CLIENT SEND AND RECVD ASYNC" );

        if ( recvd == string.Empty ) {
            Debug.Log ( "Failed to recv" );
            return;
        } else {
            Debug.Log ( "RECVD FROM SERVER ASYNC: " + recvd );
            Debug.Log ( "SIZE OF RECVD: " + recvd.Length );
        }
    }
}
