using Bang;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Core.Sounds;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Save;
using System.Numerics;
using Murder.Utilities;
using System.Diagnostics;

namespace Murder
{
    public partial class Game : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Use this to set whether diagnostics should be pulled.
        /// </summary>
        public static bool DIAGNOSTICS_MODE = true;

        /* *** Static properties of the Game *** */

        /// <summary>
        /// Singleton instance of the game. wBe cautious when referencing this...
        /// </summary>
        public static Game Instance { get; private set; } = null!;

        /// <summary>
        /// Gets the current instance of the GraphicsDevice.
        /// </summary>
        public static new GraphicsDevice GraphicsDevice => Instance._graphics.GraphicsDevice;

        /// <summary>
        /// Gets the GameDataManager instance.
        /// </summary>
        public static GameDataManager Data => Instance._gameData;

        /// <summary>
        /// Gets the active SaveData asset instance.
        /// </summary>
        public static SaveData Save => Instance._gameData.ActiveSaveData;

        /// <summary>
        /// Gets the GamePreferences asset instance.
        /// </summary>
        public static GamePreferences Preferences => Instance._gameData.Preferences;

        /// <summary>
        /// Gets the GameProfile asset instance.
        /// </summary>
        public static GameProfile Profile => Instance._gameData.GameProfile;

        /// <summary>
        /// Gets the ISoundPlayer instance.
        /// </summary>
        public static ISoundPlayer Sound => Instance.SoundPlayer;

        /// <summary>
        /// Provides a static Random instance.
        /// </summary>
        public static Random Random = new();

        /// <summary>
        /// Gets the game width from the GameProfile. This is the intended size, not the actual size. For the current window size use <see cref="RenderContext.Camera"/>.
        /// </summary>
        public static int Width => Profile.GameWidth;

        /// <summary>
        /// Gets the game height from the GameProfile. This is the intended size, not the actual size. For the current window size use <see cref="RenderContext.Camera"/>.
        /// </summary>
        public static int Height => Profile.GameHeight;

        /// <summary>
        /// Gets the PlayerInput instance.
        /// </summary>
        public static PlayerInput Input => Instance._playerInput;

        /// <summary>
        /// The time difference between current and last update, scaled by pause and other time scaling. Value is reliable only during the Update().
        /// This value will be zero during render.
        /// </summary>
        public static float DeltaTime => (float)(Instance._scaledDeltaTime);
        /// <summary>
        /// De time difference between current and last update. Value is reliable only during the Update().
        /// </summary>
        public static float UnscaledDeltaTime => (float)Instance._unscaledDeltaTime;
        /// <summary>
        /// Frame clock, updated once per frame
        /// </summary>
        public static float Now => (float)Instance._scaledElapsedTime;

        /// <summary>
        /// Gets the scaled elapsed time from the previous fixed update.
        /// </summary>
        public static float PreviousNow => (float)Instance._scaledPreviousElapsedTime;

        /// <summary>
        /// Gets the current unscaled elapsed time.
        /// </summary>
        public static float NowUnscaled => (float)Instance._unscaledElapsedTime;

        /// <summary>
        /// Time from previous fixed update.
        /// </summary>
        public static float PreviousNowUnscaled => (float)Instance._unscaledPreviousElapsedTime;

        /// <summary>
        /// Gets the fixed delta time in seconds.
        /// </summary>
        public static float FixedDeltaTime => Instance._fixedUpdateDelta;

        public static bool IsRunningSlowly { get; private set; } = false;
        public static int MaxLostFrames { get; set; } = 0;
        public static float MaxDeltaTime { get; set; } = 0;

        /// <summary>
        /// Whether the game support saving game progress.
        /// </summary>
        public static bool CanSave => Instance._game?.CanSave ?? false;

        private readonly Stopwatch _lastFrameStopwatch = new();

        private readonly Stopwatch _updateStopwatch = new();
        private readonly Stopwatch _renderStopWatch = new();
        private readonly Stopwatch _imGuiStopWatch = new();
        private readonly Stopwatch _soundStopWatch = new();

        /// <summary>
        /// Beautiful hardcoded grid so it's very easy to access in game!
        /// </summary>
        public static GridConfiguration Grid => Instance._grid;

        /// <summary>
        /// Only updated if <see cref="DIAGNOSTICS_MODE"/> is set.
        /// </summary>
        public static UpdateTimeTracker TimeTrackerDiagnostics = new();

