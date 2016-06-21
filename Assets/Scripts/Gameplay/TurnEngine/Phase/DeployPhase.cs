using System;
using UnityEngine;

namespace madMeesh.TurnBasedEngine {
    public class DeployPhase : Phase {
        public DeployPhase ( TurnController phaseOwner ) : base ( phaseOwner ) { }

        protected override void PhaseUpdateLoop ( ) {
            if ( IsPhaseComplete == false ) {
                return;
            }

            nextPhase = new EndTurnPhase ( currentOwner );
            OnPhaseCompleted ( );

            HasCompletedPhase = true;
            IsPhaseExecuting = false;
        }

        public override void SetNextPhase ( IPhase next ) {
            nextPhase = next;
        }
    }
}