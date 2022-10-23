namespace Murder.Core.Dialogs
{
    [Blackboard("Global")]
    public class GlobalBlackboard
    {
        public readonly bool HasUserCompletedGame = false;

        public readonly int PlayerDeaths = 0;
        
        public readonly int EnteredDungeonCount = 0;

        public readonly int Coins = 0;

        public readonly bool HasSpokenWithSalem = false;

        public readonly int PortalInteractedCount = 0;

        public readonly bool ActivedPortalInRun = false;

        public readonly int ReachedEndCount = 0;
    }
}
