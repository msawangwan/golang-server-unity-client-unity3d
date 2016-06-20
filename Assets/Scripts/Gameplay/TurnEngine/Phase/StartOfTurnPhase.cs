using UnityEngine;

namespace madMeesh.TurnBasedEngine {
    public class StartOfTurnPhase : Phase {
        public StartOfTurnPhase( TurnController phaseOwner ) : base( phaseOwner ) {
            Debug.Log ( "Start of turn" );
        }

        protected override void PhaseUpdateLoop ( ) {
            if ( IsPhaseComplete == false ) {
                /*
                    Implement actions to complete 
                    before setting IsPhaseCompelte to true.
                */
                IsPhaseComplete = true;
                return;
            }

            nextPhase = new DrawPhase ( currentOwner );
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
