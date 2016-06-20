using UnityEngine;
using madMeesh.TurnBasedEngine;

namespace madMeesh {
    public class Main : MonoBehaviour {
        TurnEngineComponent turnengine;
        PlayerComponent p1component;

        void Start() {
            turnengine = FindObjectOfType<TurnEngineComponent> ( );
            p1component = FindObjectOfType<PlayerComponent> ( );
            p1component.PlayerReference = new Player ( );
            p1component.PlayerReference.InitialiseNew ( );
            turnengine.EnqueueTurnTaker ( p1component.PlayerReference.PlayerTurnController );
        }
    }
}