using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.CustomEditors;
using Murder.Editor.Data;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor
{
    public partial class EditorScene : Scene
    {
        private readonly Dictionary<Guid, GameAsset> _selectedAssets = [];

        /// <summary>
        /// Asset that has just been selected and is yet to be shown.
        /// This is used to tell ImGui which tab it should open first.
        /// </summary>
        private Guid _selectedTab;
        private Guid _tabToSelect;
        private int _randomCrow = 0;

        private bool _isLoadingContent = true;
        int _changingScenesLock = 3;

        public Guid CurrentSelectedAsset => _selectedTab;

        public EditorScene()
        {
            _explorerPages = CreateExplorerPages();

            _lastTimeSavedEditorSettings = Game.UnscaledDeltaTime;

            _hoverColor = Game.Profile.Theme.HighAccent;
            _selectedColor = Game.Profile.Theme.Accent;
            _normalColor = Game.Profile.Theme.BgFaded;
            _selectedExplorerWindow = _explorerPages.First();

            _shortcuts = CreateShortcutList();
            _shortcutSearchValues = _shortcuts
                .Keys.SelectMany(group => _shortcuts[group].Select(shortcut => (group, shortcut)))
                .ToDictionary(tuple => $"{tuple.group} > {tuple.shortcut.Name}", tuple => tuple.shortcut);

            _shortcutSearchValuesCache = new Lazy<Dictionary<string, Shortcut>>(() =>
            {
                Dictionary<string, Shortcut> all = new();

                // Add _shortcutSearchValues to the dictionary
                foreach (var (key, value) in _shortcutSearchValues)
                {
                    all[key] = value;
                }

                return all;
            });

            FileDropEXT.DropFile += FileDropped;
        }

        protected void FileDropped(string path)
        {
            Architect.EditorData.OnFileDrop(path);
        }

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

        public override MonoWorld? World => null;

        bool _showingImguiDemoWindow = false;
        bool _showingMetricsWindow = false;
        bool _showStyleEditor = false;
        bool _focusOnFind = false;

        public uint EDITOR_DOCK_ID = 19;

        public override void Start()
        {
            _randomCrow = new Random(DateTime.Now.Millisecond).Next(3);
            _changingScenesLock = 3;

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

        public override void Update()
        {
            base.Update();

            UpdateSelectedEditor();
            UpdateShortcuts();
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
            var screenSize = new Vector2(Architect.Instance.Window.ClientBounds.Width, Architect.Instance.Window.ClientBounds.Height) / Architect.EditorSettings.DpiScale;

            var staticWindowFlags =
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoNav;

            ImGui.Begin("Workspace", staticWindowFlags);
            {
                DrawMainMenuBar();

                if (_showStyleEditor)
                {
                    ImGui.Begin("Style Editor", ref _showStyleEditor, ImGuiWindowFlags.AlwaysAutoResize);
                    if (ImGui.InputFloat("Dpi Scale", ref Architect.EditorSettings.DpiScale, 0.25f, 1f))
                    {

                    }

                    var io = ImGui.GetIO();
                    float fontScale = io.FontGlobalScale;
                    if (ImGui.InputFloat("Font Scale", ref fontScale, 0.25f, 1f))
                    {
                        io.FontGlobalScale = fontScale;
                    }

                    ImGui.End();
                }

                if (_showingImguiDemoWindow)
                    ImGui.ShowDemoWindow(ref _showingImguiDemoWindow);


                if (_showingMetricsWindow)
                    ImGui.ShowMetricsWindow(ref _showingMetricsWindow);

                ImGui.SetWindowPos(new Vector2(0, 10 * ImGui.GetIO().FontGlobalScale));
                ImGui.SetWindowSize(new Vector2(screenSize.X, screenSize.Y));

                ImGui.BeginChild("Workspace", new Vector2(-1, -1), ImGuiChildFlags.None);

                bool shouldDrawExplorerColumn = _selectedExplorerWindow is not null;

                if (ImGui.BeginTable("Workspace", 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit))
                {
                    float explorerColumnWidth = shouldDrawExplorerColumn ? ExplorerDefaultWidth : 0;
                    float editorSize = ImGui.GetContentRegionAvail().X - ExplorerIconsColumnWidth - explorerColumnWidth;
                    ImGuiTableColumnFlags explorerColumnFlags = ImGuiTableColumnFlags.WidthStretch | (shouldDrawExplorerColumn
                        ? ImGuiTableColumnFlags.NoSort
                        : ImGuiTableColumnFlags.Disabled);

                    ImGui.TableSetupColumn("Tab List", ImGuiTableColumnFlags.NoResize, ExplorerIconsColumnWidth);
                    ImGui.TableSetupColumn("Explorer", explorerColumnFlags, -1);
                    ImGui.TableSetupColumn("Editor", ImGuiTableColumnFlags.WidthStretch, editorSize);

                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    {
                        DrawExplorerIcons();
                    }

                    DrawExplorerIfNeeded();

                    ImGui.TableNextColumn();

                    if (_isLoadingContent)
                    {
                        ImGui.Text("\uf256 Getting everything ready...");
                    }

                    ImDrawListPtr draw;
                    ImGui.BeginChild("docker_child", ImGui.GetContentRegionAvail() - new Vector2(0, ImGui.GetStyle().FramePadding.Y));
                    {
                        if (_selectedAssets.Count > 0)
                        {
                            ImGui.DockSpace(EDITOR_DOCK_ID, new Vector2(-1, -1), ImGuiDockNodeFlags.None);
                        }
                        draw = ImGui.GetWindowDrawList();

                        // Draw asset editors
                        // This is where the World, Prefabs and other assets are edited
                        DrawAssetEditors();
                    }
                    ImGui.EndChild();

                    Vector2 min = ImGui.GetWindowPos();
                    Vector2 size = ImGui.GetWindowSize();
                    Vector2 pixelSize = new Vector2(96, 80);
                    float ratio = Math.Clamp(Math.Min((size.X / pixelSize.X), (size.Y / pixelSize.Y)), 1, 2);
                    Vector2 finalSize = pixelSize * ratio;
                    Rectangle rectangle = Rectangle.CenterRectangle(min + (size) / 2f, finalSize.X, finalSize.Y);

                    if (!_isLoadingContent)
                    {
                        if (Architect.EditorData.ImGuiTextureManager.GetEditorImage("crow_0000", "empty_canvas", Architect.EditorSettings.DpiScale > 1 ? 2 : 1) is nint emptyImage)
                        {
                            draw.AddImage(emptyImage, rectangle.TopLeft, rectangle.BottomRight, Vector2.Zero, Vector2.One, Color.ToUint(new Vector4(1, 1, 1, .8f)));
                        }
                    }
                    else
                    {
                        int frame = _randomCrow * 2 + ((Game.NowUnscaled % 0.5f) > 0.25f ? 1 : 2);
                        string frameName = $"crow_{frame:0000}";
                        if (Architect.EditorData.ImGuiTextureManager.GetEditorImage(frameName, $"loading_{frame}", Architect.EditorSettings.DpiScale > 1 ? 2 : 1) is nint emptyImage)
                        {
                            draw.AddImage(emptyImage, rectangle.TopLeft, rectangle.BottomRight, Vector2.Zero, Vector2.One, Color.ToUint(new Vector4(1, 1, 1, 1f)));
                        }
                    }
                    ImGui.EndTable();
                }
                ImGui.EndChild();
            }
            ImGui.End(); // Worspace

        }

        private void CloseTab(GameAsset asset)
        {
            _selectedAssets.Remove(asset.Guid);
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
            Architect.EditorData.SaveAsset(Architect.EditorSettings);

            _lastTimeSavedEditorSettings = Game.NowUnscaled;
        }

        private string _atlasSearchBoxTmp = string.Empty;
        private void DrawAtlasWindow()
        {
            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##Search", ref _atlasSearchBoxTmp, 256);
            ImGui.BeginChild(891237, new System.Numerics.Vector2(-1, -1));
            foreach (var atlas in Enum.GetValues(typeof(AtlasId)))
            {
                if ((AtlasId)atlas == AtlasId.None)
                {
                    if (ImGui.TreeNode("No Atlas"))
                    {
                        foreach (var texture in Architect.EditorData.AvailableUniqueTextures.Where(t => t.Contains(_atlasSearchBoxTmp)))
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
            ImGui.EndChild();
        }

        private int _selectedAtlasIndex = -1;
        private AtlasId? _inspectingAtlas;
        private void DrawAtlasImageList(AtlasId atlasId)
        {
            TextureAtlas? atlas = Architect.Data.TryFetchAtlas(atlasId);
            if (atlas is null)
            {
                return;
            }

            if (ImGui.TreeNode(atlasId.GetDescription()))
            {
                ImGui.PushStyleColor(ImGuiCol.Text, Architect.Profile.Theme.HighAccent);
                if (ImGui.Selectable($"Inspect {atlas.Name}"))
                {
                    _inspectingAtlas = atlasId;
                }
                ImGui.PopStyleColor();

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

            if (_inspectingAtlas == atlasId)
            {
                bool open = true;
                if (ImGui.Begin($"InspectAtlas", ref open))
                {
                    using (var table = new ImGuiExtended.TableTwoColumns("AtlasInfo"))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();

                        ImGui.Text(atlas.Name);
                        ImGui.Separator();
                        ImGui.Text($"TextureCount: {atlas.Textures.Length}");

                        if (atlas.Textures.FirstOrDefault() is Microsoft.Xna.Framework.Graphics.Texture2D texture)
                        {
                            ImGui.Text($"Size: {texture.Bounds.Size()}");
                        }
                        ImGui.Separator();
                        for (int i = 0; i < atlas.Textures.Length; i++)
                        {
                            if (ImGui.Selectable($"Index: {i}", _selectedAtlasIndex == i))
                            {
                                _selectedAtlasIndex = i;
                            }
                        }

                        ImGui.TableNextColumn();
                        if (_selectedAtlasIndex >= 0 && _selectedAtlasIndex < atlas.Textures.Length)
                        {
                            var selectedAtlas = atlas.Textures[_selectedAtlasIndex];
                            if (Architect.ImGuiTextureManager.GetImage(selectedAtlas, $"atlas-{atlasId}-{_selectedAtlasIndex}") is nint imagePointer)
                            {
                                Vector2 availableSpace = ImGui.GetContentRegionAvail();
                                float aspect = selectedAtlas.Bounds.Width / (float)selectedAtlas.Bounds.Height;
                                if (aspect > 1)
                                {
                                    ImGui.Image(imagePointer, new Vector2(availableSpace.X, availableSpace.X / aspect));
                                }
                                else
                                {
                                    ImGui.Image(imagePointer, new Vector2(availableSpace.Y * aspect, availableSpace.Y));
                                }
                            }
                        }
                    }
                    ImGui.End();

                    if (!open)
                    {
                        _inspectingAtlas = null;
                    }
                }
            }
        }


        int _selectedAssetToCreate = 0;

        private string _searchAssetText = string.Empty;

        private void DrawAssetsWindow()
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

                if (!string.IsNullOrEmpty(_searchAssetText) || _folders?.Count == 1)
                {
                    // disable caching for ctrl-f to pick up.
                    lock (_foldersLock)
                    {
                        _folders = null;
                    }

                    _clearedFoldersOnSearch = false;
                }

                if (!_clearedFoldersOnSearch && string.IsNullOrEmpty(_searchAssetText))
                {
                    lock (_foldersLock)
                    {
                        _folders = null;
                    }

                    _clearedFoldersOnSearch = true;
                }

                if (ImGui.Button(""))
                {
                    _colapseAll = true;
                }
                ImGuiHelpers.HelpTooltip("Collaps all folders");

                ImGui.Separator();

                // Draw asset tree
                ImGui.BeginChild("");

                // Extra padding on the left
                ImGui.Dummy(new Vector2(0,0));
                ImGui.SameLine();
                
                ImGui.BeginGroup();
                if (Architect.EditorSettings.FavoriteAssets.Any())
                {
                    ImGui.BeginGroup();

                    ImGui.TextColored(Architect.Profile.Theme.Yellow, "\uf005");
                    ImGui.SameLine();
                    if (ImGui.TreeNode("Favourites"))
                    {
                        foreach (var asset in Architect.EditorSettings.CachedFavoriteAssets)
                        {
                            DrawAssetInList(asset, Architect.Profile.Theme.Yellow, asset.Name);
                        }
                    ImGui.TreePop();    
                    }
                    ImGui.EndGroup();

                    ImGui.Separator();
                }

                DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets, !string.IsNullOrWhiteSpace(_searchAssetText));

                DrawAssetInList(Architect.EditorData.EditorSettings, Game.Profile.Theme.White, Architect.EditorData.EditorSettings.Name);
                DrawAssetInList(Architect.EditorData.GameProfile, Game.Profile.Theme.White, Architect.EditorData.GameProfile.Name);

                _colapseAll = false;
                _expandTo.Clear();

                // Button to add a new asset
                CreateAssetButton(typeof(GameAsset));
                ImGui.EndGroup();
                ImGui.EndChild();
            }
        }

        private void AfterContentLoaded()
        {
            Architect.EditorData.AfterContentLoaded();
            ReopenLastTabs();

            _isLoadingContent = false;
        }

        private void DrawSavesWindow()
        {
            // Get all assets
            var assets = Architect.EditorData.GetAllSaveAssets();

            lock (_foldersLock)
            {
                // Make sure we always clean up the cache here.
                _folders = null;
            }

            // Draw asset tree
            DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets, false);

            if (ImGuiHelpers.FadedSelectableWithIcon($"Kill all saves", '\uf54c', false))
            {
                Architect.EditorData.DeleteAllSaves();

                lock (_foldersLock)
                {
                    _folders = null;
                }
            }

            ImGui.PopStyleColor();
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
        /// Typically called after a hot reload action that requires a refresh.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ReloadEditorsOfType<T>()
        {
            if (_editors.TryGetValue(typeof(T), out CustomEditorInstance? value))
            {
                value.Editor.ReloadEditor();
            }
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