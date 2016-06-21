using System;
using System.Collections.Generic;

namespace madMeesh.Cards {
    public class Hand {
        public int HandSize { get; private set; } // max cards allowed in hand
        public int HandCount { get; private set; } // current count of cards in hand

        public Action<CardAction> RaiseAddedCardToHand;
        public Action<CardAction> RaisePlayedCardFromHand;

        private Card[] hand;

        public Hand(int handSize) {
            HandSize = handSize;
            HandCount = 0;

            hand = new Card[HandSize];
            Init ( );
        }

        public int AddToHand ( Card card ) {
            if ( HandCount < HandSize ) {
                UnityEngine.Debug.Log ( HandCount );
                hand[HandCount] = card;

                CardAction added = new CardAction ( card, HandCount );
                RaiseAddedCardToHand ( added );

                ++HandCount;

                return HandCount;
            }

            return -1;
        }

        public Card PlayFromHand(int handIndex) {
            if ( handIndex >= 0 && handIndex < HandSize ) {
                Card c = hand[handIndex];
                hand[handIndex] = null;
                --HandCount;

                CardAction played = new CardAction ( c );
                RaisePlayedCardFromHand ( played );

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