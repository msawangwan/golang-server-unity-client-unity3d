using System;

namespace madMeesh.TurnBasedEngine {
    public class TurnController {
        public Player OwningPlayer { get; set; }

        public bool HasStartedTurn { get; private set; }
        public bool HasCompletedTurn { get; private set; }

        public IPhase CurrentPhase { get; private set; }
        public IPhase NextPhase { get; private set; }

        public event Action RaiseTurnCompleted;

        public TurnController(Player owner) {
            OwningPlayer = owner;
        }

        public void StartTurn ( IPhase firstPhaseOfTurn ) {
            CurrentPhase = firstPhaseOfTurn;
            HasStartedTurn = true;
            HasCompletedTurn = false;
        }

        public void TakeTurn() {
            if ( CurrentPhase.HasEnteredPhase == false ) {
                CurrentPhase.RaisePhaseComplete += HandleOnPhaseCompleted;
                CurrentPhase.EnterPhase ( );
            }

            CurrentPhase.ExecutePhase ( );

            if ( CurrentPhase.HasCompletedPhase == false ) {
                return;
            }

            CurrentPhase = NextPhase;

            if ( NextPhase == null ) {
                OnTurnCompleted ( );
                return;
            }

            NextPhase = null;

            CurrentPhase.RaisePhaseComplete -= HandleOnPhaseCompleted;
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
            NextPhase = next;
        }
    }
}
