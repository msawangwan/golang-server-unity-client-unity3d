using UnityEngine;
using UnityEngine.UI;

namespace madMeesh.Cards {
    [RequireComponent(typeof(Button))]
    public class HandSlotComponent : MonoBehaviour {
        public Hand OwnerHand { get; private set; }
        public Card CardInSlot { get; private set; }

        public bool IsOccupied { get; private set; }

        private PlayerComponent owner = null;
        private Button cardInSlot;
        private Text slotText;
        
        public bool IsSlotOccupied ( ) {
            if ( IsOccupied )
                return true;
            return false;
        }

        public void AddCardToSlot ( Card card ) {
            if ( IsOccupied == false ) {
                if ( CardInSlot == null ) {
                    CardInSlot = card;
                    slotText.text = CardInSlot.PrintCard ( );
                }
            }
        }

        private void Start() {
            OwnerHand = null;
            CardInSlot = null;

            cardInSlot = GetComponent<Button> ( );
            slotText = GetComponentInChildren<Text> ( );

            IsOccupied = false;
        }
    }
}
