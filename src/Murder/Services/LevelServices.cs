using Bang;

namespace Murder.Services
{
    public static class LevelServices
    {
        public static ValueTask SwitchScene(Guid nextWorldGuid)
        {
            // TODO: Do something fancier?
            Game.Instance.QueueWorldTransition(nextWorldGuid);

            return default;
        }
    }
}
