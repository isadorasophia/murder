using Bang;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.Assets;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Editor.Data;
using Murder.Editor.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Systems.Debug;
using Murder.Editor.Utilities;
using Murder.Utilities;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Murder.Editor
{
    public class Architect : Game
    {
        /* *** Static properties of the Architect *** */

        new public static Architect Instance { get; private set; } = null!;

        public static EditorSettingsAsset EditorSettings => EditorData.EditorSettings;

        public static EditorDataManager EditorData => (EditorDataManager)Instance._gameData;

        internal static EditorGraphLogger EditorGraphLogger => (EditorGraphLogger)Instance.GraphLogger;

        internal static IMurderArchitect? Game => Instance._game as IMurderArchitect;

        public static UndoTracker Undo => Instance._undo;

        /// <summary>
        /// Debug and editor buffer renderer.
        /// Called in <see cref="Initialize"/>.
        /// </summary>
        public ImGuiRenderer ImGuiRenderer = null!;

        /// <summary>
        /// This handles all the ImGui texture display in the screen.
        /// </summary>
        public static ImGuiTextureManager ImGuiTextureManager => EditorData.ImGuiTextureManager;

        private EditorScene? _editorScene = null;

        private readonly UndoTracker _undo = new(capacity: 256);

        protected override Scene InitialScene => _editorScene ??= new();

        /* *** SDL helpers *** */
        private const int SDL_WINDOW_MAXIMIZED = 0x00000080;

        /* *** Architect state *** */
        private bool _isPlayingGame = false;

        private StartPlayGameInfo? _queueStartPlayGame = null;

        protected override bool AlwaysUpdateBeforeFixed => _isPlayingGame;

        public bool IsPlayingGame => _isPlayingGame;

        protected override bool IsDiagnosticEnabled => true;

        public override GraphLogger GraphLogger { get; } = new EditorGraphLogger();

        public CursorStyle Cursor { get; set; } = CursorStyle.Normal;

        /// <summary>
        /// Last asset opened in the editor.
        /// </summary>
        [HideInEditor]
        public Guid LastOpenedAsset = Guid.Empty;

        protected override bool HasCursor => true;

        public Architect(IMurderArchitect? game = null, EditorDataManager? editorDataManager = null) : base(game, editorDataManager ?? new EditorDataManager(game)) { }

        protected override void Initialize()
        {
            Instance = this;

            _playerInput.Register(MurderInputAxis.EditorCamera,
                new InputButtonAxis(Keys.W, Keys.A, Keys.S, Keys.D));

            ImGuiRenderer = new ImGuiRenderer(this);
            ImGuiRenderer.RebuildFontAtlas();

            _logger = EditorGameLogger.OverrideInstanceWithEditor();
            _ = EditorDebugSnapshot.OverrideInstanceWithEditor();

            InitializeImGui();

            base.Initialize();

            ImGuiRenderer.InitTheme();
        }

        private void InitializeImGui()
        {
            // Magic so ctrl+c and ctrl+v work in mac and linux!
            if ((OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()))
            {
                if (!OperatingSystemHelpers.ClipboardDependencyExists())
                {
                    var missingDependency = OperatingSystem.IsMacOS()
                        ? OperatingSystemHelpers.AppKit
                        : OperatingSystemHelpers.SDL;
                    GameLogger.Error($"Clipboard support is disabled. Could not load necessary dependency: '{missingDependency}'.");
                    return;
                }

                ImGuiIOPtr io = ImGui.GetIO();

                if (OperatingSystemHelpers.GetFnPtr is IntPtr getFnPtr)
                {
                    io.GetClipboardTextFn = getFnPtr;
                }

                if (OperatingSystemHelpers.SetFnPtr is IntPtr setFnPtr)
                {
                    io.SetClipboardTextFn = setFnPtr;
                }
            }
        }

        public override void SetWindowSize(Point screenSize, bool remember)
        {
            if (_isPlayingGame)
            {
                base.SetWindowSize(screenSize, false);
                return;
            }

            if (!IsMaximized() && EditorSettings.WindowStartPosition.X > 0 && EditorSettings.WindowStartPosition.Y > 0)
            {
                Point startPos = EditorSettings.WindowStartPosition;
                SetWindowPosition(startPos);
            }

            Point displaySize = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.TitleSafeArea.Size();
            
            if (EditorSettings.WindowSize.X > 0 && EditorSettings.WindowSize.Y > 0)
            {
                Point diffToMaxSize = displaySize - EditorSettings.WindowSize;

                if (diffToMaxSize.Y < 80)
                {
                    // This is too big, the user probably just wants the screen to be maximized.
                    MaximizeWindow();
                    _graphics.PreferredBackBufferWidth = displaySize.X;
                    _graphics.PreferredBackBufferHeight = displaySize.Y;
                }
                else
                {
                    // Clamp to the current display size with a small margin.
                    Point minSize = new(800, 600);
                    Point maxSize = (displaySize - new Point(100, 100)).Max(minSize);
                    screenSize = EditorSettings.WindowSize.Clamp(minSize, maxSize);

                    _graphics.PreferredBackBufferWidth = screenSize.X;
                    _graphics.PreferredBackBufferHeight = screenSize.Y;
                }
            }

            if (EditorSettings.StartMaximized)
            {
                if (GetWindowPosition() is Point startPosition)
                {
                    SetWindowPosition(new Point(startPosition.X, startPosition.Y));
                }

                MaximizeWindow();
                _graphics.PreferredBackBufferWidth = displaySize.X;
                _graphics.PreferredBackBufferHeight = displaySize.Y;
            }
        }

        private void QuitToEditor()
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();

            Resume();
            Input.LockInput(false);

            SoundPlayer.Stop(Murder.Core.Sounds.SoundLayer.Any, fadeOut: false);

            GameLogger.Verify(_sceneLoader is not null);

            _isPlayingGame = false;

            if (ActiveScene is GameScene)
            {
                Debug.Assert(_editorScene is not null);

                _sceneLoader.SwitchScene(_editorScene);

                // Manually set things up in the editor scene.
                _editorScene.Reload();

                // RefreshWindow();
            }

            // Here, let's mock what a real "quit" would do.
            // Manually unload and load all saves.
            Data.UnloadAllSaves();
            Data.LoadAllSaves();

            (_gameData as EditorDataManager)?.RefreshAfterSave();

            _playerInput.ClearBinds(MurderInputButtons.PlayGame);

            if (Fullscreen)
            {
                Fullscreen = false;
            }
        }

        /// <summary>
        /// Queues an operation for start playing a game. This will be queued and executed in the next
        /// update call.
        /// </summary>
        public void QueueStartPlayingGame(bool quickplay, Guid? startingScene = null)
        {
            StartPlayGameInfo info = new() { IsQuickplay = quickplay, StartingScene = startingScene };
            _queueStartPlayGame = info;
        }

        /// <summary>
        /// Queues an operation for start playing a game. This will be queued and executed in the next
        /// update call.
        /// </summary>
        public void QueueStartPlayingGame(StartPlayGameInfo info)
        {
            _queueStartPlayGame = info;
        }

        private void PlayGame(StartPlayGameInfo info)
        {
            Guid actualStartingScene = info.StartingScene ?? Profile.StartingScene;

            // Data.ResetActiveSave();

            WorldAsset? world = actualStartingScene != Guid.Empty ? Data.TryGetAsset<WorldAsset>(actualStartingScene) : null;
            if (!info.IsQuickplay && world is null)
            {
                GameLogger.Error("Unable to start the game, please specify a valid starting scene on \"Game Profile\".");
                return;
            }

            if (world is { HasSystems: false })
            {
                GameLogger.Error($"Unable to start the game, '{world.Name}' has no systems. Add at least one system to the world.");
                return;
            }

            Resume();

            GameLogger.Verify(_sceneLoader is not null);

            SaveWindowPosition();
            _isPlayingGame = true;

            //ActiveScene?.RefreshWindow(new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), GraphicsDevice, Profile);

            bool shouldLoad = true;
            if (info.IsQuickplay)
            {
                shouldLoad = SwitchToQuickPlayScene();
            }
            else
            {
                _sceneLoader.SwitchScene(actualStartingScene);
            }

            if (shouldLoad)
            {
                // Make sure we load the save before playing the game.
                Data.LoadSaveAsCurrentSave(slot: info.SaveSlot ?? -1);

                LoadSceneAsync(waitForAllContent: true).Wait();
            }
            else
            {
                _isPlayingGame = false;
            }

            Game?.OnBeforePlayGame(info);

            _playerInput.Consume(MurderInputButtons.PlayGame);

            _playerInput.Bind(MurderInputButtons.PlayGame, (input) =>
            {
                _playerInput.Consume(MurderInputButtons.PlayGame);
                QuitToEditor();
            });
        }

        private bool SwitchToQuickPlayScene()
        {
            GameLogger.Verify(_sceneLoader is not null);

            // Handle awkward quick save loading.
            if (Data.TryGetActiveSaveData() is null)
            {
                if (!Data.LoadSaveAsCurrentSave(slot: -1))
                {
                    GameLogger.Warning("Quick play currently only works on a loaded save.");
                    return false;
                }
            }

            if (EditorSettings.QuickStartScene == Guid.Empty)
            {
                GameLogger.Warning("Set a Quick Start Scene on Editor Settings first!");
                return false;
            }

            _sceneLoader.SwitchScene(EditorSettings.QuickStartScene);
            return true;
        }

        public void ReloadContent()
        {
            GameLogger.Log("===== Reloading content! =====", Data.GameProfile.Theme.Green);
            GameLogger.Log("Saving current editor settings...", Data.GameProfile.Theme.Green);

            EditorData.SaveAsset(EditorData.EditorSettings);

            Data.ClearContent();
            Data.Initialize();

            if (ActiveScene is EditorScene editor)
            {
                editor.ReopenTabs();
            }
        }

        protected override void LoadContentImpl() { }

        protected override async Task LoadSceneAsync(bool waitForAllContent)
        {
            GameLogger.Verify(_sceneLoader is not null);

            if (!EditorData.EditorSettings.StartOnEditor)
            {
                if (Profile.StartingScene == Guid.Empty)
                {
                    GameLogger.Error("Unable to start the game, please specify a valid starting scene on \"Game Profile\".");
                    return;
                }
                else
                {
                    // Switch scene to game.
                    _sceneLoader.SwitchScene(Profile.StartingScene);
                    GameLogger.Log($"Game will start!");
                }
            }

            await base.LoadSceneAsync(waitForAllContent);

            if (ActiveScene?.World is World world)
            {
                world.AddEntity(new EditorComponent());
                EditorHook? hook = world.GetUnique<EditorComponent>().EditorHook;

                hook.DrawEntityInspector += EntityInspector.DrawInspector;
            }
        }

        private bool _isForeground = false;

        protected override void DrawImGui(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GameLogger.Verify(ActiveScene is not null);

            ImGuiRenderer.BeforeLayout(gameTime);

            ActiveScene.DrawGui();

            if (!IsActive)
            {
                _isForeground = true;

                ImGui.SetNextWindowBgAlpha(0.01f);
                ImGui.Begin("Editor is not focused!", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
                ImGui.BeginDisabled();
                ImGui.PushStyleColor(ImGuiCol.Button, Profile.Theme.Faded);
                ImGui.Button("Murder Engine is not focused.");
                ImGui.PopStyleColor();
                ImGui.EndDisabled();
                ImGui.SetWindowPos(new System.Numerics.Vector2());
                ImGui.SetWindowSize(ImGui.GetMainViewport().Size);
                ImGui.SetWindowFocus();
                ImGui.End();
            }
            else if (_isForeground)
            {
                // Window is now active and was previously on foreground on the last frame.
                EditorData.ReloadOnWindowForeground();

                _isForeground = false;
            }

            if (!_isPlayingGame)
            {
                // Outside of the game, also display the console.
                _logger.DrawConsole();
            }

            ImGuiRenderer.AfterLayout();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            GameLogger.Log("Wrapping up, bye!");

            ImGuiRenderer.AfterLayout();

            if (!_isPlayingGame)
            {
                SaveWindowPosition();
                SaveEditorState();
            }

            EditorData.SaveSettings();
        }

        internal void SaveWindowPosition()
        {
            bool isMaximized = IsMaximized();
            if (!isMaximized && GetWindowPosition() is Point position)
            {
                EditorSettings.WindowStartPosition = position;
                EditorSettings.WindowSize = Window.ClientBounds.Size();
            }

            EditorSettings.StartMaximized = isMaximized;
        }

        private void SaveEditorState()
        {
            if (ActiveScene is EditorScene editor)
            {
                editor.SaveEditorState();
            }
            else
            {
                GameLogger.Fail("How was this called out of an Editor scene?");
            }
        }

        protected bool IsMaximized()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SDL3.SDL.SDL_WindowFlags windowState = SDL3.SDL.SDL_GetWindowFlags(Window.Handle);
                return (windowState & SDL3.SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED) != 0;
            }

            return false;
        }

        protected void MaximizeWindow()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SDL3.SDL.SDL_MaximizeWindow(Window.Handle);
            }
        }

        protected Point? GetWindowPosition()
        {
            // Not sure what is not supported here?
            bool supportedOs = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || 
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            if (supportedOs)
            {
                SDL3.SDL.SDL_GetWindowPosition(Window.Handle, out int x, out int y);
                return new(x, y);
            }

            return null;
        }

        protected bool SetWindowPosition(Point p)
        {
            // Not sure what is not supported here?
            bool supportedOs = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            if (supportedOs)
            {
                SDL3.SDL.SDL_SetWindowPosition(Window.Handle, p.X, p.Y);
                return true;
            }

            return false;
        }

        public override void RefreshWindow()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            io.FontGlobalScale = Math.Clamp(Architect.EditorSettings.FontScale, 1, 2);

            base.RefreshWindow();
        }

        /// <summary>
        /// Refresh buffer target after reloading shaders. This CANNOT be called while drawing! Or it will crash!
        /// </summary>
        public void RefreshWindowsBufferAfterReloadingShaders()
        {
            if (ActiveScene is null)
            {
                return;
            }
            // TODO: Is this really necessary??
            // ActiveScene.RefreshWindow(_graphics.GraphicsDevice, Profile);
        }

        protected override void ExitGame()
        {
            if (_isPlayingGame)
            {
                QuitToEditor();
            }
            else
            {
                base.ExitGame();
            }
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_queueStartPlayGame is StartPlayGameInfo info)
            {
                _queueStartPlayGame = null;
                PlayGame(info);
            }

            Input.MouseConsumed = ImGui.GetIO().WantCaptureMouse && _isPlayingGame;
            Input.KeyboardConsumed = ImGui.GetIO().WantCaptureKeyboard;


            base.Update(gameTime);

            if (EditorData.ShadersNeedReloading)
            {
                // We must make sure this is not called while drawing.
                EditorData.ReloadShaders();
            }

            // TODO: Figure out how to listen to drop file events
            SDL3.SDL.SDL_SetEventEnabled((uint)SDL3.SDL.SDL_EventType.SDL_EVENT_DROP_FILE, true);
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            EditorData.CursorTextureManager?.RenderCursor(Cursor);
        }

        protected override void OnLoadingDraw(RenderContext renderContext) { }

        protected override void ApplyGameSettingsImpl()
        {
            // This will allow us to run as many updates as possible in editor, for debugging.
            // keywords: Framerate, FPS, VSync
            _graphics.SynchronizeWithVerticalRetrace = Architect.EditorSettings.LockFramerate;
            IsFixedTimeStep = Architect.EditorSettings.LockFramerate;
        }

        protected override void Dispose(bool isDisposing)
        {
            ImGuiRenderer?.Dispose();

            base.Dispose(isDisposing);
        }
    }
}