using UnityEngine;
using madMeesh.Cards;
using System.Collections;
namespace madMeesh.Cards {
    public static class Extensions {
        public static byte[] CardSerializer ( this Card c ) {
            int cardColor = (int) c.CardColor;
            int cardSuite = (int) c.CardSuit;
            int cardValue = (int) c.CardValue;

            byte[] colorData = SerializeInt(cardColor);
            byte[] suitData = SerializeInt(cardSuite);
            byte[] valueData = SerializeInt(cardValue);

            byte[] cardData = new byte[16];

            int offset = 0;
            System.Buffer.BlockCopy ( colorData, 0, cardData, offset, colorData.Length );
            offset += colorData.Length;
            System.Buffer.BlockCopy ( suitData, 0, cardData, offset, suitData.Length );
            offset += suitData.Length;
            System.Buffer.BlockCopy ( valueData, 0, cardData, offset, valueData.Length );
            offset += valueData.Length;

            return cardData;
        }

        public static byte[] DeckSerializer ( this Deck d ) {
            byte[] deckData = new byte[(16 * d.Size)];
            int offset = 0;

            for ( int i = 0; i < d.Size; i++ ) {
                if ( d[i] != null ) {
                    byte[] cardData = d[i].CardSerializer();
                    Debug.Log ( "carddata " + cardData );
                    System.Buffer.BlockCopy ( cardData, 0, deckData, offset, cardData.Length );
                    offset += cardData.Length;
                }
            }

            return deckData;
        }

        public static byte[] SerializeInt ( int obj ) {
            return System.BitConverter.GetBytes ( obj );
        }
    }
}
