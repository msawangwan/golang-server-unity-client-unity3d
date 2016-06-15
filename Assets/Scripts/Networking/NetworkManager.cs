using UnityEngine;
using madMeesh.Cards;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    private ClientTCP clientConnection_async;
    private Player player;

	void Start () {
        clientConnection_async = new ClientTCP ( );
        player = FindObjectOfType<Player> ( );

        clientConnection_async.RaiseDataFrameRecvd += HandleOnDataFrameRecvd;
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
        clientConnection_async.ListenAndRecvAsync ( );
    }

    public void Test_async_disconnect() {
        Debug.Log ( "Disconnecting" );
        clientConnection_async.Disconnect ( );
    }
    
    public void Test_async_send() {
        Card card = player.deck.Draw ( );
        player.deck.Add ( card );
        byte[] d = player.deck.DeckSerializer ( );

        clientConnection_async.SendAsync ( d );
    }

    private void HandleOnDataFrameRecvd ( RecvdDataFrameEventArgs e ) {

    }
}
