using Bang;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
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
using Murder.Editor.EditorCore;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Systems.Debug;
using Murder.Editor.Utilities;
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

        protected override Scene InitialScene => _editorScene ??= new();

        /* *** SDL helpers *** */

        private const string SDL = "SDL2.dll";
        private const int SDL_WINDOW_MAXIMIZED = 0x00000080;

        [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_MaximizeWindow(IntPtr window);

        [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowFlags(IntPtr window);


        /* *** Architect state *** */
        private bool _isPlayingGame = false;

        private StartPlayGameInfo? _queueStartPlayGame = null;

        protected override bool AlwaysUpdateBeforeFixed => _isPlayingGame;

        public bool IsPlayingGame => _isPlayingGame;

        protected override bool IsDiagnosticEnabled => true;

        public override GraphLogger GraphLogger { get; } = new EditorGraphLogger();

        public CursorStyle Cursor { get; set; } = CursorStyle.Normal;

        protected override bool HasCursor => true;

        public Architect(IMurderArchitect? game = null, EditorDataManager? editorDataManager = null) : base(game, editorDataManager ?? new EditorDataManager(game)) { }

        protected override void Initialize()
        {
            Instance = this;

            _playerInput.Register(MurderInputAxis.EditorCamera,
                new InputButtonAxis(Keys.W, Keys.A, Keys.S, Keys.D),
                new InputButtonAxis(Keys.Up, Keys.Left, Keys.Down, Keys.Right));

            ImGuiRenderer = new ImGuiRenderer(this);
            ImGuiRenderer.RebuildFontAtlas();

            _logger = EditorGameLogger.OverrideInstanceWithEditor();

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

        protected override void SetWindowSize(Point screenSize)
        {
            if (_isPlayingGame)
            {
                base.SetWindowSize(screenSize);
                return;
            }

            if (!IsMaximized() && EditorSettings.WindowStartPosition.X > 0 && EditorSettings.WindowStartPosition.Y > 0)
            {
                Window.Position = EditorSettings.WindowStartPosition - new Point(-2, 0);
            }

            if (EditorSettings.WindowSize.X > 0 && EditorSettings.WindowSize.Y > 0)
            {
                _graphics.PreferredBackBufferWidth = EditorSettings.WindowSize.X;
                _graphics.PreferredBackBufferHeight = EditorSettings.WindowSize.Y;
            }

            if (EditorSettings.StartMaximized)
            {
                var titleBar = 32;
                Window.Position = new Microsoft.Xna.Framework.Point(Window.Position.X - 2, titleBar);
                //_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                //_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height - titleBar;

                MaximizeWindow();
            }
        }

        private void QuitToEditor()
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
            Resume();
            SoundPlayer.Stop(fadeOut: false, out _);

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

            for (int i = (int)GraphicsDevice.Metrics.TextureCount - 1; i >= 0; i--)
            {
                GraphicsDevice.Textures[i]?.Dispose();
            }

            GameLogger.Verify(_sceneLoader is not null);

            SaveWindowPosition();
            _isPlayingGame = true;

            ActiveScene?.RefreshWindow(GraphicsDevice, Profile);

            Data.InitializeAssets();

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
                Data.LoadSaveAsCurrentSave(slot: -1);

                LoadSceneAsync(waitForAllContent: true).Wait();
            }
            else
            {
                _isPlayingGame = false;
            }

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
                var hook = world.GetUnique<EditorComponent>().EditorHook;
                hook.DrawEntityInspector += EntityInspector.DrawInspector;
                hook.RefreshAtlas = Architect.Instance.ReloadImages;
            }
        }

        private void ReloadImages()
        {
            Data.LoadFontsAndTextures();
            EditorData.ReloadSprites();
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

                ImGui.SetNextWindowBgAlpha(0.5f);
                ImGui.Begin("Editor is not focused!", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
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

        public override void BeginImGuiTheme()
        {
            var theme = Game.Profile.Theme;
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3);
            ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 3);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 6);

            ImGui.PushStyleColor(ImGuiCol.Text, theme.White);
            ImGui.PushStyleColor(ImGuiCol.PopupBg, theme.Bg);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, theme.Bg);
            ImGui.PushStyleColor(ImGuiCol.TitleBg, theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, theme.Faded);

            ImGui.PushStyleColor(ImGuiCol.TextSelectedBg, theme.Accent);
            ImGui.PushStyleColor(ImGuiCol.ChildBg, theme.Bg);

            ImGui.PushStyleColor(ImGuiCol.PopupBg, theme.Bg);

            ImGui.PushStyleColor(ImGuiCol.Header, theme.Faded);
            ImGui.PushStyleColor(ImGuiCol.HeaderActive, theme.Accent);
            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, theme.Accent);

            ImGui.PushStyleColor(ImGuiCol.TabActive, theme.Accent);
            ImGui.PushStyleColor(ImGuiCol.TabHovered, theme.HighAccent);
            ImGui.PushStyleColor(ImGuiCol.TabUnfocused, theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.TabUnfocusedActive, theme.HighAccent);
            ImGui.PushStyleColor(ImGuiCol.Tab, theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.DockingEmptyBg, theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.DockingPreview, theme.Faded);

            ImGui.PushStyleColor(ImGuiCol.Button, theme.Foreground);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, theme.HighAccent);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, theme.Accent);

            ImGui.PushStyleColor(ImGuiCol.FrameBg, theme.BgFaded);
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, theme.Bg);
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, theme.Bg);

            ImGui.PushStyleColor(ImGuiCol.SeparatorActive, theme.Accent);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, theme.HighAccent);
        }
        public override void EndImGuiTheme()
        {
            ImGui.PopStyleColor(25);
            ImGui.PopStyleVar(3);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            GameLogger.Log("Wrapping up, bye!");

            ImGuiRenderer.AfterLayout();

            if (!_isPlayingGame) SaveWindowPosition();

            Architect.EditorData.SaveSettings();
        }

        internal void SaveWindowPosition()
        {
            if (ActiveScene is EditorScene editor)
            {
                editor.SaveEditorState();
            }
            else
            {
                GameLogger.Fail("How was this called out of an Editor scene?");
            }

            EditorSettings.WindowStartPosition = Window.Position;
            EditorSettings.WindowSize = Window.ClientBounds.Size;
            EditorSettings.StartMaximized = IsMaximized();
        }

        protected bool IsMaximized()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var windowState = SDL_GetWindowFlags(Window.Handle);
                return (windowState & SDL_WINDOW_MAXIMIZED) != 0;
            }

            return false;
        }

        protected void MaximizeWindow()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {

                var windowState = SDL_GetWindowFlags(Window.Handle);
                SDL_MaximizeWindow(Window.Handle);
            }
        }

        public void ReloadShaders()
        {
            Data.LoadShaders(breakOnFail: false, forceReload: true);

            Data.InitShaders();
            if (ActiveScene != null)
            {
                var scale = ActiveScene.RefreshWindow(_graphics.GraphicsDevice, Profile);
                ActiveScene.RenderContext?.UpdateBufferTarget(scale); // This happens twice, but it's not a big deal.
            }
        }

        public override void RefreshWindow()
        {
            var io = ImGui.GetIO();
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;
            io.FontGlobalScale = Math.Clamp(Architect.EditorSettings.FontScale, 1, 2);
            base.RefreshWindow();
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
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        protected override void Dispose(bool isDisposing)
        {
            ImGuiRenderer?.Dispose();

            base.Dispose(isDisposing);
        }
    }
}