﻿using Microsoft.Xna.Framework;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Core
{
    public class SceneLoader
    {
        protected readonly GraphicsDeviceManager _graphics;
        protected readonly GameProfile _settings;

        /// <summary>
        /// Whether it will load a scene and a load context that has advanced diagnostic
        /// features enabled (which also implies in more processing overhead).
        /// </summary>
        protected readonly bool IsDiagnosticEnabled;

        private Scene? _activeScene;

        public Scene ActiveScene => _activeScene!;

        private readonly Dictionary<Guid, GameScene> _gameScenes = new();

        private readonly Dictionary<Type, Scene> _genericScenes = new();

        public SceneLoader(GraphicsDeviceManager graphics, GameProfile settings, Scene scene, bool isDiagnosticEnabled)
        {
            _graphics = graphics;
            _settings = settings;

            IsDiagnosticEnabled = isDiagnosticEnabled;

            SetScene(scene);
        }

        public bool ReplaceWorldOnCurrentScene(MonoWorld world, bool disposeWorld)
        {
            if (_activeScene is not GameScene gameScene)
            {
                return false;
            }

            return gameScene.ReplaceWorld(world, disposeWorld);
        }

        public void SwitchScene(Guid worldGuid)
        {
            if (_activeScene is GameScene gameScene &&
                gameScene.WorldGuid == worldGuid)
            {
                // Reload the active scene.
                _activeScene.Reload();
                return;
            }

            if (_gameScenes.TryGetValue(worldGuid, out GameScene? scene))
            {
                // Scene was already loaded, just change the active scene.
                scene.Reload();

                SetScene(scene);
                return;
            }

            CacheAndSetScene(new GameScene(worldGuid));
        }

        /// <summary>
        /// Switch to a scene of type <typeparamref name="T"/>.
        /// </summary>
        public void SwitchScene<T>() where T : Scene, new()
        {
            if (_genericScenes.TryGetValue(typeof(T), out Scene? scene))
            {
                // Scene was already loaded, just change the active scene.
                scene.Reload();

                SetScene(scene);
                return;
            }

            CacheAndSetScene(new T());
        }

        /// <summary>
        /// Switch to <paramref name="scene"/>.
        /// </summary>
        public void SwitchScene(Scene scene) => SetScene(scene);

        /// <summary>
        /// Initialize current active scene.
        /// </summary>
        public void Initialize()
        {
            if (_activeScene is null)
            {
                return;
            }

            RenderContextFlags flags = RenderContextFlags.CustomShaders;
            if (IsDiagnosticEnabled)
            {
                flags |= RenderContextFlags.Debug;
            }

            _activeScene.Initialize(_graphics.GraphicsDevice, _settings, flags);
        }

        /// <summary>
        /// Load the content of the current active scene.
        /// </summary>
        public void LoadContent()
        {
            if (_activeScene is null)
            {
                return;
            }

            using PerfTimeRecorder recorder = new("Loading Scene Content");
            _activeScene.LoadContent();
        }

        private void CacheAndSetScene(Scene scene)
        {
            if (scene is GameScene gameScene)
            {
                _gameScenes.Add(gameScene.WorldGuid, gameScene);
            }
            else
            {
                _genericScenes.Add(scene.GetType(), scene);
            }

            SetScene(scene);
        }

        [MemberNotNull(nameof(_activeScene))]
        private void SetScene(Scene scene)
        {
            _activeScene?.Unload();
            _activeScene = scene;

            Initialize();
        }
    }
}