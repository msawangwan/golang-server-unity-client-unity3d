using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameObject Component.
/// 
/// Represents a deck of cards the player draws from.
/// </summary>

namespace madMeesh.Cards {
    [RequireComponent(typeof(Button))]
    public class DeckComponent : MonoBehaviour {
        public Deck DeckOwner { get; private set; }

        private PlayerComponent owner = null;
        private Button drawDeckGameObject = null;

        public void RegisterComponentWithOwner ( PlayerComponent player ) {
            owner = player; // assign owning player

            if ( owner != null ) { // assign owning player's deck
                if ( DeckOwner == null ) {
                    DeckOwner = owner.PlayerReference.PlayerDeck;
                }
            }

            OnCardDraw ( );
        }

        private void Start ( ) {
            drawDeckGameObject = GetComponent<Button> ( );
        }

        /* Registers onClick button listener. */
        private void OnCardDraw ( ) {
            drawDeckGameObject.onClick.RemoveAllListeners ( );
            drawDeckGameObject.onClick.AddListener ( ( ) => {
                int maxCardsAllowed = owner.PlayerReference.PlayerHand.HandSize;
                int numCardsInHand = owner.PlayerReference.PlayerHand.HandCount;

                if ( numCardsInHand < maxCardsAllowed ) {
                    Card c = DeckOwner.DrawFromTopOfActivePile();
                    owner.PlayerReference.PlayerHand.AddToHand ( c );
                }
            } );
        }
    }
}
