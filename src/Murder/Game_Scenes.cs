using Murder.Diagnostics;

namespace Murder
{
    public partial class Game
    {
        protected Guid? _pendingWorldTransition = default;

        public bool QueueWorldTransition(Guid world)
        {
            if (_pendingWorldTransition.HasValue)
            {
                GameLogger.Verify(_pendingWorldTransition.Value == world, "Queue another world transition mid-update?");
                return false;
            }

            _pendingWorldTransition = world;
            return true;
        }

        protected void DoPendingWorldTransition()
        {
            if (!_pendingWorldTransition.HasValue)
            {
                return;
            }

            // TODO: Cross fade? Review this flag here!
            SoundPlayer.Stop(fadeOut: true);

            GameLogger.Verify(_sceneLoader is not null);

            _game?.OnSceneTransition();

            // Unpause on each world transition.
            Resume();

            _sceneLoader.SwitchScene(_pendingWorldTransition.Value);
            _pendingWorldTransition = default;

            // TODO: Fancier loading bar.
            LoadSceneAsync().Wait();
        }
    }
}
