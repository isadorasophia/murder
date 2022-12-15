using Microsoft.Xna.Framework.Input;
using Murder.Diagnostics;
using Murder.Core.Input;
using Murder.Core.Geometry;
using Microsoft.Xna.Framework.Graphics;
using Murder.Assets;
using Murder.Data;
using Murder.Save;
using Murder.Core;

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

        public static Random Random = new();

        public static int Width => Profile.GameWidth;
        public static int Height => Profile.GameHeight;

        public static PlayerInput Input => Instance._playerInput;

        public static float DeltaTime => (float)Instance._escaledDeltaTime;
        public static float UnescaledDeltaTime => (float)Instance._unescaledDeltaTime;

        public static float Now => Instance.ElapsedTime;
        public static float NowUnescaled => (float)Instance._unescaledElapsedTime;

        public static float FixedDeltaTime => Instance._fixedUpdateDelta;
        public static float ElapsedDeltaTime => (float)Instance._escaledDeltaTime;

        /* *** Protected helpers *** */

        protected readonly Microsoft.Xna.Framework.GraphicsDeviceManager _graphics;

        protected readonly PlayerInput _playerInput;

        protected readonly GameDataManager _gameData;

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
        /// Elapsed time in seconds since the game started
        /// </summary>
        public float ElapsedTime => (float)_escaledElapsedTime;

        public bool IsPaused { get; private set; }

        /// <summary>
        /// If set, this is the amount of frames we will skip while rendering.
        /// </summary>
        private int _freezeFrameCount = 0;

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

        private float? _slowDownScale;

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

        private Point _screenSize;

        // TODO: Make this private or within a setter or whatever.
        // We need to refresh window when it does get set...
        public float DPIScale = 100;
        public float Downsample = 1;

        /* *** Private instance fields *** */

        // Update properties.
        private float _fixedUpdateDelta;
        private double _targetFixedUpdateTime = 0;

        private double _escaledElapsedTime = 0;
        private double _unescaledElapsedTime = 0;

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
            IsMouseVisible = true;

            _logger = GameLogger.GetOrCreateInstance();
            _logger.Initialize();
            
            _playerInput = new PlayerInput();

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

            // Navigation input
            _playerInput.Register(MurderInputButtons.Submit, Keys.Space, Keys.Enter);
            _playerInput.Register(MurderInputButtons.Submit, Buttons.A, Buttons.Y, Buttons.LeftShoulder, Buttons.RightShoulder);

            _playerInput.Register(MurderInputButtons.Exit, Buttons.B, Buttons.Back);

            _playerInput.Register(MurderInputButtons.Pause, Keys.Escape, Keys.P);
            _playerInput.Register(MurderInputButtons.Pause, Buttons.Start);

            _playerInput.Register(MurderInputAxis.Ui, GamepadAxis.LeftThumb, GamepadAxis.RightThumb, GamepadAxis.Dpad);
            _playerInput.Register(MurderInputAxis.Ui,
                new KeyboardAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new KeyboardAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));
            
            _playerInput.Bind(MurderInputButtons.Debug, (i) => { _logger.ToggleDebugWindow(); });

            base.Initialize(); // Content is loaded here
            Data.InitializeAssets();

            // Setting window size
            RefreshWindow();

            _game?.Initialize();
        }

        public virtual void RefreshWindow()
        {
            SetTargetFps(Profile.TargetFps, Profile.FixedUpdateFactor);

            _screenSize = new Point(Width, Height) * Data.GameProfile.GameScale;

            SetWindowSize(_screenSize);

            _graphics.IsFullScreen = Fullscreen;
            _graphics.ApplyChanges();

            ActiveScene?.RefreshWindow(GraphicsDevice, Profile); // TODO: Change this to the scale defined in the options
        }

        protected virtual void SetWindowSize(Point screenSize)
        {
            if (Fullscreen)
            {
                _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = screenSize.X;
                _graphics.PreferredBackBufferHeight = screenSize.Y;
            }
        }

        protected override void LoadContent()
        {
            var now = DateTime.Now;

            LoadContentImpl();

            // Load assets, textures, content, etc
            _gameData.LoadContent();

            GameLogger.Log($"Game content loaded! I did it in {(DateTime.Now - now).Milliseconds} ms");

            LoadSceneAsync().Wait();
            _game?.LoadContentAsync().Wait();
        }

        protected virtual void LoadContentImpl()
        {
            _gameData.Init();
            ApplyGameSettings();

            _sceneLoader = new SceneLoader(_graphics, Profile, InitialScene);
            _gameData.LoadShaders(true);
        }

        /// <summary>
        /// This will apply the game settings according to <see cref="GameProfile"/>, loaded with <see cref="_gameData"/>. />.
        /// </summary>
        protected void ApplyGameSettings()
        {
            var settings = _gameData.GameProfile;

            // This will allow us to run as many updates as possible.
            _graphics.SynchronizeWithVerticalRetrace = settings.IsVSyncEnabled;
            IsFixedTimeStep = false;

            _graphics.ApplyChanges();
        }

        protected virtual async Task LoadSceneAsync()
        {
            GameLogger.Verify(_sceneLoader is not null);

            var now = DateTime.Now;

            // Load the initial scene!
            await _sceneLoader.LoadContentAsync();
            _game?.OnSceneTransition();

            GameLogger.Log($"Scene loaded in {(DateTime.Now - now).Milliseconds} ms");
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
            // If this is set, the game has been frozen for some frames.
            // We will simply wait until this returns properly.
            if (_freezeFrameCount > 0)
            {
                _freezeFrameCount--;
                return;
            }

            DoPendingWorldTransition();

            GameLogger.Verify(ActiveScene is not null);

            var startTime = DateTime.Now;

            _playerInput.Update();

            double deltaTime = _isSkippingDeltaTimeOnUpdate ? 
                TargetElapsedTime.TotalSeconds : gameTime.ElapsedGameTime.TotalSeconds;

            _unescaledElapsedTime += deltaTime;
            _unescaledDeltaTime = deltaTime;

            if (_slowDownScale.HasValue)
            {
                deltaTime *= _slowDownScale.Value;
            }
            
            if (IsPaused)
            {
                deltaTime = 0;
            }

            _escaledElapsedTime += deltaTime;
            _escaledDeltaTime = deltaTime;
            
            ActiveScene.Update();

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
            }

            base.Update(gameTime);

            UpdateTime = (float)(DateTime.Now - startTime).TotalMilliseconds;
            
            if (ElapsedTime > _longestUpdateTimeAt + LONGEST_TIME_RESET)
            {
                _longestUpdateTimeAt = ElapsedTime;
                LongestUpdateTime = 0.0f;
            }

            if (UpdateTime > LongestUpdateTime)
            {
                _longestUpdateTimeAt = ElapsedTime;
                LongestUpdateTime = UpdateTime;
            }

            _game?.OnUpdate();

            while (_isSkippingDeltaTimeOnUpdate)
            {
                Update(gameTime);
                Draw(gameTime);
            }
        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GameLogger.Verify(ActiveScene is not null);

            var startTime = DateTime.Now;

            ActiveScene.Draw();

            base.Draw(gameTime);
            DrawImGui(gameTime);

            RenderTime = (float)(DateTime.Now - startTime).TotalMilliseconds;

            if (ElapsedTime > _longestRenderTimeAt + LONGEST_TIME_RESET)
            {
                _longestRenderTimeAt = ElapsedTime;
                LongestRenderTime = 0.0f;
            }

            if (RenderTime > LongestRenderTime)
            {
                _longestRenderTimeAt = ElapsedTime;
                LongestRenderTime = UpdateTime;
            }

            _game?.OnDraw();
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

            base.Dispose(isDisposing);
        }

        private void SetTargetFps(int fps, float fixedUpdateFactor)
        {
            //_targetFps = fps;
            _fixedUpdateDelta = 1f / (fps / fixedUpdateFactor);
        }

        /// <summary>
        /// Exit the game. This is used to wrap any custom behavior depending on the game implementation.
        /// </summary>
        public virtual void ExitGame()
        {
            _game?.OnExit();

            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
            base.Exit();
        }
    }
}
