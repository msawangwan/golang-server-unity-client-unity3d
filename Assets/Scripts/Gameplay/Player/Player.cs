using UnityEngine;
using madMeesh.Cards;
using System.Collections;

public class Player : MonoBehaviour {
    public Hand hand = new Hand(52);
    public Deck deck = new Deck(true);

	void Start () {
        deck.Shuffle ( );
	}

    public void PrintDeck() {
        for ( int i = 0; i < 52; i++ ) {
            Debug.Log ( deck[i].PrintCard( ) );
        }
    }
}
