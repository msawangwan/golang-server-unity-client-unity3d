using UnityEngine;
using System.Collections.Generic;

namespace madMeesh.TurnBasedEngine {
    public class TurnEngineComponent : MonoBehaviour {
        private Queue<PlayerTurnController> turnQueue = new Queue<PlayerTurnController> ( );

        private IPhase startPhase;
        private IPhase nextPhase = null;

        private PlayerTurnController turnTaker;
        private PlayerTurnController currentTurnTaker {
            set {
                turnTaker = value;
                startPhase = new StartOfTurnPhase();

                turnTaker.RaiseTurnCompleted += HandleOnTurnCompleted;
                turnTaker.StartTurn ( startPhase );
            }
        }

        public void EnqueueTurnTaker ( PlayerTurnController player ) {
            turnQueue.Enqueue ( player );
        }

        private void Update ( ) {
            if ( turnTaker == null ) {
                if ( turnQueue.Count > 0 ) {
                    currentTurnTaker = turnQueue.Dequeue ( );
                    turnQueue.Enqueue ( turnTaker );
                } else {
                    return; // no turn taker in queue
                }
            }

            if ( turnTaker.HasCompletedTurn == false ) {
                Debug.Log ( "Taking Turn" );
                turnTaker.TakeTurn ( );
                return;
            }

            turnTaker.RaiseTurnCompleted -= HandleOnTurnCompleted;
            turnTaker = null;
        }

        private void HandleOnTurnCompleted ( ) {
            turnTaker.EndTurn ( );
        }
    }
}
