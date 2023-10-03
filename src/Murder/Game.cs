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

namespace Murder
{
    public partial class Game : Microsoft.Xna.Framework.Game
    {
        /* *** Static properties of the Game *** */

        /// <summary>
        /// Singleton instance of the game. Be cautious when referencing this...
        /// </summary>
        public static Game Instance { get; private set; } = null!;

        new public static GraphicsDevice GraphicsDevice => Instance._graphics.GraphicsDevice;

        public static GameDataManager Data => Instance._gameData;

        public static SaveData Save => Instance._gameData.ActiveSaveData;

        public static GamePreferences Preferences => Instance._gameData.Preferences;

        public static GameProfile Profile => Instance._gameData.GameProfile;

        public static ISoundPlayer Sound => Instance.SoundPlayer;

        public static Random Random = new();

        public static int Width => Profile.GameWidth;
        public static int Height => Profile.GameHeight;

        public static PlayerInput Input => Instance._playerInput;

        public static float DeltaTime => (float)Instance._escaledDeltaTime;
        public static float UnscaledDeltaTime => (float)Instance._unescaledDeltaTime;

        public static float Now => (float)Instance._escaledElapsedTime;
        public static float PreviousNow => (float)Instance._scaledPreviousElapsedTime;
        public static float NowUnscaled => (float)Instance._unescaledElapsedTime;
        public static float PreviousNowUnscaled => (float)Instance._unescaledPreviousElapsedTime;

        public static float FixedDeltaTime => Instance._fixedUpdateDelta;
        public static float ElapsedDeltaTime => (float)Instance._escaledDeltaTime;

        /* *** Protected helpers *** */

        protected readonly Microsoft.Xna.Framework.GraphicsDeviceManager _graphics;

        protected readonly PlayerInput _playerInput;

        protected readonly GameDataManager _gameData;

        public readonly ISoundPlayer SoundPlayer;

        /// <summary>
        /// Initialized in <see cref="LoadContent"/>.
        /// </summary>
        protected SceneLoader? _sceneLoader;

        protected virtual Scene InitialScene => new GameScene(Profile.StartingScene);

        /* *** Public instance fields *** */

        public Scene? ActiveScene => _sceneLoader?.ActiveScene;

        public const float LONGEST_TIME_RESET = 5f;

        public float UpdateTime { get; private set; }
        public float LongestUpdateTime { get; private set; }
        private float _longestUpdateTimeAt;

        public float RenderTime { get; private set; }
        public float LongestRenderTime { get; private set; }
        private float _longestRenderTimeAt;
        
        /// <summary>
        /// Elapsed time in seconds from the previous update frame since the game started
        /// </summary>
        public float PreviousElapsedTime => (float)_scaledPreviousElapsedTime;

        public bool IsPaused { get; private set; }

        /// <summary>
        /// If set, this is the amount of frames we will skip while rendering.
        /// </summary>
        private int _freezeFrameCount = 0;

        /// <summary>
        /// Time since we have been freezing the frames.
        /// </summary>
        private double _freezeFrameTime = 0;

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

        private float? _slowDownScale;

        protected virtual bool AlwaysUpdateBeforeFixed => true;

        private Point _windowedSize = Point.Zero;
        public bool Fullscreen
        {
            get => Profile.Fullscreen;
            set
            {
                Profile.Fullscreen = value;
                RefreshWindow();
            }
        }

