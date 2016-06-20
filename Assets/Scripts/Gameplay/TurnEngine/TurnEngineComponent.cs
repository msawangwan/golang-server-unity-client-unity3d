using UnityEngine;
using System.Collections.Generic;

namespace madMeesh.TurnBasedEngine {
    public class TurnEngineComponent : MonoBehaviour {
        private Queue<TurnController> turnQueue = new Queue<TurnController> ( );

        private IPhase startPhase;
        private IPhase nextPhase = null;

        private TurnController turnTaker;
        private TurnController currentTurnTaker {
            set {
                turnTaker = value;
                startPhase = new StartOfTurnPhase ( value ); // pass in current turn taker as owner of phase

                turnTaker.RaiseTurnCompleted += HandleOnTurnCompleted;
                turnTaker.StartTurn ( startPhase );
            }
        }

        public void EnqueueTurnTaker ( TurnController player ) {
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
