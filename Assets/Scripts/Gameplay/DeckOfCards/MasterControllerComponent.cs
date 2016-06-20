using UnityEngine;
using System.Collections;

namespace madMeesh.Cards {
    public class MasterControllerComponent : MonoBehaviour {
        private PlayerComponent owner;

        private DeckComponent deck;
        private HandComponent hand;
        private CardViewComponent cardView;

        public void SetOwner ( PlayerComponent player ) {
            owner = player;

            deck = owner.DeckGO;
            hand = owner.HandGO;
            cardView = owner.CardViewGO;

            deck.RegisterComponentWithOwner ( owner );
            hand.RegisterComponentWithOwner ( owner );
            cardView.RegisterComponentWithOwner ( owner );
        }
    }
}