using UnityEngine;
using UnityEngine.UI;

namespace madMeesh.Cards {
    public class HandComponent : MonoBehaviour {
        public Hand OwnerHand { get; private set; }

        private PlayerComponent owner = null;
        private HandSlotComponent[] handSlots;

        private int currentEmptySlot = 0;
        private bool isFullHand = false;

        public void RegisterComponentWithOwner ( PlayerComponent player ) {
            owner = player;

            OwnerHand = owner.PlayerReference.PlayerHand;
            OwnerHand.RaiseAddedCardToHand += HandleOnCardAdded;
            OwnerHand.RaisePlayedCardFromHand += HandleOnCardRemoved;

            handSlots = FindObjectsOfType<HandSlotComponent> ( );

            for ( int i = 0; i < handSlots.Length; i++ ) {
                handSlots[i].RegisterParentHandComponent ( owner );
            }
        }

        private void HandleOnCardAdded ( CardAction added ) {
            int slotToAddTo = added.CardID;
            if ( slotToAddTo == currentEmptySlot ) {
                if ( isFullHand == false ) {
                    handSlots[slotToAddTo].AddCard ( added.CardInAction, slotToAddTo );
                    currentEmptySlot++;
                }
            }
        }

        private void HandleOnCardRemoved ( CardAction removed ) {
            currentEmptySlot--;
        }
    }
}