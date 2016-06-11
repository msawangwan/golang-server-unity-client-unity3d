using System.Collections;
using System.Collections.Generic;

namespace madMeesh.Cards {
    public class Hand {
        public int HandSize { get; set; }

        private Card[] hand;
        private int handIndex = 0;

        public Hand(int handSize) {
            HandSize = handSize;

            hand = new Card[HandSize];
            Init ( );
        }

        public int AddToHand(Card card) {
            hand[handIndex] = card;
            ++handIndex;
            return handIndex;
        }

        public Card PlayFromHand(int card) {
            if ( card >= 0 && card < HandSize ) {
                Card c = hand[card];
                hand[card] = null;
                --handIndex;
                return c;
            }
            return null;
        }

        public IEnumerable<Card> ShowHand() {
            for ( int i = 0; i < hand.Length; i++ ) {
                if ( hand[i] == null )
                    continue;
                yield return hand[i];
            }
        }

        private void Init() {
            for ( int i = 0; i < hand.Length; i++ ) {
                hand[i] = null;
            }
        }
    }
}