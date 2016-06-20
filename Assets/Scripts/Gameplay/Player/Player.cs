using UnityEngine;
using madMeesh.Cards;
using madMeesh.TurnBasedEngine;
using System.Collections;

public class Player {
    public Hand PlayerHand { get; set; }
    public Deck PlayerDeck { get; set; }

    public TurnController PlayerTurnController { get; set; }

    public ClientTCP PlayerNetConn { get; set; }

    public bool IsTurn { get; set; }

	public Player() {
        Init ( );
    }

    public void InitialiseNew() {
        PlayerHand = new Hand ( 5 );
        PlayerDeck = new Deck ( true );

        PlayerDeck.AddDeckToActivePile ( );
    }

    private void Init() {
        PlayerTurnController = new TurnController ( this );
        PlayerNetConn = new ClientTCP ( );
    }
}
