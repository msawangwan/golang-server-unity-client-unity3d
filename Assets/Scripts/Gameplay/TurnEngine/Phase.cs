using System;

namespace madMeesh.TurnBasedEngine {
    public abstract class Phase : IPhase {
        public bool HasEnteredPhase { get; protected set; }
        public bool IsPhaseComplete { get; protected set; }
        public bool HasCompletedPhase { get; protected set; }

        protected TurnController currentOwner;
        protected IPhase nextPhase = null;

        protected bool IsPhaseExecuting { get; set; }

        public Phase ( TurnController phaseOwner ) {
            currentOwner = phaseOwner;

            HasEnteredPhase = false;
            IsPhaseComplete = false;
            HasCompletedPhase = false;
        }

        public void EnterPhase() {
            IsPhaseExecuting = true;
        }

        public void ExecutePhase() {
            if ( IsPhaseExecuting ) {
                PhaseUpdateLoop ( );
            }
        }

        public event Action<IPhase> RaisePhaseComplete;
        public abstract void SetNextPhase ( IPhase next );

        /* All derived Phases must implement an if statement that returns until the phase is complete. */
        protected abstract void PhaseUpdateLoop ( );

        protected void OnPhaseCompleted() {
            if ( nextPhase != null ) {
                if ( RaisePhaseComplete != null ) {
                    RaisePhaseComplete ( nextPhase );
                }
            }
        }
    }
}