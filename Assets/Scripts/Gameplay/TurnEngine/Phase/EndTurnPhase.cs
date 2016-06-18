using UnityEngine;
using System.Collections;
using System;

namespace madMeesh.TurnBasedEngine {
    public class EndTurnPhase : Phase {
        public EndTurnPhase() : base() { }

        protected override void PhaseUpdateLoop ( ) {
            Debug.Log ( "End turn" );

            if ( IsPhaseComplete == false ) {
                IsPhaseComplete = true;
                return;
            }

            nextPhase = null;
            OnPhaseCompleted ( );

            HasCompletedPhase = true;
            IsPhaseComplete = false;
        }

        public override void SetNextPhase ( IPhase next ) {
            nextPhase = next;
        }
    }
}