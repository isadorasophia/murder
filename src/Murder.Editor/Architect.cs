using ImGuiNET;
using System.Runtime.InteropServices;
using Bang;
using Murder.Editor.Assets;
using Murder.Diagnostics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Input;
using Murder.Assets;
using Murder.Serialization;
using Murder.Data;
using Murder.Editor.Data;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Diagnostics;
using Murder.Services;
using System.Diagnostics;
using Murder.Editor.EditorCore;

namespace Murder.Editor
{
    public class Architect : Game
    {
        /* *** Static properties of the Architect *** */

        new public static Architect Instance { get; private set; } = null!;

        public static EditorSettingsAsset EditorSettings => EditorData.EditorSettings;

        public static EditorDataManager EditorData => (EditorDataManager)Instance._gameData;

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

        public CursorStyle Cursor { get; set; } = CursorStyle.Normal;

        protected override bool HasCursor => true;

        public Architect(IMurderArchitect? game = null) : base(game, new EditorDataManager(game)) { }

        protected override void Initialize()
        {
            Instance = this;

            ImGuiRenderer = new ImGuiRenderer(this);
            ImGuiRenderer.RebuildFontAtlas();

            _logger = EditorGameLogger.OverrideInstanceWithEditor();

            base.Initialize();
            ImGuiRenderer.InitTheme();
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
                Window.Position = new Microsoft.Xna.Framework.Point(Window.Position.X-2, titleBar);
                //_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                //_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height - titleBar;

                MaximizeWindow();
            }
        }

        private void QuitToEditor()
        {
            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
            Resume();
            SoundPlayer.Stop(fadeOut: true);

            GameLogger.Verify(_sceneLoader is not null);

            _isPlayingGame = false;

            // Here, let's mock what a real "quit" would do.
            // Manually unload and load all saves.
            Data.UnloadAllSaves();
            Data.LoadAllSaves();

            if (ActiveScene is GameScene)
            {
                Debug.Assert(_editorScene is not null);

                _sceneLoader.SwitchScene(_editorScene);

                // Manually set things up in the editor scene.
                _editorScene.Reload();

                // RefreshWindow();
            }

            (_gameData as EditorDataManager)?.RefreshAfterSave();

            _playerInput.ClearBinds(MurderInputButtons.PlayGame);
        }
        