        public Vector2 GameScale 
        {
            get
            {
                if (Window.ClientBounds.Width <= 0 || Window.ClientBounds.Height <= 0)
                    return Vector2.One;
                    
                return new(
                    ((float)_screenSize.X / Window.ClientBounds.Width) / Profile.GameScale,
                    ((float)_screenSize.Y / Window.ClientBounds.Height) / Profile.GameScale );
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
        private double _targetFixedUpdateTime = 0;

        private double _escaledElapsedTime = 0;
        private double _unescaledElapsedTime = 0;
        
        private double _scaledPreviousElapsedTime = 0;
        private double _unescaledPreviousElapsedTime = 0;

        private double _escaledDeltaTime = 0;
        private double _unescaledDeltaTime = 0;

        /// <summary>
        /// This is the underlying implementation of the game. This listens to the murder game events.
        /// </summary>
        private readonly IMurderGame? _game;

        /// <summary>
        /// Single logger of the game.
        /// </summary>
        protected GameLogger _logger;

        public RenderContext CreateRenderContext(GraphicsDevice graphicsDevice, Camera2D camera, bool useCustomShader) => _game?.CreateRenderContext(graphicsDevice, camera, useCustomShader) ?? new RenderContext(graphicsDevice, camera, useCustomShader); 

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
                if (_graphics != null)
                {
                    _graphics.ApplyChanges();
                }

                ActiveScene?.RefreshWindow(GraphicsDevice, Profile); // TODO: Change this to the scale defined in the options
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = HasCursor || (game?.HasCursor ?? false);

            _logger = GameLogger.GetOrCreateInstance();
            _logger.Initialize();
            
            _playerInput = new PlayerInput();
            SoundPlayer = game?.CreateSoundPlayer() ?? new SoundPlayer();

            _game = game;
            _gameData = dataManager;

            _graphics = new(this);
        }

        protected override void Initialize()
        {
            // Register Input

            // Editor input
            _playerInput.Register(MurderInputButtons.Debug, Keys.OemTilde, Keys.F1);
            _playerInput.Register(MurderInputButtons.PlayGame, Keys.OemTilde, Keys.F5);
            _playerInput.Register(MurderInputButtons.LeftClick, MouseButtons.Left);
            _playerInput.Register(MurderInputButtons.RightClick, MouseButtons.Right);
            _playerInput.Register(MurderInputButtons.MiddleClick, MouseButtons.Middle);

            _playerInput.Register(MurderInputButtons.Shift, Keys.LeftShift);
            _playerInput.Register(MurderInputButtons.Esc, Keys.Escape);
            _playerInput.Register(MurderInputButtons.Delete, Keys.Delete);
            _playerInput.Register(MurderInputButtons.Ctrl, Keys.LeftControl, Keys.RightControl);
            _playerInput.Register(MurderInputButtons.Space, Keys.Space);

            // Navigation input
            _playerInput.Register(MurderInputButtons.Submit, Keys.Space, Keys.Enter);
            _playerInput.Register(MurderInputButtons.Submit, Buttons.A, Buttons.Y);

            _playerInput.Register(MurderInputButtons.Cancel, Buttons.B, Buttons.Back, Buttons.Start);

            _playerInput.Register(MurderInputButtons.Pause, Keys.Escape, Keys.P);
            _playerInput.Register(MurderInputButtons.Pause, Buttons.Start);

            _playerInput.RegisterAxes(MurderInputAxis.Ui, GamepadAxis.LeftThumb, GamepadAxis.RightThumb, GamepadAxis.Dpad);
            _playerInput.Register(MurderInputAxis.Ui,
                new InputButtonAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new InputButtonAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));
            
            base.Initialize(); // Content is loaded here
            _gameData.InitializeAssets();

            // Setting window size
            RefreshWindow();

            _game?.Initialize();
        }