        /// <summary>
        /// Only updated if <see cref="DIAGNOSTICS_MODE"/> is set.
        /// </summary>
        public static UpdateTimeTracker RenderTimeTrackerDiagnostics = new();

        /* *** Protected helpers *** */

        protected readonly Microsoft.Xna.Framework.GraphicsDeviceManager _graphics;
        public Microsoft.Xna.Framework.GraphicsDeviceManager GraphicsDeviceManager => _graphics;

        protected readonly PlayerInput _playerInput;

        protected readonly GameDataManager _gameData;

        public readonly ISoundPlayer SoundPlayer;

        public readonly HapticsManager Haptics;

        /// <summary>
        /// Initialized in <see cref="LoadContent"/>.
        /// </summary>
        protected SceneLoader? _sceneLoader;

        protected virtual Scene InitialScene => new GameScene(Profile.StartingScene);

        protected virtual bool IsDiagnosticEnabled => false;

        /* *** Public instance fields *** */

        public Scene? ActiveScene => _sceneLoader?.ActiveScene;

        public const float LONGEST_TIME_RESET = 5f;

        /// <summary>
        /// Time in seconds that the Update() methods took to finish.
        /// This is not the time between updates (that's <see cref="DeltaTime"/>), instead it's the time
        /// That the systems take to actually update the game logic, including all the systems and components and render systems.
        /// </summary>
        public float UpdateTime { get; private set; }
        public float LongestUpdateTime { get; private set; }
        private float _longestUpdateTimeAt;

        /// <summary>
        /// Time in seconds that the Draw() method took to finish.
        /// This is not the time that the render systems take, instead it's the time
        /// that the game takes to actually render the scene, including all the draw calls.
        /// </summary>
        public float RenderTime { get; private set; }

        /// <summary>
        /// Time in seconds that the ImGui rendering took to finish.
        /// </summary>
        public float ImGuiRenderTime { get; private set; }
        public float SoundUpdateTime { get; private set; }

        /// <summary>
        /// Gets the longest render time ever recorded.
        /// </summary>
        public float LongestRenderTime { get; private set; }

        private float _longestRenderTimeAt;

        /// <summary>
        /// Elapsed time in seconds from the previous update frame since the game started
        /// </summary>
        public float PreviousElapsedTime => (float)_scaledPreviousElapsedTime;

        public float TimeScale = 1f;

        public bool IsPaused { get; private set; }

        private GridConfiguration _grid = new(cellSize: 24 /* default size, just in case, who knows */);

        /// <summary>
        /// If set, this is the amount of frames we will skip while rendering.
        /// </summary>
        private int _freezeFrameCount = 0;

        /// <summary>
        /// Time since we have been freezing the frames.
        /// </summary>
        private double _freezeFrameTime = 0;

        /// <summary>
        /// Whether there is a preload game active.
        /// </summary>
        private IPreloadGame? _preload = null;

        /// <summary>
        /// Whether the player is currently skipping frames (due to cutscene) and ignore
        /// the time while calling update methods.
        /// </summary>
        private bool _isSkippingDeltaTimeOnUpdate = false;

        /// <summary>
        /// Whether the player is currently skipping frames (due to cutscene) and ignore
        /// the time while calling update methods.
        /// </summary>
        public bool IsSkippingDeltaTimeOnUpdate => _isSkippingDeltaTimeOnUpdate;

        /// <summary>
        /// Whether we are waiting for a save complete operation: do not do any update logic.
        /// </summary>
        private bool _waitForSaveComplete = false;

        /// <summary>
        /// Whether the player started skipping.
        /// </summary>
        public bool StartedSkippingCutscene = false;

        /// <summary>
        /// Whether it initialized loading textures after the content was loaded.
        /// </summary>
        private bool _initialiazedAfterContentLoaded = false;

        private Point _windowedSize = Point.Zero;
        private bool _windowSettingsDirty = true;

        /// <summary>
        /// Gets or sets the fullscreen mode of the game.
        /// When set, it updates the game's window to reflect the new mode.
        /// </summary>
        public bool Fullscreen
        {
            get => Preferences.FullScreen;
            set
            {
                Preferences.SetFullScreen(value);
                OnWindowChanged();
            }
        }

        public void OnWindowChanged()
        {
            _windowSettingsDirty = true;
        }