        internal void PlayGame(bool quickplay, Guid? startingScene = null)
        {
            startingScene ??= Profile.StartingScene;

            // Data.ResetActiveSave();

            if (!quickplay && startingScene == Guid.Empty)
            {
                GameLogger.Error("Unable to start the game, please specify a valid starting scene on \"Game Profile\".");
                return;
            }

            if (Game.Data.TryGetAsset<WorldAsset>(startingScene.Value) is WorldAsset world && 
                !world.HasSystems)
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

            EditorData.BuildBinContentFolder();
            Data.InitializeAssets();

            bool shouldLoad = true;
            if (quickplay)
            {
                shouldLoad = SwitchToQuickPlayScene();
            }
            else
            {
                _sceneLoader.SwitchScene(startingScene.Value);
            }

            if (shouldLoad)
            {
                // Make sure we load the save before playing the game.
                Data.LoadSaveAsCurrentSave();

                LoadSceneAsync(waitForAllContent: true).Wait();
            }
            else
            {
                _isPlayingGame = false;
            }
            
            _playerInput.Consume(MurderInputButtons.PlayGame);

            _playerInput.Bind(MurderInputButtons.PlayGame, (input) => {
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
                if (!Data.LoadSaveAsCurrentSave())
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
            GameLogger.Log("Saving current editor settings", Data.GameProfile.Theme.Green);
            EditorData.SaveAsset(EditorData.EditorSettings);
            foreach (var texture in Data.CachedUniqueTextures)
            {
                texture.Value.Dispose();
            }
            Data.CachedUniqueTextures.Clear();
            _gameData.Init();
            LoadContent();
            if (ActiveScene is EditorScene editor)
            {
                editor.ReopenTabs();
            }
        }

        protected override void LoadContent()
        {
            var now = DateTime.Now;

            LoadContentImpl();

            GameLogger.Log($"Content loaded! I did it in {(DateTime.Now - now).Milliseconds} ms");

            LoadSceneAsync(waitForAllContent: false).Wait();
        }

        protected override void LoadContentImpl()
        {
            base.LoadContentImpl();
             
            // Pack assets (this will be pre-packed for the final game)
            PackAtlas();

            // Save sounds to the packed folder
            SoundServices.StopAll();
            PackSounds();

            // Load assets, textures, content, etc
            _gameData.LoadContent();
        }

        private void PackSounds()
        {
            if (!Directory.Exists(FileHelper.GetPath(EditorSettings.GameSourcePath)))
            {
                GameLogger.Warning($"Please specify a valid \"Game Source Path\" in \"Editor Settings\". Unable to find the resources to build the atlas from.");
                return;
            }

            string soundRawResourcesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, Data.GameProfile.SoundsPath);
            if (!Directory.Exists(soundRawResourcesPath))
            {
                // There are no sounds to pack!
                return;
            }

            string binDirectory = FileHelper.GetPath(EditorSettings.BinResourcesPath, Data.GameProfile.SoundsPath);
            FileHelper.DeleteDirectoryIfExists(binDirectory);
            FileHelper.DirectoryDeepCopy(soundRawResourcesPath, binDirectory);
        }

        internal static void PackAtlas()
        {
            Architect.Data.DisposeAtlases();

            // Cleanup generated assets folder
            FileHelper.DeleteDirectoryIfExists(FileHelper.GetPath(Path.Join(Game.Profile.GenericAssetsPath, "Generated")));

            if (!Directory.Exists(FileHelper.GetPath(EditorSettings.GameSourcePath)))
            {
                GameLogger.Warning($"Please specify a valid \"Game Source Path\" in \"Editor Settings\". Unable to find the resources to build the atlas from.");
                return;
            }

            string sourcePackedTarget = FileHelper.GetPath(EditorSettings.SourcePackedPath);
            if (!Directory.Exists(sourcePackedTarget))
            {
                GameLogger.Warning($"Didn't find packed folder. Creating one.");
                FileHelper.GetOrCreateDirectory(sourcePackedTarget);
            }

            string binPackedTarget = FileHelper.GetPath(EditorSettings.BinResourcesPath);
            FileHelper.GetOrCreateDirectory(binPackedTarget);

            string editorImagesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, "/editor/");
            Processor.Pack(editorImagesPath, sourcePackedTarget, binPackedTarget,
                AtlasId.Editor, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);

            // Pack the regular pixel art atlasses
            string rawImagesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath, "/images/");
            Processor.Pack(rawImagesPath, sourcePackedTarget, binPackedTarget,
                AtlasId.Gameplay, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);

            // Copy the lost textures to the no_atlas folder
            var noAtlasRawResourceDirecotry = FileHelper.GetPath(EditorSettings.RawResourcesPath, "no_atlas");
            if (!Directory.Exists(noAtlasRawResourceDirecotry))
            {
                return;
            }
            
            string sourceNoAtlasPath = FileHelper.GetPath(Path.Join(EditorSettings.SourceResourcesPath, "/images/"));
            FileHelper.DeleteContent(sourceNoAtlasPath, deleteRootFiles: true);
            FileHelper.GetOrCreateDirectory(sourceNoAtlasPath);

            foreach (var image in Directory.GetFiles(noAtlasRawResourceDirecotry))
            {
                var target = Path.Join(sourceNoAtlasPath, Path.GetRelativePath(noAtlasRawResourceDirecotry, image));
                File.Copy(image, target);

                // GameLogger.Log($"Copied {image} to {target}");
            }

            // Make sure we are sendind this to the bin folder!
            string noAtlasImageBinPath = FileHelper.GetPath(Path.Join(EditorSettings.BinResourcesPath, "/images/"));
            FileHelper.DirectoryDeepCopy(sourceNoAtlasPath, noAtlasImageBinPath);
        }
        
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
            PackAtlas();
            Data.RefreshAtlas();
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
        public override void EndImGuiTheme(){
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
                return  (windowState & SDL_WINDOW_MAXIMIZED) != 0;
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
            Data.LoadShaders(false);
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
            io.FontGlobalScale = Math.Clamp(Architect.EditorSettings.FontScale,1,2);
            base.RefreshWindow();
        }

        public override void ExitGame()
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
            Input.MouseConsumed = ImGui.GetIO().WantCaptureMouse && _isPlayingGame;
            base.Update(gameTime);

            if (Architect.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F6))
            {
                Architect.Instance.ReloadShaders();
            }

            UpdateCursor();
        }

        private void UpdateCursor()
        {
            EditorData.CursorTextureManager?.RenderCursor(Cursor);
        }

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
