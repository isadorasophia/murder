using ImGuiNET;
using System.Runtime.InteropServices;
using Bang;
using Murder.Editor.Assets;
using Murder;
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

namespace Murder.Editor
{
    public class Architect : Game
    {
        /* *** Static properties of the Architect *** */

        new public static Architect Instance { get; private set; } = null!;

        public static EditorSettingsAsset EditorSettings => EditorData.EditorSettings;

        public static EditorDataManager EditorData => (EditorDataManager)Instance._gameData;

        protected override Scene InitialScene => new EditorScene();

        /* *** SDL helpers *** */

        private const string SDL = "SDL2.dll";
        private const int SDL_WINDOW_MAXIMIZED = 0x00000080;

        [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_MaximizeWindow(IntPtr window);

        [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowFlags(IntPtr window);

        /* *** Architect state *** */

        private bool _isPlayingGame = false;

        public Architect() : base(new EditorDataManager()) { }

        protected override void Initialize()
        {
            Instance = this;

            base.Initialize();
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

            GameLogger.Verify(_sceneLoader is not null);

            _isPlayingGame = false;

            // Here, let's mock what a real "quit" would do.
            // Manually unload and load all saves.
            Data.UnloadAllSaves();
            Data.LoadAllSaves();

            if (ActiveScene is GameScene)
            {
                _sceneLoader.SwitchScene<EditorScene>();

                LoadSceneAsync().Wait();
                RefreshWindow();
            }

            (_gameData as EditorDataManager)?.RefreshAfterSave();

            _playerInput.ClearBinds(InputButtons.PlayGame);
        }

        internal void PlayGame(bool quickPlay)
        {
            Resume();

            Downsample = EditorSettings.Downsample;

            for (int i = (int)GraphicsDevice.Metrics.TextureCount - 1; i >= 0; i--)
            {
                GraphicsDevice.Textures[i].Dispose();
            }
            GameLogger.Verify(_sceneLoader is not null);

            SaveWindowPosition();
            _isPlayingGame = true;

            Architect.Instance.DPIScale = Architect.EditorSettings.DPI;
            
            ActiveScene?.RefreshWindow(GraphicsDevice, Profile, EditorSettings.Downsample);

            EditorData.BuildBinContentFolder();
            Data.InitializeAssets();

            bool shouldLoad = true;
            if (quickPlay)
            {
                shouldLoad = SwitchToQuickPlayScene();
            }
            else
            {
                _sceneLoader.SwitchScene(Profile.StartingScene);
            }

            if (shouldLoad)
            {
                LoadSceneAsync().Wait();
            }
            
            _playerInput.Consume(InputButtons.PlayGame);

            _playerInput.Bind(InputButtons.PlayGame, (input) => {
                _playerInput.Consume(InputButtons.PlayGame);
                QuitToEditor();
            });
        }

        private bool SwitchToQuickPlayScene()
        {
            GameLogger.Verify(_sceneLoader is not null);

            // Handle awkward quick save loading.
            if (Data.TryGetActiveSaveData() is null)
            {
                if (Data.GetAllSaves().FirstOrDefault() is SaveData savedRun)
                {
                    Data.LoadSave(savedRun.Guid);
                }
                else
                {
                    GameLogger.Warning("Quick play currently only works on a loaded save.");
                    return false;
                }
            }

            _sceneLoader.SwitchScene(EditorSettings.QuickStartScene);
            return true;
        }

        public void ReloadContent()
        {
            GameLogger.Log("===== Reloading content! =====", Data.GameProfile.Theme.Green);
            LoadContent();
        }

        protected override void LoadContent()
        {
            var now = DateTime.Now;

            LoadContentImpl();

            GameLogger.Log($"Content loaded! I did it in {(DateTime.Now - now).Milliseconds} ms");

            LoadSceneAsync().Wait();
        }

        protected override void LoadContentImpl()
        {
            base.LoadContentImpl();
             
            // Pack assets (this will be pre-packed for the final game)
            PackAtlas();

            // Load assets, textures, content, etc
            _gameData.LoadContent();
        }

        internal static void PackAtlas()
        {
            var packTarget = FileHelper.GetPath(Path.Join(EditorSettings.AssetPathPrefix, Profile.GameAssetsContentPath));
            FileHelper.GetOrCreateDirectory(packTarget);

            // Pack the regular pixel art atlasses
            Processor.Pack(
                FileHelper.GetPath(EditorSettings.ContentSourcesPath, "/images/"),
                packTarget,
                AtlasId.Gameplay, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);


            // Pack the big images into atlasses
            Processor.Pack(
                FileHelper.GetPath(EditorSettings.ContentSourcesPath, GameDataManager.HIGH_RES_IMAGES_PATH, "generic"),
                FileHelper.GetPath(Path.Join(EditorSettings.AssetPathPrefix, Profile.GameAssetsContentPath)),
                AtlasId.Generic, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);

            Processor.Pack(
                FileHelper.GetPath(EditorSettings.ContentSourcesPath, GameDataManager.HIGH_RES_IMAGES_PATH, "main_menu"),
                packTarget,
                AtlasId.MainMenu, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);

            Processor.Pack(
                FileHelper.GetPath(EditorSettings.ContentSourcesPath, GameDataManager.HIGH_RES_IMAGES_PATH, "portraits"),
                packTarget,
                AtlasId.Portraits, !Architect.EditorSettings.OnlyReloadAtlasWithChanges);

            // Copy the really big textures to the no_atlas folder
            var outputfolder = FileHelper.GetPath(Path.Join(EditorSettings.AssetPathPrefix, Profile.GameAssetsContentPath, "no_atlas"));
            var scanFolder = FileHelper.GetPath(EditorSettings.ContentSourcesPath, GameDataManager.HIGH_RES_IMAGES_PATH, "no_atlas");
            FileHelper.DeleteContent(outputfolder, deleteRootFiles: true);
            FileHelper.GetOrCreateDirectory(outputfolder);
            foreach (var image in Directory.GetFiles(scanFolder))
            {
                var target = Path.Join(outputfolder, Path.GetRelativePath(scanFolder, image));
                File.Copy(image, target);
                GameLogger.Log($"Copied {image} to {target}");
            }
        }
        
        protected override async Task LoadSceneAsync()
        {
            GameLogger.Verify(_sceneLoader is not null);

            if (!EditorData.EditorSettings.StartOnEditor)
            {
                // Switch scene to game.
                _sceneLoader.SwitchScene(Profile.StartingScene);
                GameLogger.Log($"Game will start!");
            }

            await base.LoadSceneAsync();

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

        protected override void DrawImGuiImpl()
        {
            if (!_isPlayingGame)
            {
                // Outside of the game, also display the console.
                _logger.DrawConsole();
            }
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
                var scale = ActiveScene.RefreshWindow(_graphics.GraphicsDevice, Profile, EditorSettings.Downsample);
                ActiveScene.RenderContext?.UpdateBufferTarget(scale, EditorSettings.Downsample); // This happens twice, but it's not a big deal.
            }
        }

        public override void RefreshWindow()
        {
            Instance.DPIScale = EditorSettings.DPI;
            var io = ImGui.GetIO();
            io.FontGlobalScale = EditorSettings.DPI / 100;
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;

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
            base.Update(gameTime);

            if (Architect.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F6))
            {
                Architect.Instance.ReloadShaders();
            }
        }
    }
}
