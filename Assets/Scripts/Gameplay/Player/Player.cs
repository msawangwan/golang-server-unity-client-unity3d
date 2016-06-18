using UnityEngine;
using madMeesh.Cards;
using madMeesh.TurnBasedEngine;
using System.Collections;

public class Player {
    public Hand hand = new Hand(52);
    public Deck deck = new Deck(true);

    public PlayerTurnController TurnController { get; set; }

    public ClientTCP netConn { get; set; }

    public bool IsTurn { get; set; }

	public Player() {
        TurnController = new PlayerTurnController ( );
        deck.ShuffleNewDeck ( );
    }

    public void PrintDeck() {
        for ( int i = 0; i < 52; i++ ) {
            Debug.Log ( deck[i].PrintCard( ) );
        }
    }
}
