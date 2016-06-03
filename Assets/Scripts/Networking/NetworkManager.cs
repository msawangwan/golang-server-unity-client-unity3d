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
}
