
namespace madMeesh.Cards {
    public enum Color {
        None = 0,
        Red,
        Black,
    }

    public enum Suit {
        None = 0,
        Hearts,
        Diamonds,
        Spades,
        Clubs,
    }

    public enum Value {
        None  = 0,
        Joker = 1,
        Two   = 2,
        Three = 3,
        Four  = 4,
        Five  = 5,
        Six   = 6,
        Seven = 7,
        Eight = 8,
        Nine  = 9,
        Ten   = 10,
        Jack  = 11,
        Queen = 13,
        King  = 14,
        Ace   = 15,
    }

    public class Card {
        public Color CardColor { get; set; }
        public Suit CardSuit { get; set; }
        public Value CardValue { get; set; }

        public Card() {
            CardColor = Color.None;
            CardSuit = Suit.None;
            CardValue = Value.None;
        }

        public Card(Color color, Suit suite, Value val) {
            CardColor = color;
            CardSuit = suite;
            CardValue = val;
        }

        public string PrintCard() {
            string c = CardColor.ToString();
            string s = CardSuit.ToString();
            string v = CardValue.ToString();
            return c + " " + v + " of " + s;
        }
    }
}