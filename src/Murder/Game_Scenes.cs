using Bang;
using Murder.Core;
using Murder.Diagnostics;

namespace Murder
{
    public partial class Game
    {
        protected Guid? _pendingWorldTransition = default;

        protected MonoWorld? _pendingWorld = default;

        protected bool _pendingExit = false;

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

        /// <summary>
        /// This is called when replacing the world for a current scene.
        /// Happened when transition from two different scenes (already loaded) as a world.
        /// </summary>
        public bool QueueReplaceWorldOnCurrentScene(MonoWorld world)
        {
            if (_pendingWorldTransition.HasValue)
            {
                GameLogger.Fail("Queue another world transition mid-update?");
                return false;
            }

            _pendingWorld = world;
            return true;
        }

        protected void DoPendingWorldTransition()
        {
            if (_pendingWorld is not null)
            {
                _sceneLoader?.ReplaceWorldOnCurrentScene(_pendingWorld);

                _pendingWorld = null;
                return;
            }

            if (!_pendingWorldTransition.HasValue)
            {
                return;
            }

            // TODO: Cross fade? Review this flag here!
            // SoundPlayer.Stop(fadeOut: true);

            GameLogger.Verify(_sceneLoader is not null);

            _game?.OnSceneTransition();

            // Unpause on each world transition.
            Resume();

            _sceneLoader.SwitchScene(_pendingWorldTransition.Value);
            _pendingWorldTransition = default;

            // TODO: Fancier loading bar.
            LoadSceneAsync(waitForAllContent: true).Wait();
        }

        /// <summary>
        /// This queues such that the game exit at the end of the update.
        /// We wait until the end of the update to avoid any access to a world that has been disposed on cleanup.
        /// </summary>
        public void QueueExitGame()
        {
            _pendingExit = true;
        }

        protected void DoPendingExitGame()
        {
            if (!_pendingExit)
            {
                return;
            }

            ExitGame();

            _pendingExit = false;
        }
    }
}