        /// <summary>
        /// Gets the scale of the game relative to the window size and game profile scale.
        /// Returns Vector2.One if the window has invalid dimensions.
        /// </summary>
        public Vector2 GameScale
        {
            get
            {
                if (Window.ClientBounds.Width <= 0 || Window.ClientBounds.Height <= 0)
                    return Vector2.One;

                return new(
                    ((float)_screenSize.X / Window.ClientBounds.Width) / Profile.GameScale,
                    ((float)_screenSize.Y / Window.ClientBounds.Height) / Profile.GameScale);
            }
        }

        protected virtual bool HasCursor => false;

        private Point _screenSize;

        // TODO: Make this private or within a setter or whatever.
        // We need to refresh window when it does get set...
        public float Downsample = 1;

        /* *** Private instance fields *** */

        // Update properties.
        private float _fixedUpdateDelta;

        private double _scaledElapsedTime = 0;
        private double _unscaledElapsedTime = 0;
        private double _scaledDeltaTime = 0;
        private double _unscaledDeltaTime = 0;

        private double _lastFixedUpdateTime = 0;

        private double _scaledPreviousElapsedTime = 0;
        private double _unscaledPreviousElapsedTime = 0;

        /// <summary>
        /// This is the underlying implementation of the game. This listens to the murder game events.
        /// </summary>
        protected readonly IMurderGame? _game;

        public IMurderGame? MurderGame => _game;

        /// <summary>
        /// Single logger of the game.
        /// </summary>
        protected GameLogger _logger;

        protected virtual IPreloadGame? TryCreatePreloadScreen() => _game?.TryCreatePreload();

        /// <summary>
        /// Gets the current graph logger debugger.
        /// </summary>
        public virtual GraphLogger GraphLogger { get; } = new GraphLogger();

        /// <summary>
        /// Creates a RenderContext using the specified graphics device, camera, and settings.
        /// Returns a new RenderContext if the game instance is null.
        /// Optionally implement this interface for using your custom RenderContext.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device to use for rendering.</param>
        /// <param name="camera">The camera to be used in the rendering context.</param>
        /// <param name="settings">The rendering settings flags.</param>
        /// <returns>A new or existing RenderContext instance.</returns>
        public RenderContext CreateRenderContext(GraphicsDevice graphicsDevice, Camera2D camera, RenderContextFlags settings) =>
            _game?.CreateRenderContext(graphicsDevice, camera, settings) ?? new RenderContext(graphicsDevice, camera, settings);

        public Game(IMurderGame? game = null) : this(game, new GameDataManager(game)) { }

        /// <summary>
        /// Creates a new game, there should only be one game instance ever.
        /// If <paramref name="dataManager"/> is not initialized, it will create the starting scene from <see cref="GameProfile"/>.
        /// </summary>
        public Game(IMurderGame? game, GameDataManager dataManager)
        {
            Instance = this;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s, e) =>
            {
                if (s is not Microsoft.Xna.Framework.GameWindow window)
                {
                    GameLogger.Error($"How do we have an invalid window of {s?.GetType()}?");
                    return;
                }

                // propagate to the scene
                ActiveScene?.OnClientWindowChanged(new(window.ClientBounds.Width, window.ClientBounds.Height));
            };

            IsMouseVisible = HasCursor || (game?.HasCursor ?? false);

            _logger = GameLogger.GetOrCreateInstance();
            _logger.Initialize(IsDiagnosticEnabled);

            _playerInput = new PlayerInput();
            SoundPlayer = game?.CreateSoundPlayer() ?? new SoundPlayer();

            Haptics = new HapticsManager();

            _game = game;
            _gameData = dataManager;

            _graphics = new(this);
            _preload = TryCreatePreloadScreen();

