using UnityEngine;
using System.Collections;
using System;

namespace madMeesh.TurnBasedEngine {
    public class EndTurnPhase : Phase {
        public EndTurnPhase( TurnController phaseOwner ) : base( phaseOwner ) {
            Debug.Log ( "End turn" );
        }

        protected override void PhaseUpdateLoop ( ) {
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