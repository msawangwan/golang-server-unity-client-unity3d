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

            handSlots = FindObjectsOfType<HandSlotComponent> ( );
        }

        private void HandleOnCardAdded ( CardAction added ) {
            Debug.Log ( "Add " + added.CardID );
            int slotToAddTo = added.CardID;
            if ( slotToAddTo == currentEmptySlot ) {
                if ( isFullHand == false ) {
                    Debug.Log ( handSlots[slotToAddTo].name );
                    handSlots[slotToAddTo].AddCardToSlot ( added.CardInAction );

                    currentEmptySlot++;
                }
            }
        }
    }
}