using UnityEngine;
using madMeesh.TurnBasedEngine;

namespace madMeesh {
    public class Main : MonoBehaviour {
        TurnEngineComponent turnengine;
        PlayerComponent p1component;

        void Start() {
            turnengine = FindObjectOfType<TurnEngineComponent> ( );
            p1component = FindObjectOfType<PlayerComponent> ( );
            p1component.Reference = new Player ( );
            turnengine.EnqueueTurnTaker ( p1component.Reference.TurnController );
        }
    }
}