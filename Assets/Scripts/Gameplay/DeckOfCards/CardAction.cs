namespace madMeesh.Cards {
    public class CardAction {
        public readonly Card CardInAction;
        public readonly int CardID = 0;

        public CardAction ( Card c ) {
            CardInAction = c;
        }

        public CardAction ( Card c, int id ) {
            CardInAction = c;
            CardID = id;
        }
    }
}