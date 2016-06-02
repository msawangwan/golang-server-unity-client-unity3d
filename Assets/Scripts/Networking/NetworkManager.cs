using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    private ClientTCP clientConnection;

	void Start () {
        clientConnection = new ClientTCP ( );
	}

	void Update () {
	
	}

    public void Test_SendDataToServer() {
        clientConnection.EstablishConnection ( );
    }
}
