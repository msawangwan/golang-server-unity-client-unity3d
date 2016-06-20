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
        public Deck OwnerDeck { get; private set; }

        private PlayerComponent owner = null;
        private Button deckActiveGO = null;

        private void Start ( ) {
            deckActiveGO = GetComponent<Button> ( );

            OwnerDeck = null;
        }

        public void RegisterComponentWithOwner ( PlayerComponent player ) {
            owner = player; // assign owning player

            if ( owner != null ) { // assign owning player's deck
                if ( OwnerDeck == null ) {
                    OwnerDeck = owner.PlayerReference.PlayerDeck;
                }
            }

            OnCardDraw ( );
        }

        private void OnCardDraw() {
            deckActiveGO.onClick.RemoveAllListeners ( );
            deckActiveGO.onClick.AddListener ( ( ) => {
                int maxCardsAllowed = owner.PlayerReference.PlayerHand.HandSize;
                int numCardsInHand = owner.PlayerReference.PlayerHand.HandCount;

                if ( numCardsInHand <= maxCardsAllowed ) {
                    Card c = OwnerDeck.DrawFromTopOfActivePile();
                    owner.PlayerReference.PlayerHand.AddToHand ( c );
                }

            } );
        }
    }
}