            Content.RootDirectory = _gameData.BinResourcesDirectoryPath;
        }

        /// <summary>
        /// Initializes the game by setting up input bindings and configuring initial settings.
        /// Typically overridden by the game implementation.
        /// </summary>
        /// <remarks>
        /// Registers various input buttons for both editor and navigation controls using the MurderInputButtons enumeration.
        /// Configures gamepad axes for UI navigation. Also initializes game assets and refreshes the window size.
        /// Calls the base Initialize method for content loading and initializes the game instance if available.
        /// </remarks>
        protected override void Initialize()
        {
            World.InitializeLookupComponents(_game?.ComponentsLookup ?? new MurderComponentsLookup());

            // Subscribe events
            AppDomain.CurrentDomain.ProcessExit += OnClose;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // Editor input
            _playerInput.RegisterButton(MurderInputButtons.Debug, Keys.F1);
            _playerInput.RegisterButton(MurderInputButtons.PlayGame, Keys.F5);

            _playerInput.Register(MurderInputButtons.LeftClick, MouseButtons.Left);
            _playerInput.Register(MurderInputButtons.RightClick, MouseButtons.Right);
            _playerInput.Register(MurderInputButtons.MiddleClick, MouseButtons.Middle);

            _playerInput.RegisterButton(MurderInputButtons.Shift, Keys.LeftShift);
            _playerInput.RegisterButton(MurderInputButtons.Esc, Keys.Escape);
            _playerInput.RegisterButton(MurderInputButtons.Delete, Keys.Delete);
            _playerInput.RegisterButton(MurderInputButtons.Ctrl, Keys.LeftControl, Keys.RightControl);
            _playerInput.RegisterButton(MurderInputButtons.Space, Keys.Space);
            _playerInput.RegisterButton(MurderInputButtons.Backspace, Keys.Back, Keys.BrowserBack);

            // Navigation input
            _playerInput.RegisterAxes(MurderInputAxis.Ui,
                GamepadAxis.LeftThumb, GamepadAxis.RightThumb, GamepadAxis.Dpad);

            base.Initialize(); // Content is loaded here

            _game?.Initialize();

            // Propagate dianostics mode settings.
            World.DIAGNOSTICS_MODE = DIAGNOSTICS_MODE;

            RefreshWindow();
        }

        /// <summary>
        /// Refreshes the game window settings based on the current profile.
        /// </summary>
        /// <remarks>
        /// Refreshes the active scene with new graphics settings, if present.
        /// </remarks>
        protected virtual void RefreshWindow()
        {
            SetTargetFps(Profile.TargetFps);

            _screenSize = new Point(Width, Height) * Data.GameProfile.GameScale;

            if (Fullscreen)
            {
                SetWindowSize(new Point(Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width, Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height), false);
            }
            else
            {
                SetWindowSize(_screenSize, false);
            }

            _screenSize = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            if (!Fullscreen)
            {
                // This seems to be a bug in Monogame
                // This line must be repeated otherwise the window won't be
                // borderless.
                Window.IsBorderlessEXT = false;
            }

            ActiveScene?.OnClientWindowChanged(_screenSize);
            _windowSettingsDirty = false;
        }

        /// <summary>
        /// Sets the window size for the game based on the specified screen size and full screen settings.
        /// </summary>
        /// <param name="screenSize">The desired screen size in pixels.</param>
        /// <param name="remember">Whether we should persist this window size.</param>
        /// <remarks>
        /// In windowed mode, uses either the saved window size or the provided screen size.
        /// Synchronizes with vertical retrace in debug mode.
        /// </remarks>
        public virtual void SetWindowSize(Point screenSize, bool remember)
        {
            // _graphics.SynchronizeWithVerticalRetrace = true;
            _windowSettingsDirty = false;

            if (Fullscreen)
            {
                _windowedSize = _graphics.GraphicsDevice.Viewport.Bounds.Size();

                Window.IsBorderlessEXT = true;
                _graphics.IsFullScreen = true;

                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                _graphics.IsFullScreen = false;
                Window.IsBorderlessEXT = false;

                if (remember && _windowedSize.X > 0 && _windowedSize.Y > 0)
                {
                    _graphics.PreferredBackBufferWidth = (int)(_windowedSize.X);
                    _graphics.PreferredBackBufferHeight = (int)(_windowedSize.Y);
                }
                else
                {
                    _graphics.PreferredBackBufferWidth = screenSize.X;
                    _graphics.PreferredBackBufferHeight = screenSize.Y;
                }
            }

            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Loads game content and initializes it. This includes initializing the sound player, game data, settings, shaders, and initial scene. Also asynchronously loads the initial scene.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize our actual sound player!
            SoundPlayer.Initialize(_gameData.BinResourcesDirectoryPath);

            _gameData.Initialize();

            ApplyGameSettings();

            LoadContentImpl();

            _gameData.LoadShaders(breakOnFail: false);

            // Load assets, textures, content, etc
            _gameData.LoadContent();

            // Initialize the initial scene.
            _sceneLoader = new SceneLoader(_graphics, Profile, InitialScene, IsDiagnosticEnabled);

            _ = LoadSceneAsync(waitForAllContent: true);
        }

        /// <summary>
        /// Virtual method for extended content loading implementation in derived classes.
        /// </summary>
        protected virtual void LoadContentImpl() { }

        /// /// <summary>
        /// Applies game settings based on the current <see cref="GameProfile"/>, loaded with <see cref="_gameData"/>. Configures grid and rendering settings, and calls an implementation-specific settings application method.
        /// </summary>
        protected void ApplyGameSettings()
        {
            _grid = new GridConfiguration(Profile.DefaultGridCellSize);

            // This will keep the camera and other render positions in sync with the fixed update.
            _graphics.SynchronizeWithVerticalRetrace = true; // vsync handles frame pacing
            IsFixedTimeStep = false; // call Update/Draw as fast as vsync allows.

            ApplyGameSettingsImpl();
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Virtual method for extended game settings application in derived classes.
        /// </summary>
        protected virtual void ApplyGameSettingsImpl() { }

        /// <summary>
        /// Asynchronously loads the game's content.
        /// </summary>
        /// <param name="waitForAllContent">Indicates whether to wait for all game content to be loaded before proceeding.</param>
        protected virtual async Task LoadSceneAsync(bool waitForAllContent)
        {
            try
            {
                GameLogger.Verify(_sceneLoader is not null);

                if (waitForAllContent && _gameData.LoadContentProgress is not null)
                {
                    await _gameData.LoadContentProgress;
                }

                if (_game is not null)
                {
                    await _game.LoadContentAsync();
                    _game.OnSceneTransition();
                }

                _sceneLoader.LoadContent();
            }
            catch (Exception ex)
            {
                GameLogger.SpewException(ex);
                GameLogger.CaptureCrash();

                ExitGame();
            }
        }

        /// <summary>
        /// This will pause the game.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// This will pause the game for <paramref name="amount"/> of frames.
        /// </summary>
        public void FreezeFrames(int amount)
        {
            _freezeFrameCount = amount;
        }

        /// <summary>
        /// This will skip update times and immediately run the update calls from the game 
        /// until <see cref="ResumeDeltaTimeOnUpdate"/> is called.
        /// </summary>
        public void SkipDeltaTimeOnUpdate()
        {
            _isSkippingDeltaTimeOnUpdate = true;

            // reset this state.
            StartedSkippingCutscene = false;
        }

        /// <summary>
        /// Resume game to normal game time.
        /// </summary>
        public bool ResumeDeltaTimeOnUpdate()
        {
            bool wasSkipping = _isSkippingDeltaTimeOnUpdate;
            _isSkippingDeltaTimeOnUpdate = false;

            return wasSkipping;
        }

        /// <summary>
        /// Sets the flag to indicate that the game should wait for the save operation to complete.
        /// </summary>
        public void SetWaitForSaveComplete()
        {
            _waitForSaveComplete = true;
        }

        /// <summary>
        /// Determines if the game can resume after a save operation is complete. Returns true if there's no active save data or the save operation has finished.
        /// </summary>
        /// <returns>True if the game can resume, false otherwise.</returns>
        public bool CanResumeAfterSaveComplete()
        {
            _waitForSaveComplete = Data.WaitPendingSaveTrackerOperation;
            return !_waitForSaveComplete;
        }

        /// <summary>
        /// This will resume the game.
        /// </summary>
        public void Resume()
        {
            IsPaused = false;
        }

        /// <summary>
        /// Performs game frame updates, handling logic for paused states, fixed updates, and unscaled time.
        /// </summary>
        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            float rawDeltaTime = (float)_lastFrameStopwatch.Elapsed.TotalSeconds;
            _lastFrameStopwatch.Restart();

            if (_waitForSaveComplete && !CanResumeAfterSaveComplete())
            {
                UpdateUnscaledDeltaTime(gameTime.ElapsedGameTime.TotalSeconds); // Using XNA GameTime, because this is called before the actual update logic

                // Don't do any logic operation yet, we are waiting for the save to complete.
                return;
            }

            if (ActiveScene is not null && ActiveScene.Loaded)
            {
                if (_preload is not null && _preload.WrapItUp())
                {
                    _preload = null;
                }

                if (_preload is null && !_initialiazedAfterContentLoaded)
                {
                    _gameData.AfterContentLoadedFromMainThread();
                    _initialiazedAfterContentLoaded = true;
                }
            }

            _preload?.Update();

            if (DIAGNOSTICS_MODE)
            {
                _updateStopwatch.Start();
            }

            UpdateImpl(rawDeltaTime);

            while (_isSkippingDeltaTimeOnUpdate)
            {
                UpdateImpl(FixedDeltaTime);
                ActiveScene?.OnBeforeDraw();
            }

            if (DIAGNOSTICS_MODE)
            {
                _soundStopWatch.Start();
            }

            // Update sound logic!
            SoundPlayer.Update();

            if (DIAGNOSTICS_MODE)
            {
                SoundUpdateTime = (float)(_soundStopWatch.Elapsed.TotalSeconds);
                _soundStopWatch.Stop();
                _soundStopWatch.Reset();
            }

            // Update haptics
            Haptics.Update();
        }

        /// <summary>
        /// Implements core update logic, including frame freezing, world transitions, input handling, and time scaling.
        /// </summary>
        protected void UpdateImpl(float deltaTime)
        {
            GameLogger.Verify(ActiveScene is not null);
            IsRunningSlowly = deltaTime > _fixedUpdateDelta * 1.5f;

            if (_freezeFrameCount > 0)
            {
                _freezeFrameTime += deltaTime;
                if (_freezeFrameTime >= _fixedUpdateDelta)
                {
                    _freezeFrameCount--;
                    _freezeFrameTime = 0;
                }
                return;
            }

            _unscaledPreviousElapsedTime = _unscaledElapsedTime;
            _scaledPreviousElapsedTime = _scaledElapsedTime;

            double scaledDeltaTime = IsPaused ? 0 : deltaTime * TimeScale;

            // Advance clocks
            _unscaledElapsedTime += deltaTime;
            _unscaledDeltaTime = deltaTime;
            _scaledElapsedTime += scaledDeltaTime;
            _scaledDeltaTime = scaledDeltaTime;

            // Fixed update
            double timeSinceLastFixedUpdate = _scaledElapsedTime - _lastFixedUpdateTime;
            int fixedUpdatesRequired = (int)Math.Floor(timeSinceLastFixedUpdate / _fixedUpdateDelta);
            MaxLostFrames = Math.Max(MaxLostFrames, fixedUpdatesRequired);
            MaxDeltaTime = Math.Max(MaxDeltaTime, deltaTime);
            fixedUpdatesRequired = Math.Clamp(fixedUpdatesRequired, 0, 3);

            DoPendingExitGame();
            DoPendingWorldTransition();

            _playerInput.Update();  // poll once per frame

            for (int i = 0; i < fixedUpdatesRequired; i++)
            {
                _lastFixedUpdateTime += _fixedUpdateDelta;

                if (_preload is null)
                {
                    //SimulateRandomStalls();
                    ActiveScene.FixedUpdate();
                }
            }

            if (_scaledElapsedTime - _lastFixedUpdateTime > _fixedUpdateDelta * 1.5f)
            {
                // This is too much debt, we need to catch up.
                _lastFixedUpdateTime = _scaledElapsedTime;
            }


            // --- Update (once per frame) ---
            if (_preload is null)
            {
                //SimulateRandomStalls();
                ActiveScene.Update();
            }

            _game?.OnUpdate();
        }

        /// <summary>
        /// Call this to simulate random frame spikes for testing.
        /// </summary>
        [Conditional("DEBUG")]
        private void SimulateRandomStalls()
        {
            // ~2% chance of a stall each frame
            if (Random.NextDouble() < 0.02)
            {
                // Random stall between 1-4 fixed updates worth of time
                int ms = (int)(_fixedUpdateDelta * 1000 * (1 + Random.NextDouble() * 3));
                Thread.Sleep(ms);
            }
        }

        /// <summary>
        /// Renders the current frame, handling loading draw and ImGui rendering, and tracks rendering time.
        /// </summary>
        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GameLogger.Verify(ActiveScene is not null);

            if (_windowSettingsDirty)
            {
                RefreshWindow();
            }

            bool loading = _preload is not null || !ActiveScene.Loaded;
            if (loading && ActiveScene.RenderContext is RenderContext renderContext)
            {
                OnLoadingDraw(renderContext);
            }
            //SimulateRandomStalls();
            DrawScene();

            base.Draw(gameTime); // Monogame/XNA internal Draw

            if (DIAGNOSTICS_MODE)
            {
                RenderTime = (float)(_renderStopWatch.Elapsed.TotalSeconds);
                _renderStopWatch.Stop();
                _renderStopWatch.Reset();
                RenderTimeTrackerDiagnostics.Update(RenderTime);

                _imGuiStopWatch.Start();
            }

            DrawImGui(gameTime); // <== Draw ImGui content

            if (DIAGNOSTICS_MODE)
            {
                ImGuiRenderTime = (float)(_imGuiStopWatch.Elapsed.TotalSeconds);
                _imGuiStopWatch.Stop();
                _imGuiStopWatch.Reset();

                if (Now > _longestRenderTimeAt + LONGEST_TIME_RESET)
                {
                    _longestRenderTimeAt = Now;
                    LongestRenderTime = 0.0f;
                }

                if (RenderTime > LongestRenderTime)
                {
                    _longestRenderTimeAt = Now;
                    LongestRenderTime = UpdateTime;
                }
            }

            _game?.AfterDraw();
        }
        /// <summary>
        /// Display drawing for the load animation.
        /// </summary>
        protected virtual void OnLoadingDraw(RenderContext render)
        {
            if (_preload is not null)
            {
                _preload.Draw(render);
            }
            else
            {
                GraphicsDevice.SetRenderTarget(null);
            }
        }

        private void DrawScene()
        {
            if (_preload is not null)
            {
                return;
            }

            GameLogger.Verify(ActiveScene is not null);

            bool drawStarted = ActiveScene.DrawStart(); // <==== Start RenderContext draw call

            if (DIAGNOSTICS_MODE)
            {
                UpdateTime = (float)(_updateStopwatch.Elapsed.TotalSeconds);
                _updateStopwatch.Stop();
                _updateStopwatch.Reset();
                TimeTrackerDiagnostics.Update(UpdateTime);

                if (Now > _longestUpdateTimeAt + LONGEST_TIME_RESET)
                {
                    _longestUpdateTimeAt = Now;
                    LongestUpdateTime = 0.0f;
                }

                if (UpdateTime > LongestUpdateTime)
                {
                    _longestUpdateTimeAt = Now;
                    LongestUpdateTime = UpdateTime;
                }

                _renderStopWatch.Start();
            }

            if (drawStarted)
            {
                ActiveScene.DrawEnd(); // <==== End RenderContext draw call
            }
        }

        /// <summary>
        /// Placeholder for extending the ImGui drawing functionality in game editor.
        /// </summary>
        protected virtual void DrawImGui(Microsoft.Xna.Framework.GameTime gameTime) { }

        /// <summary>
        /// Placeholder for setting up a custom ImGui theme, to be extended in game editor.
        /// </summary>
        public virtual void BeginImGuiTheme() { }

        /// <summary>
        /// Placeholder for finalizing a custom ImGui theme, to be extended in game editor.
        /// </summary>
        public virtual void EndImGuiTheme() { }

        protected override void OnExiting(object sender, EventArgs args)
        {
            GameLogger.Log("Wrapping up, bye!");

            // TODO: Save config of GameSettings.
            // Data.SaveConfig();
        }

        protected override void Dispose(bool isDisposing)
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();

            ActiveScene?.Dispose();
            Data?.Dispose();

            base.Dispose(isDisposing);
        }

        private void UpdateUnscaledDeltaTime(double deltaTime)
        {
            _unscaledElapsedTime += deltaTime;
        }

        private void SetTargetFps(int fps)
        {
            _fixedUpdateDelta = 1f / fps;
        }

        /// <summary>
        /// Exit the game. This is used to wrap any custom behavior depending on the game implementation.
        /// </summary>
        protected virtual void ExitGame()
        {
            base.Exit();
        }

        protected virtual void OnUnhandledException(object? sender, EventArgs e)
        {
            if (e is UnhandledExceptionEventArgs unhandled && unhandled.IsTerminating)
            {
                OnClose(sender, e);
                GameLogger.CaptureCrash();
            }
        }

        protected virtual void OnClose(object? sender, EventArgs e)
        {
            // right before the game itself closes.
            Haptics.ClearAll();
            _game?.OnClose();
        }
    }
}