        public virtual void RefreshWindow()
        {
            SetTargetFps(Profile.TargetFps, Profile.FixedUpdateFactor);

            _screenSize = new Point(Width, Height) * Data.GameProfile.GameScale;

            SetWindowSize(_screenSize);
            _graphics.ApplyChanges();


            if (!Fullscreen)
            {
                // This seems to be a bug in Monogame
                // This line must be repeated otherwise the window won't be
                // borderless.
                Window.IsBorderless = false;
            }
         
            ActiveScene?.RefreshWindow(GraphicsDevice, Profile);
        }
        protected virtual void SetWindowSize(Point screenSize)
        {
            if (Fullscreen)
            {
                _windowedSize = _graphics.GraphicsDevice.Viewport.Bounds.Size;

                Window.IsBorderless = true;
                _graphics.HardwareModeSwitch = false;
                _graphics.IsFullScreen = true;
#if DEBUG
                _graphics.SynchronizeWithVerticalRetrace = true;
#endif

                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                _graphics.IsFullScreen = false;
                Window.IsBorderless = false;
#if DEBUG
                _graphics.SynchronizeWithVerticalRetrace = false;
#endif
                
                if (_windowedSize.X > 0 && _windowedSize.Y > 0)
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
        }

        protected override void LoadContent()
        {
            using PerfTimeRecorder recorder = new("Game Content");

            // Initialize our actual sound player!
            SoundPlayer.Initialize(_gameData.BinResourcesDirectoryPath);

            _gameData.Initialize();
            ApplyGameSettings();

            LoadContentImpl();

            _gameData.LoadShaders(true);

            // Load assets, textures, content, etc
            _gameData.LoadContent();

            // Initialize the initial scene.
            _sceneLoader = new SceneLoader(_graphics, Profile, InitialScene);

            _ = LoadSceneAsync(waitForAllContent: true);
        }

        protected virtual void LoadContentImpl() { }

        /// <summary>
        /// This will apply the game settings according to <see cref="GameProfile"/>, loaded with <see cref="_gameData"/>. />.
        /// </summary>
        protected void ApplyGameSettings()
        {
            // This will keep the camera and other render positions in sync with the fixed update.
            _graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;

            ApplyGameSettingsImpl();

            _graphics.ApplyChanges();
        }

        protected virtual void ApplyGameSettingsImpl() { }

        protected virtual async Task LoadSceneAsync(bool waitForAllContent)
        {
            GameLogger.Verify(_sceneLoader is not null);

            if (waitForAllContent && _gameData.LoadContentProgress is not null)
            {
                await _gameData.LoadContentProgress;
            }

            _game?.LoadContentAsync();
            _game?.OnSceneTransition();

            _sceneLoader.LoadContent();
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

        public void SetWaitForSaveComplete()
        {
            _waitForSaveComplete = true;
        }

        public bool CanResumeAfterSaveComplete()
        {
            bool result;

            if (Data.TryGetActiveSaveData() is not SaveData save)
            {
                // No active save, so yes?
                result = true;
            }
            else
            {
                result = save.HasFinishedSaveWorld();
            }

            if (result)
            {
                _waitForSaveComplete = false;
            }

            return result;
        }

        /// <summary>
        /// This will slow down the game time.
        /// TODO: What if we have multiple slow downs in the same run?
        /// </summary>
        public void SlowDown(float scale)
        {
            _slowDownScale = scale;
        }

        public void RevertSlowDown()
        {
            _slowDownScale = null;
        }

        /// <summary>
        /// This will resume the game.
        /// </summary>
        public void Resume()
        {
            _slowDownScale = null;
            IsPaused = false;
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_waitForSaveComplete && !CanResumeAfterSaveComplete())
            {
                UpdateUnescaledDeltaTime(gameTime.ElapsedGameTime.TotalSeconds);
                _targetFixedUpdateTime = _unescaledElapsedTime;

                // Don't do any logic operation yet, we are waiting for the save to complete.
                return;
            }

            UpdateImpl(gameTime);

            while (_isSkippingDeltaTimeOnUpdate)
            {
                UpdateImpl(gameTime);
            }

            // Update sound logic!
            SoundPlayer.Update();
        }

        protected void UpdateImpl(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // If this is set, the game has been frozen for some frames.
            // We will simply wait until this returns properly.
            if (_freezeFrameCount > 0)
            {
                _freezeFrameTime += gameTime.ElapsedGameTime.TotalSeconds;

                if (_freezeFrameTime >= _fixedUpdateDelta)
                {
                    _freezeFrameCount--;
                    _freezeFrameTime = 0;
                }

                return;
            }

            DoPendingExitGame();
            DoPendingWorldTransition();

            GameLogger.Verify(ActiveScene is not null);

            var startTime = DateTime.Now;

            double deltaTime = _isSkippingDeltaTimeOnUpdate ? 
                TargetElapsedTime.TotalSeconds : gameTime.ElapsedGameTime.TotalSeconds;

            UpdateUnescaledDeltaTime(deltaTime);

            if (_slowDownScale.HasValue)
            {
                deltaTime *= _slowDownScale.Value;
            }
            
            if (IsPaused)
            {
                // Make sure we don't update the escaled delta time.
                deltaTime = 0;
            }

            UpdateEscaledDeltaTime(deltaTime);
            UpdateInputAndScene();

            // Check for fixed updates as well! TODO: Do we need to recover from lost frames?
            // See https://github.com/amzeratul/halley/blob/41cd76c927ce59cfcc400f8cdf5f1465e167341a/src/engine/core/src/game/main_loop.cpp
            int maxRecoverFrames = 3;
            while (_unescaledElapsedTime >= _targetFixedUpdateTime)
            {
                ActiveScene.FixedUpdate();
                _targetFixedUpdateTime += _fixedUpdateDelta;

                if (maxRecoverFrames-- == 0)
                {
                    _targetFixedUpdateTime = (float)_unescaledElapsedTime; // Just slow down the game at this point, sorry.
                }
                else if (AlwaysUpdateBeforeFixed)
                {
                    // Update must always run before FixedUpdate
                    UpdateInputAndScene();
                }
            }

            base.Update(gameTime);

            UpdateTime = (float)(DateTime.Now - startTime).TotalMilliseconds;
            
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

            _game?.OnUpdate();
        }

        private void UpdateInputAndScene()
        {
            GameLogger.Verify(ActiveScene is not null);

            _playerInput.Update();
            ActiveScene.Update();
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GameLogger.Verify(ActiveScene is not null);

            var startTime = DateTime.Now;

            if (!ActiveScene.Loaded && ActiveScene.RenderContext is RenderContext renderContext)
            {
                OnLoadingDraw(renderContext);
            }

            ActiveScene.Draw();

            base.Draw(gameTime);
            DrawImGui(gameTime);

            RenderTime = (float)(DateTime.Now - startTime).TotalMilliseconds;

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

            _game?.OnDraw();
        }

        /// <summary>
        /// Display drawing for the load animation.
        /// </summary>
        protected virtual void OnLoadingDraw(RenderContext renderContext)
        {
            _game?.OnLoadingDraw(renderContext);
        }

        protected virtual void DrawImGui(Microsoft.Xna.Framework.GameTime gameTime) { }

        public virtual void BeginImGuiTheme() {}
        public virtual void EndImGuiTheme() {}

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

        private void UpdateUnescaledDeltaTime(double deltaTime)
        {
            _unescaledPreviousElapsedTime = _unescaledElapsedTime;
            _unescaledElapsedTime += deltaTime;
            _unescaledDeltaTime = deltaTime;
        }

        private void UpdateEscaledDeltaTime(double deltaTime)
        {
            _scaledPreviousElapsedTime = _escaledElapsedTime;
            _escaledElapsedTime += deltaTime;
            _escaledDeltaTime = deltaTime;
        }

        private void SetTargetFps(int fps, float fixedUpdateFactor)
        {
            //_targetFps = fps;
            _fixedUpdateDelta = 1f / (fps / fixedUpdateFactor);
        }

        /// <summary>
        /// Exit the game. This is used to wrap any custom behavior depending on the game implementation.
        /// </summary>
        protected virtual void ExitGame()
        {
            _game?.OnExit();

            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
            base.Exit();
        }
    }
}
