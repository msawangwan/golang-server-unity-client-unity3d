using UnityEngine;
using UnityEngine.UI;

namespace madMeesh.Cards {
    [RequireComponent(typeof(Button))]
    public class HandSlotComponent : MonoBehaviour {
        public Hand OwnerHand { get; private set; }
        public Card CardInSlot { get; private set; }

        public bool IsOccupied { get; private set; }

        private PlayerComponent owner = null;
        private Button slotGameObject;
        private Text slotText;

        private int slotID = -1;
        private const string defaultText = "<EMPTY>";

        public void RegisterParentHandComponent ( PlayerComponent player ) {
            owner = player;
            OwnerHand = owner.PlayerReference.PlayerHand;
        }

        public void AddCard ( Card card, int slotIndex ) {
            if ( IsOccupied == false && CardInSlot == null ) {
                CardInSlot = card;
                slotID = slotIndex;
                slotText.text = CardInSlot.PrintCard ( );

                IsOccupied = true;
            }
        }

        private void Start() {
            CardInSlot = null;

            slotGameObject = GetComponent<Button> ( );
            slotText = GetComponentInChildren<Text> ( );

            slotText.text = defaultText;

            IsOccupied = false;

            OnCardPlayedFromHand ( );
        }

        private void OnCardPlayedFromHand ( ) {
            slotGameObject.onClick.RemoveAllListeners ( );
            slotGameObject.onClick.AddListener ( ( ) => {
                if ( IsOccupied == true && CardInSlot != null ) {
                    if ( slotID != -1 ) {
                        OwnerHand.PlayFromHand ( slotID );
                        CardInSlot = null;
                        slotText.text = defaultText;

                        slotID = -1;
                        IsOccupied = false;
                    }
                }
            } );
        }
    }
}
