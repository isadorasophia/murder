﻿using Bang.Components;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.CustomComponents;
using Murder.Editor.CustomEditors;
using Murder.Editor.Data;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor
{
    public partial class EditorScene : Scene
    {
        private readonly Dictionary<Guid, GameAsset> _selectedAssets = new Dictionary<Guid, GameAsset>();

        /// <summary>
        /// Asset that has just been selected and is yet to be shown.
        /// This is used to tell ImGui which tab it should open first.
        /// </summary>
        private Guid _selectedTab;
        private Guid _tabToSelect;

        private bool _isLoadingContent = true;

        /// <summary>
        /// Asset currently open and being shown.
        /// </summary>
        internal GameAsset? CurrentAsset
        {
            get
            {
                if (_selectedTab == Guid.Empty)
                    return null;

                if (_selectedAssets.TryGetValue(_selectedTab, out var value))
                    return value;
                else
                    return null;
            }
        }

        public CustomEditor? EditorShown => CurrentAsset is null ? null :
            GetOrCreateAssetEditor(CurrentAsset)?.Editor;

        public readonly Lazy<IntPtr> PreviewTexture = new(Architect.Instance.ImGuiRenderer.GetNextIntPtr);

        public static ImFontPtr EditorFont;
        public static ImFontPtr FaFont;

        /// <summary>
        /// Initialized in <see cref="Start()"/>.
        /// </summary>
        internal IList<Type> ComponentTypes = null!;

        public override MonoWorld? World => null;

        bool _f5Lock = true;
        bool _showingMetricsWindow = false;
        bool _showStyleEditor = false;
        bool _focusOnFind = false;

        public uint EDITOR_DOCK_ID = 19;

        public override void Start()
        {
            ComponentTypes = new List<Type>();
            foreach (var t in ReflectionHelper.GetAllImplementationsOf<IComponent>())
            {
                ComponentTypes.Add(t);
            }

            _f5Lock = true;

            base.Start();
        }

        private void ReopenLastTabs()
        {
            foreach (var item in Architect.EditorSettings.OpenedTabs)
            {
                if (Game.Data.TryGetAsset(item) is GameAsset asset)
                {
                    OpenAssetEditor(asset, false);
                }
            }

            // Start from asset opened in the last session
            if (Architect.EditorSettings.LastOpenedAsset is Guid tab &&
                Game.Data.TryGetAsset(tab) is GameAsset selectedAsset)
            {
                _tabToSelect = selectedAsset.Guid;
                _initializedEditors = false;
            }
        }

        public override void ReloadImpl()
        {
            _initializedEditors = false;
        }

        public override void Draw()
        {
            // We don't need to draw the world when in the editor scene
            // TODO: Pedro fix shader
            // Game.Data.SimpleShader.CurrentTechnique.Passes[0].Apply();
            Game.GraphicsDevice.SetRenderTarget(null);
            Game.GraphicsDevice.Clear(Game.Profile.Theme.Bg.ToXnaColor());
        }

        public override void DrawGui()
        {
            var screenSize = new System.Numerics.Vector2(Architect.Instance.Window.ClientBounds.Width, Architect.Instance.Window.ClientBounds.Height);

            var staticWindowFlags =
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoNav;

            float menuHeight;

            ImGui.Begin("Workspace", staticWindowFlags);

            ImGui.BeginMainMenuBar();
            {
                if (ImGui.MenuItem("Quick-Play", "Shift+F5"))
                {
                    SaveEditorState();
                    Architect.Instance.PlayGame(true);
                }

                if (ImGui.MenuItem("Play", "F5"))
                {
                    SaveEditorState();
                    Architect.Instance.PlayGame(false);
                }

                // If there is no lock, the player attempted to play the game.
                if (!_f5Lock && Game.Input.Pressed(MurderInputButtons.PlayGame))
                {
                    Architect.Instance.PlayGame(quickplay: Game.Input.Pressed(Keys.LeftShift) || Game.Input.Pressed(Keys.RightShift));
                }

                if (_f5Lock && !Game.Input.Pressed(MurderInputButtons.PlayGame))
                {
                    _f5Lock = false;
                }

                //if (ImGui.BeginMenu("Edit"))
                //{
                //    if (ImGui.MenuItem("Undo", "Ctrl+Z"))
                //    {
                //        Undo();
                //    }
                //    ImGui.EndMenu();
                //}

                if (ImGui.BeginMenu("Assets"))
                {
                    if (ImGui.MenuItem("Save All Assets", ""))
                    {
                        Architect.EditorData.SaveAllAssets();
                    }

                    if (ImGui.MenuItem("Bake Aseprite Guids", ""))
                    {
                        AsepriteServices.BakeAllAsepriteFileGuid();
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Reload"))
                {
                    if (ImGui.MenuItem("Content and Atlas", "F3"))
                    {
                        _ = Architect.EditorData.ReloadSprites();
                        AssetsFilter.RefreshCache();
                    }
                    if (ImGui.MenuItem("Shaders", "F6"))
                    {
                        Architect.Instance.ReloadShaders();
                    }

                    if (ImGui.MenuItem("Sounds", "F7"))
                    {
                        _ = Game.Data.LoadSounds(reload: true);
                    }

                    ImGui.Separator();

                    ImGui.MenuItem("Only Reload Atlas With Changes", "", ref Architect.EditorSettings.OnlyReloadAtlasWithChanges);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Util"))
                {
                    ImGui.MenuItem("Show Metrics", "", ref _showingMetricsWindow);
                    ImGui.MenuItem("Show Style Editor", "", ref _showStyleEditor);

                    if (ImGui.MenuItem("Run diagnostics", ""))
                    {
                        if (_selectedTab == Guid.Empty || !_selectedAssets.TryGetValue(_selectedTab, out GameAsset? asset))
                        {
                            GameLogger.Warning("An asset must be opened in order to run diagnostics.");
                        }
                        else
                        {
                            CustomEditorInstance? instance = GetOrCreateAssetEditor(asset);
                            if (instance?.Editor.RunDiagnostics() ?? true)
                            {
                                GameLogger.Log($"\uf00c Successfully ran diagnostics on {asset.Name}.");
                            }
                            else
                            {
                                GameLogger.Log($"\uf00d Issue found while running diagnostics on {asset.Name}.");
                            }
                        }
                    }

                    ImGui.EndMenu();
                }

                if (_showStyleEditor)
                {
                    ImGui.Begin("Style Editor", ref _showStyleEditor, ImGuiWindowFlags.AlwaysAutoResize);
                    if (ImGui.SliderFloat("Editor Scale", ref Architect.EditorSettings.FontScale, 1f, 2f))
                        ImGui.GetIO().FontGlobalScale = Math.Clamp(Architect.EditorSettings.FontScale, 1, 2);

                    ImGui.End();
                }

                if (_showingMetricsWindow)
                    ImGui.ShowMetricsWindow(ref _showingMetricsWindow);

                if (Architect.Input.Shortcut(Keys.W, Keys.LeftControl) || Architect.Input.Shortcut(Keys.W, Keys.RightControl))
                {
                    CloseTab(_selectedAssets[_selectedTab]);
                }
                if (Architect.Input.Shortcut(Keys.F, Keys.LeftControl) || Architect.Input.Shortcut(Keys.F, Keys.RightControl))
                {
                    _focusOnFind = true;
                }

                if (Architect.Input.Shortcut(Keys.F1) ||
                    (Architect.Input.Shortcut(Keys.Escape) && GameLogger.IsShowing))
                {
                    GameLogger.GetOrCreateInstance().ToggleDebugWindow();
                }
                if (Architect.Input.Shortcut(Keys.F3))
                {
                    Architect.Instance.ReloadContent();
                }
                if (Architect.Input.Shortcut(Keys.F4))
                {
                    Architect.Instance.SaveWindowPosition();
                    Architect.Instance.RefreshWindow();
                }
                if (Architect.Input.Shortcut(Keys.F6))
                {
                    Architect.Instance.ReloadShaders();
                }
                if (Architect.Input.Shortcut(Keys.F7))
                {
                    _ = Game.Data.LoadSounds(reload: true);
                }

                menuHeight = ImGui.GetItemRectSize().Y;

            }
            ImGui.EndMainMenuBar();

            ImGui.SetWindowPos(new System.Numerics.Vector2(0, 10 * ImGui.GetIO().FontGlobalScale));
            ImGui.SetWindowSize(new System.Numerics.Vector2(screenSize.X, screenSize.Y));

            ImGui.BeginChild("Workspace", new System.Numerics.Vector2(-1, -1), false);
            if (ImGui.BeginTable("Workspace", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableSetupColumn("Explorer", ImGuiTableColumnFlags.NoSort, 300f);
                ImGui.TableSetupColumn("Editor", ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.WidthStretch, -1);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                {
                    if (ImGui.BeginChild("explorer_child", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, ImGui.GetStyle().FramePadding.Y)))
                    {
                        ImGui.BeginTabBar("explorer");

                        if (ImGui.BeginTabItem(" Assets"))
                        {
                            DrawAssetsTab();
                            ImGui.EndTabItem();
                        }
                        DrawAtlasTab();
                        DrawSavesTab();

                        ImGui.EndTabBar();

                    }
                    ImGui.EndChild();
                }

                ImGui.TableNextColumn();

                if (_isLoadingContent)
                {
                    ImGui.Text("\uf256 Getting everything ready...");
                }

                {
                    if (ImGui.BeginChild("docker_child", ImGui.GetContentRegionAvail() - new System.Numerics.Vector2(0, ImGui.GetStyle().FramePadding.Y)))
                    {
                        ImGui.DockSpace(EDITOR_DOCK_ID);
                        // Draw asset editors
                        DrawAssetEditors();
                    }
                    ImGui.EndChild();
                }
                ImGui.EndTable();
            }
            ImGui.EndChild();
            ImGui.End();
        }

        private void CloseTab(GameAsset asset)
        {
            _selectedAssets.Remove(asset.Guid);
        }

        private void Undo()
        {
            throw new NotImplementedException();
        }

        public void SaveEditorState()
        {
            Architect.EditorSettings.OpenedTabs = new Guid[_selectedAssets.Count];
            int i = 0;
            foreach (var asset in _selectedAssets.Values)
            {
                Architect.EditorSettings.OpenedTabs[i++] = asset.Guid;
            }
            Architect.EditorSettings.LastOpenedAsset = CurrentAsset?.Guid;

            ((EditorDataManager)Architect.Data!).SaveAsset(Architect.EditorSettings);
        }

        private string _atlasSearchBoxTmp = string.Empty;
        private void DrawAtlasTab()
        {
            if (ImGui.BeginTabItem("Atlas"))
            {
                ImGui.SetNextItemWidth(-1);
                ImGui.InputText("##Search", ref _atlasSearchBoxTmp, 256);
                ImGui.BeginChildFrame(891237, new System.Numerics.Vector2(-1, -1));
                foreach (var atlas in Enum.GetValues(typeof(AtlasId)))
                {
                    if ((AtlasId)atlas == AtlasId.None)
                    {
                        if (ImGui.TreeNode("No Atlas"))
                        {
                            foreach (var texture in Game.Data.AvailableUniqueTextures.Where(t => t.Contains(_atlasSearchBoxTmp)))
                            {
                                ImGui.Selectable(FileHelper.GetPathWithoutExtension(texture), false);
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    Architect.ImGuiTextureManager.DrawPreviewImage(texture, 256, null);
                                    ImGui.EndTooltip();
                                }
                            }
                            ImGui.TreePop();
                        }
                    }
                    else
                    {

                        DrawAtlasImageList((AtlasId)atlas);
                    }
                }
                ImGui.EndChildFrame();
                ImGui.EndTabItem();
            }
        }

        private void DrawAtlasImageList(AtlasId atlasId)
        {
            TextureAtlas? atlas = Architect.Data.TryFetchAtlas(atlasId);
            if (atlas is null)
            {
                return;
            }

            if (ImGui.TreeNode(atlasId.GetDescription()))
            {
                foreach (var item in atlas.GetAllEntries().Where(t => t.Name.Contains(_atlasSearchBoxTmp)))
                {
                    ImGui.Selectable(item.Name);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        Architect.ImGuiTextureManager.DrawPreviewImage(item.Name, 256, atlas);
                        ImGui.EndTooltip();
                    }
                }
                ImGui.Separator();
                ImGui.TreePop();
            }
        }

        int _selectedAssetToCreate = 0;

        private string _searchAssetText = string.Empty;

        private void DrawAssetsTab()
        {
            if (Architect.EditorData.CallAfterLoadContent)
            {
                AfterContentLoaded();
            }

            lock (Architect.EditorData.AssetsLock)
            {
                IEnumerable<GameAsset> assets = Architect.EditorData.GetAllAssets()
                    .Where(asset => StringHelper.FuzzyMatch(_searchAssetText, asset.Name));

                ImGui.PushItemWidth(-1);
                if (_focusOnFind)
                {
                    _focusOnFind = false;
                    ImGui.SetKeyboardFocusHere();
                }
                ImGui.InputTextWithHint("##search_assets", "Search...", ref _searchAssetText, 256);
                ImGui.PopItemWidth();

                // Draw asset tree
                ImGui.BeginChild("");
                DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets, !string.IsNullOrWhiteSpace(_searchAssetText));

                DrawAssetInList(Architect.EditorData.EditorSettings, Game.Profile.Theme.White, Architect.EditorData.EditorSettings.Name);
                DrawAssetInList(Architect.EditorData.GameProfile, Game.Profile.Theme.White, Architect.EditorData.GameProfile.Name);

                // Button to add a new asset
                CreateAssetButton(typeof(GameAsset));

                ImGui.EndChild();
            }
        }

        private void AfterContentLoaded()
        {
            Architect.EditorData.AfterContentLoaded();
            ReopenLastTabs();

            _isLoadingContent = false;
        }

        private void DrawSavesTab()
        {
            if (ImGui.BeginTabItem("Save data"))
            {
                // Get all assets
                var assets = Architect.EditorData.GetAllSaveAssets();

                // Draw asset tree
                DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets, false);

                if (ImGuiHelpers.FadedSelectableWithIcon($"Kill all saves", '\uf54c', false))
                {
                    Architect.EditorData.DeleteAllSaves();
                }
                ImGui.PopStyleColor();
                ImGui.EndTabItem();
            }
        }

        private void DrawSelectedAtlasImage(AtlasCoordinates selectedAtlasImage)
        {
            ImGui.BeginGroup();
            {
                ImGui.Text(selectedAtlasImage.Name);
                ImGui.Image(PreviewTexture.Value, selectedAtlasImage.SourceRectangle.Size.ToVector2());
            }
            ImGui.EndGroup();
        }

        private bool DrawRenameModal(GameAsset? asset)
        {
            var closed = false;
            if (asset is not null)
            {
                if (ImGui.BeginPopup("Asset Name"))
                {
                    ImGui.SetWindowSize(new System.Numerics.Vector2(400, 100));
                    ImGui.Text("What's the new name?");

                    if (ImGui.IsWindowAppearing())
                        ImGui.SetKeyboardFocusHere();
                    ImGui.InputText("", ref _newAssetName, 64, ImGuiInputTextFlags.AutoSelectAll);

                    if (ImGui.Button("Rename") || Architect.Input.Pressed(Keys.Enter))
                    {
                        asset.Name = _newAssetName;
                        asset.Rename = true;
                        ImGui.CloseCurrentPopup();

                        closed = true;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Cancel") || Architect.Input.Pressed(Keys.Escape))
                    {
                        ImGui.CloseCurrentPopup();
                        closed = true;
                    }
                    ImGui.EndPopup();
                }
            }
            return closed;
        }
        private bool DrawDeleteModal(GameAsset asset)
        {
            var closed = false;
            if (ImGui.BeginPopup("Delete?"))
            {
                ImGui.Text("Are you sure you want to delete this asset?");
                if (ImGui.Button("Delete"))
                {
                    Architect.EditorData.RemoveAsset(asset);
                    CloseTab(asset);
                    ImGui.CloseCurrentPopup();
                    closed = true;
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                    closed = true;
                }
                ImGui.EndPopup();
            }

            return closed;
        }
        private bool DrawDiscardModal(GameAsset asset)
        {
            var closed = false;
            if (ImGui.BeginPopup("Discard?"))
            {
                ImGui.Text("Are you sure you want to discard all changes?");
                if (ImGui.Button("Discard"))
                {
                    DiscardAsset(asset);

                    ImGui.CloseCurrentPopup();
                    closed = true;
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                    closed = true;
                }
                ImGui.EndPopup();
            }

            return closed;
        }

        private void DiscardAsset(GameAsset asset)
        {
            var relativePath = asset.GetRelativePath();
            var newAsset = Game.Data.TryLoadAsset(
                FileHelper.GetPath(asset.GetEditorAssetPath()!),
                FileHelper.GetPath(relativePath.AsSpan().Slice(0, relativePath.Length - FileHelper.Clean(asset.EditorFolder).Length).ToString())
                );

            if (newAsset is not null)
            {
                Game.Data.AddAsset(newAsset, true);
                OpenAssetEditor(newAsset, true);
            }
        }

        /// <summary>
        /// Do operations once a scene has been resumed from foreground.
        /// </summary>
        public bool ReloadOnWindowForeground()
        {
            if (Architect.EditorData.ReloadDialogs())
            {
                // Hardcode to the dialog. If we need to do that more often, rethink that?
                if (_editors.TryGetValue(typeof(CharacterEditor), out CustomEditorInstance? value))
                {
                    value.Editor.ReloadEditor();
                }

                return true;
            }

            return false;
        }

        internal void ReopenTabs()
        {
            var openedGuids = _selectedAssets.Select(asset => asset.Value.Guid).ToImmutableArray();
            _selectedAssets.Clear();

            var toSelect = _tabToSelect;

            foreach (var asset in openedGuids)
            {
                if (Architect.Data.TryGetAsset(asset) is not GameAsset newAsset)
                {
                    if (Game.Profile.Guid == asset)
                        newAsset = Game.Profile;
                    else if (Architect.EditorSettings.Guid == asset)
                        newAsset = Architect.EditorSettings;
                    else
                    {
                        GameLogger.Warning($"Could not reopen asset tab {asset}");
                        _tabToSelect = toSelect;
                        return;
                    }
                }

                OpenAssetEditor(newAsset, true);
            }

            _tabToSelect = toSelect;
        }
    }
}