﻿using Murder.Diagnostics;

namespace Murder.Core
{
    public class GameScene : Scene
    {
        private readonly Guid _worldGuid;

        private MonoWorld? _world;

        public Guid WorldGuid => _worldGuid;

        public override MonoWorld? World => _world;

        public GameScene(Guid guid)
        {
            _worldGuid = guid;
        }

        public override void LoadContentImpl()
        {
            _world = CreateWorld();
            GC.Collect(generation: 0, mode: GCCollectionMode.Forced, blocking: true);
        }

        public override void ReloadImpl()
        {
            _world = null;
        }

        public override void ResumeImpl()
        {
            _world?.ActivateAllSystems();
        }

        public override void SuspendImpl()
        {
            _world?.DeactivateAllSystems();
        }

        public override async Task UnloadAsyncImpl()
        {
            ValueTask<bool> result = Game.Data.PendingSave ?? new(true);
            await result;

            _world?.Dispose();
            _world = null;
        }

        /// <summary>
        /// Replace world and return the previous one, which should be disposed.
        /// </summary>
        public bool ReplaceWorld(MonoWorld world, bool disposeWorld)
        {
            MonoWorld? previousWorld = _world;

            _world = world;

            if (disposeWorld)
            {
                previousWorld?.Dispose();
            }

            return true;
        }

        private MonoWorld CreateWorld()
        {
            GameLogger.Verify(RenderContext is not null);

            return Game.Data.CreateWorldInstanceFromSave(_worldGuid, RenderContext.Camera);
        }
    }
}