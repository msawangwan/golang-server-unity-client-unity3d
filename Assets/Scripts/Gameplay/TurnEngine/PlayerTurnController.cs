using UnityEngine;
using System;

namespace madMeesh.TurnBasedEngine {
    public class PlayerTurnController {
        public bool HasStartedTurn { get; private set; }
        public bool HasCompletedTurn { get; private set; }

        private IPhase currentPhase;
        private IPhase nextPhase;

        public event Action RaiseTurnCompleted;

        public void StartTurn ( IPhase startOfTurnPhase ) {
            currentPhase = startOfTurnPhase;
            HasStartedTurn = true;
            HasCompletedTurn = false;
        }

        public void TakeTurn() {
            if ( currentPhase.HasEnteredPhase == false ) {
                currentPhase.RaisePhaseComplete += HandleOnPhaseCompleted;
                currentPhase.EnterPhase ( );
            }

            currentPhase.ExecutePhase ( );

            if ( currentPhase.HasCompletedPhase == false ) {
                Debug.Log ( "Phase not yet complete." );
                return;
            }

            currentPhase = nextPhase;

            if ( nextPhase == null ) {
                OnTurnCompleted ( );
                return;
            }

            nextPhase = null;

            currentPhase.RaisePhaseComplete -= HandleOnPhaseCompleted;
        }

        public void EndTurn() {
            HasCompletedTurn = true;
        }

        private void OnTurnCompleted() {
            if ( RaiseTurnCompleted != null ) {
                RaiseTurnCompleted ( );
            }
        }

        private void HandleOnPhaseCompleted(IPhase next) {
            nextPhase = next;
        }
    }
}
