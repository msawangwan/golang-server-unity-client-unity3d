using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    private ClientTCP clientConnection;

	void Start () {
        clientConnection = new ClientTCP ( );
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
            clientConnection.SendData ( );
        }       
    }

    public void Test_CloseConnection() {
        clientConnection.CloseConnection ( );
        isInitialConnection = true;
        hasConnected = false;
    }
}
