using UnityEngine;
using System.Collections.Generic;

namespace madMeesh.Cards {
    public class Deck {        
        public Card this[int i] {
            get {
                if ( i >= 0 && i < deck.Length ) {
                    return deck[i];
                }
                return null;
            }
            set {
                if ( i >= 0 && i < deck.Length ) {
                    deck[i] = value;
                }
            }
        }

        public int Size { get; private set; }

        private Card[] deck;
        private Queue<Card> activePile;
        private Stack<Card> discardPile;

        public Deck (bool createWithNewCards) {
            Size = 52;

            deck = new Card[Size];
            activePile = new Queue<Card> ( );
            discardPile = new Stack<Card> ( );

            if ( createWithNewCards ) {
                InitNewDeck ( );
            }
        }

        public Card DrawFromTopActivePile() {
            if ( activePile.Count > 0 && activePile.Peek ( ) != null ) {
                return activePile.Dequeue ( );
            }
            return null;
        }

        public Card PeekTopCardActivePile() {
            return activePile.Peek ( );
        }

        public void AddToBottomOfActivePile(Card card) {
            activePile.Enqueue ( card );
        }

        public void AddToTopOfDiscardPile(Card card) {
            discardPile.Push ( card );
        }

        /* Shuffles the deck. */
        public void ShuffleNewDeck() {
            Card[] deckCopy = new Card[52];
            int numShuffled = 0;

            while ( numShuffled < 52 ) {
                int randCard = UnityEngine.Random.Range(0, 52);

                if ( deck[randCard] == null ) {
                    continue;
                } else {
                    Card current = deck[randCard];

                    deck[randCard] = null;
                    deckCopy[numShuffled] = current;

                    activePile.Enqueue ( current );

                    ++numShuffled;
                }
            }

            deck = deckCopy;
        }

        /* Creates a new deck of cards. */
        private void InitNewDeck ( ) {
            Card[] temp = new Card[52];

            for ( int i = 0; i < temp.Length; i++ ) {
                Card c = new Card();

                if ( i % 2 == 0 ) {
                    c.CardColor = Color.Black;
                } else if ( i % 2 == 1 ) {
                    c.CardColor = Color.Red;
                }

                if ( i % 4 == 0 ) {
                    c.CardSuit = Suit.Clubs;
                } else if ( i % 4 == 1 ) {
                    c.CardSuit = Suit.Diamonds;
                } else if ( i % 4 == 2 ) {
                    c.CardSuit = Suit.Hearts;
                } else if ( i % 4 == 3 ) {
                    c.CardSuit = Suit.Spades;
                }

                if ( i % 13 == 0 ) {
                    c.CardValue = Value.Two;
                } else if ( i % 13 == 1 ) {
                    c.CardValue = Value.Three;
                } else if ( i % 13 == 2 ) {
                    c.CardValue = Value.Four;
                } else if ( i % 13 == 3 ) {
                    c.CardValue = Value.Five;
                } else if ( i % 13 == 4 ) {
                    c.CardValue = Value.Six;
                } else if ( i % 13 == 5 ) {
                    c.CardValue = Value.Seven;
                } else if ( i % 13 == 6 ) {
                    c.CardValue = Value.Eight;
                } else if ( i % 13 == 7 ) {
                    c.CardValue = Value.Nine;
                } else if ( i % 13 == 8 ) {
                    c.CardValue = Value.Ten;
                } else if ( i % 13 == 9 ) {
                    c.CardValue = Value.Jack;
                } else if ( i % 13 == 10 ) {
                    c.CardValue = Value.Queen;
                } else if ( i % 13 == 11 ) {
                    c.CardValue = Value.King;
                } else if ( i % 13 == 12 ) {
                    c.CardValue = Value.Ace;
                }

                this[i] = c;
            }
        }
    }
}