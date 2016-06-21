using UnityEngine;
using UnityEngine.UI;

namespace madMeesh.Cards {
    public class TableComponent : MonoBehaviour {
        private PlayerComponent owner = null;
        private CardComponent[] cardsInPlay = null;

        private GridLayoutGroup grid;
        private Vector2 cardDimensions = new Vector2 ( 32, 32 );

        private const int maxCardsAllowedInPlay = 52;
        private int numCardsInPlay = 0;

        public void RegisterComponentWithOwner ( PlayerComponent player ) {
            owner = player;
            owner.PlayerReference.PlayerHand.RaisePlayedCardFromHand += HandleOnCardPlayedToTable;

            cardsInPlay = new CardComponent[maxCardsAllowedInPlay];
        }

        private void Start() {
            grid = GetComponent<GridLayoutGroup> ( );
        }

        private void HandleOnCardPlayedToTable ( CardAction fromHand ) {
            GameObject card = InstantiateCard ( gameObject.transform );
            cardsInPlay[numCardsInPlay] = card.GetComponent<CardComponent> ( );
            card.GetComponentInChildren<Text> ( ).text = fromHand.CardInAction.PrintCard ( );

            numCardsInPlay++;
        }

        private static GameObject InstantiateCard (Transform parentTform) {
            GameObject card = new GameObject();
            card.AddComponent<CardComponent> ( );
            card.AddComponent<RectTransform> ( );
            card.AddComponent<Image> ( );
            card.transform.SetParent ( parentTform , false );

            GameObject cardText = new GameObject();
            cardText.AddComponent<RectTransform> ( );
            cardText.AddComponent<Text> ( ); // TODO: need to format
            cardText.transform.SetParent ( card.transform , false );

            return card;
        }
    }
}