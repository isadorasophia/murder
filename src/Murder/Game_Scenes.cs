using Bang;
using Murder.Core;
using Murder.Core.Sounds;
using Murder.Diagnostics;

namespace Murder
{
    public partial class Game
    {
        protected Guid? _pendingWorldTransition = default;

        protected MonoWorld? _pendingWorld = default;

        protected bool _pendingResetSave = false;
        protected bool _disposePendingWorld = true;
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

        public bool QueueWorldTransition(Guid world, bool resetSave)
        {
            bool result = QueueWorldTransition(world);
            if (!result)
            {
                return false;
            }

            _pendingResetSave = resetSave;
            return true;
        }

        /// <summary>
        /// This is called when replacing the world for a current scene.
        /// Happened when transition from two different scenes (already loaded) as a world.
        /// </summary>
        public bool QueueReplaceWorldOnCurrentScene(MonoWorld world, bool disposeWorld)
        {
            if (_pendingWorldTransition.HasValue)
            {
                GameLogger.Fail("Queue another world transition mid-update?");
                return false;
            }

            _pendingWorld = world;
            _disposePendingWorld = disposeWorld;

            return true;
        }

        protected void DoPendingWorldTransition()
        {
            if (_pendingWorld is not null)
            {
                _sceneLoader?.ReplaceWorldOnCurrentScene(_pendingWorld, _disposePendingWorld);

                _pendingWorld = null;
                _disposePendingWorld = true;

                return;
            }

            if (!_pendingWorldTransition.HasValue)
            {
                return;
            }

            if (_pendingResetSave)
            {
                // Unload any pending save so we don't "leak".
                Data.ResetActiveSave();
            }

            // TODO: Cross fade? Review this flag here!
            SoundPlayer.Stop(SoundLayer.Sfx, fadeOut: true);

            GameLogger.Verify(_sceneLoader is not null);

            _game?.OnSceneTransition();

            // Unpause on each world transition.
            Resume();

            _sceneLoader.SwitchScene(_pendingWorldTransition.Value);
            _pendingWorldTransition = default;
            _pendingResetSave = false;

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