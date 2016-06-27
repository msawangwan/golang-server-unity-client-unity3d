using UnityEngine;
using madMeesh.TurnBasedEngine;

namespace madMeesh {
    public class Main : MonoBehaviour {
        TurnEngineComponent turnengine;
        PlayerComponent p1component;

        void Start() {
           // turnengine = FindObjectOfType<TurnEngineComponent> ( );
           // Player player = new Player ( );
           // player.InitialiseNew ( );

           // p1component = FindObjectOfType<PlayerComponent> ( );
            //p1component.RegisterPlayerReference ( player );
           // turnengine.EnqueueTurnTaker ( p1component.PlayerReference.PlayerTurnController );
        }
    }
}