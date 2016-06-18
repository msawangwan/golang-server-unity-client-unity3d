using UnityEngine;
using System.Collections;
using System;

namespace madMeesh.TurnBasedEngine {
    public class StartOfTurnPhase : Phase {
        public StartOfTurnPhase() : base() { }

        protected override void PhaseUpdateLoop ( ) {
            Debug.Log ( "Start of turn" );

            if ( IsPhaseComplete == false ) {
                IsPhaseComplete = true;
                return;
            }

            nextPhase = new DrawPhase ( );
            OnPhaseCompleted ( );

            HasCompletedPhase = true;
            IsPhaseExecuting = false;
        }

        /* For manually switching to a specific phase. */
        public override void SetNextPhase(IPhase next) {
            nextPhase = next;
        }
    }
}
