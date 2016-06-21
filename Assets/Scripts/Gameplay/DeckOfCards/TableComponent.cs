using UnityEngine;
using UnityEngine.UI;

namespace madMeesh.Cards {
    public class TableComponent : MonoBehaviour {
        private PlayerComponent owner = null;
        private CardComponent[] cardsInPlay = null;

        private const int maxCardsAllowedInPlay = 52;
        private int numCardsInPlay = 0;

        public void RegisterComponentWithOwner ( PlayerComponent player ) {
            owner = player;
            owner.PlayerReference.PlayerHand.RaisePlayedCardFromHand += HandleOnCardPlayedToTable;

            cardsInPlay = new CardComponent[maxCardsAllowedInPlay];
        }

        private void HandleOnCardPlayedToTable ( CardAction fromHand ) {
            Button card = new GameObject().AddComponent<Button>();
            cardsInPlay[numCardsInPlay] = card.gameObject.AddComponent<CardComponent>();
            card.gameObject.AddComponent<RectTransform> ( );
            card.gameObject.AddComponent<CanvasRenderer> ( );
            card.gameObject.AddComponent<Image> ( );
            card.transform.SetParent ( gameObject.transform, false );

            //card.transform.gameObject

            numCardsInPlay++;
        }
    }
}