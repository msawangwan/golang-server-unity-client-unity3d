namespace madMeesh.TurnBasedEngine {
    public interface IPhase {
        bool HasEnteredPhase { get; }
        bool IsPhaseComplete { get; }
        bool HasCompletedPhase { get; } // may be redundant

        void EnterPhase ( );
        void ExecutePhase ( );

        event System.Action<IPhase> RaisePhaseComplete;
    }
}