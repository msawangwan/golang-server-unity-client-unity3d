using UnityEngine;
using UnityEngine.UI;

namespace madMeesh.Cards {
    [RequireComponent(typeof(Text))]
    public class CardViewComponent : MonoBehaviour {
        private PlayerComponent owner;
        private DeckComponent deck;

        private Text viewText;
        
        public void RegisterComponentWithOwner ( PlayerComponent player) {
            owner = player;
            deck = owner.DeckGameObject;

            viewText = GetComponent<Text> ( );
            viewText.resizeTextForBestFit = true;

            deck.DeckOwner.RaiseCardDrawn += HandleOnCardDraw;
        }

        private void HandleOnCardDraw( CardAction drawActionArgs ) {
            viewText.text = drawActionArgs.CardInAction.PrintCard ( );
        }
    }
}
