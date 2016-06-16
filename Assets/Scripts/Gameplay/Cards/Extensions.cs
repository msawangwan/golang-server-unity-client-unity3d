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

            byte[] cardData = new byte[12];

            int offset = 0;
            System.Buffer.BlockCopy ( colorData, 0, cardData, offset, colorData.Length );
            offset += colorData.Length;
            System.Buffer.BlockCopy ( suitData, 0, cardData, offset, suitData.Length );
            offset += suitData.Length;
            System.Buffer.BlockCopy ( valueData, 0, cardData, offset, valueData.Length );

            //Debug.Log ( "carddata: " + System.BitConverter.ToString ( cardData ) );
            return cardData;
        }

        public static Card CardDeserializer ( this Deck d, byte[] b ) {
            Card c = new Card();
            Debug.Log ( "savedcard: " + System.BitConverter.ToString ( b ) );
            int offset = 0;
            c.CardColor = (Color) DeserializeInt ( b, offset );
            offset += 4;
            c.CardSuit = (Suit) DeserializeInt ( b, offset );
            offset += 4;
            c.CardValue = (Value) DeserializeInt ( b, offset );            
            
            return c;
        }

        public static byte[] DeckSerializer ( this Deck d ) {
            byte[] deckData = new byte[(12 * d.Size)];
            int offset = 0;

            for ( int i = 0; i < d.Size; i++ ) {
                if ( d[i] != null ) {
                    Debug.Log ( d[i].PrintCard ( ) );
                    byte[] cardData = d[i].CardSerializer();
                    System.Buffer.BlockCopy ( cardData, 0, deckData, offset, cardData.Length );
                    offset += cardData.Length;
                }
            }

            //Debug.Log ( "DECKDATA: " + System.BitConverter.ToString ( deckData ) );
            return deckData;
        }

        public static void DeckDeserializer (this Deck d, byte[] b) {
            //Debug.Log ( "SAVEDDECK: " + System.BitConverter.ToString ( b ) );
            byte[] currentCard = new byte[12];
            for ( int i = 1; i < b.Length + 1; i++ ) {
                if ( i % 12 == 0 ) {
                    Card c = d.CardDeserializer(currentCard);
                    Debug.Log ( c.PrintCard ( ) );

                    d.Add ( c );
                    currentCard = new byte[12];
                }

                if ( i == b.Length ) {
                    break;
                }

                currentCard[i % 12] = b[i];
            }
        }

        public static byte[] SerializeInt ( int obj ) {
            int readyForNetwork = System.Net.IPAddress.HostToNetworkOrder ( obj );
            return System.BitConverter.GetBytes ( readyForNetwork );
        }

        public static int DeserializeInt ( byte[] section, int offset ) {
            int nbo = System.BitConverter.ToInt32(section, offset);
            return System.Net.IPAddress.NetworkToHostOrder ( nbo );
        }
    }
}
