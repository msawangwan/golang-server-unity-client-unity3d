using UnityEngine;
using madMeesh.Cards;
using System;

public class NetworkManager : MonoBehaviour {
    private ClientTCP clientConnection;
    private Player player;
    private DataFramePacket packet = null;
    private Deck saved = new Deck(false);

    private bool hasRecvdData = false;

	void Start () {
        clientConnection = new ClientTCP ( );
        //player = new Player ( );

        clientConnection.RaiseDataFrameRecvd += HandleOnDataFrameRecvd;
    }
    
    void Update() {
        if ( hasRecvdData ) {
            Debug.Log ( "got data" );
            //Debug.Log ( BitConverter.ToString ( packet.Frame ) );

            hasRecvdData = false;
        }
    }

    public void Test_async_connect() {
        Debug.Log ( "Attempting to connect -- listening for server reply .. " );
        clientConnection.Connect ( );
        clientConnection.ListenAndRecvAsync ( );
    }

    public void Test_async_disconnect() {
        Debug.Log ( "Disconnecting" );
        clientConnection.Disconnect ( );
    }
    
    public void Test_async_send() {
        Debug.Log ( "Sending TestData from host to client" );
        Card card = player.PlayerDeck.DrawFromTopOfActivePile ( );
        player.PlayerDeck.AddToBottomOfActivePile ( card );
        byte[] d = player.PlayerDeck.DeckSerializer ( );

        clientConnection.SendAsync ( d );
    }

    private void HandleOnDataFrameRecvd ( RecvdDataFrameEventArgs e ) {
        Debug.Log ( "handling" );
        if ( e.Status == DataFrameStatus.Normal ) {
            packet = e.Data;
            saved.DeckDeserializer ( packet.Frame );
            hasRecvdData = true;
        }
    }
}
