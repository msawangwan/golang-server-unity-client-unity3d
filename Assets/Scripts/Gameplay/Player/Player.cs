using UnityEngine;
using madMeesh.Cards;
using System.Collections;

public class Player : MonoBehaviour {
    Hand hand = new Hand(52);
    Deck deck = new Deck();

	void Start () {
        deck.Shuffle ( );
        PrintDeck ( );
	}

    void PrintDeck() {
        for ( int i = 0; i < 52; i++ ) {
            Debug.Log ( deck[i].PrintCard( ) );
        }
    }
}
