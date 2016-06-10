using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    private ClientTCP clientConnection_async;

	void Start () {
        clientConnection_async = new ClientTCP ( );
    }

    bool isInitialConnection = true;
    bool hasConnected = false;

    public void Test_CloseConnection() {
        if (hasConnected) {
            isInitialConnection = true;
            hasConnected = false;
        }
    }

    public void Test_async_connect() {
        Debug.Log ( "Attempting to connect -- listening for server reply .. " );
        clientConnection_async.Connect ( );
        clientConnection_async.Listen ( );
    }

    public void Test_async_disconnect() {
        Debug.Log ( "Disconnecting" );
        clientConnection_async.Disconnect ( );
    }
    
    public void Test_async_send() {
        clientConnection_async.Send ( "CLIENT SENT ASYNCHH" );
    }
}
