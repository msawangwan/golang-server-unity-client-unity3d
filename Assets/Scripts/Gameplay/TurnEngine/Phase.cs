using UnityEngine;
using System.Collections;

namespace madMeesh.TurnBasedEngine {
    public abstract class Phase : IPhase {
        public bool HasEnteredPhase { get; protected set; }
        public bool IsPhaseComplete { get; protected set; }
        public bool HasCompletedPhase { get; protected set; }

        protected IPhase nextPhase = null;

        protected bool IsPhaseExecuting { get; set; }

        public Phase() {
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

        public event System.Action<IPhase> RaisePhaseComplete;
        public abstract void SetNextPhase ( IPhase next );
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