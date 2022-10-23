using Murder.Diagnostics;

namespace Murder
{
    public partial class Game
    {
        private Guid? _pendingWorldTransition = default;

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

        private void DoPendingWorldTransition()
        {
            if (!_pendingWorldTransition.HasValue)
            {
                return;
            }

            GameLogger.Verify(_sceneLoader is not null);

            // Unpause on each world transition.
            Resume();

            _sceneLoader.SwitchScene(_pendingWorldTransition.Value);
            _pendingWorldTransition = default;

            // TODO: Fancier loading bar.
            LoadSceneAsync().Wait();
        }
    }
}
