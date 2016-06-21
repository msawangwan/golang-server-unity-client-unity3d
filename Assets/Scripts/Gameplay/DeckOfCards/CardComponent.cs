using UnityEngine;
using UnityEngine.UI;

namespace madMeesh.Cards {
    [RequireComponent(typeof(Button))]
    public class CardComponent : MonoBehaviour {
        public Card CardReference { get; private set; }

        private Button CardGameObject = null;
    }
}