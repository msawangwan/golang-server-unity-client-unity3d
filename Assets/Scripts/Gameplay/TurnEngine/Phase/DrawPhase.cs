using UnityEngine;
using System.Collections;
using System;

namespace madMeesh.TurnBasedEngine {
    public class DrawPhase : Phase {
        public DrawPhase() : base() { }

        int testcount = 100;

        protected override void PhaseUpdateLoop ( ) {
            Debug.Log ( "Draw Phase" );

            if ( IsPhaseComplete == false ) {
                if ( testcount > 0 ) {
                    testcount--;
                    return;
                }
            }

            nextPhase = new EndTurnPhase ( );
            OnPhaseCompleted ( );

            HasCompletedPhase = true;
            IsPhaseExecuting = false;
        }

        public override void SetNextPhase ( IPhase next ) {
            nextPhase = next;
        }
    }
}
