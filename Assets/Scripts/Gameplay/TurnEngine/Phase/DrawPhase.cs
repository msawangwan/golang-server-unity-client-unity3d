using UnityEngine;
using madMeesh.Cards;

namespace madMeesh.TurnBasedEngine {
    public class DrawPhase : Phase {
        public DrawPhase( TurnController phaseOwner ) : base( phaseOwner ) {
            Debug.Log ( "Draw Phase" );
            phaseOwner.OwningPlayer.PlayerDeck.RaiseCardDrawn += HandleOnCardDrawn;
        }

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

        private void HandleOnCardDrawn ( CardAction drawn ) {
            IsPhaseComplete = true;
        }
    }
}
