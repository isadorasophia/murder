using Bang.Components;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Data;
using Murder.Serialization;
using Murder.ImGuiExtended;
using Murder.Utilities;
using Murder.Diagnostics;
using Murder.Editor.Utilities;
using Murder.Editor.CustomEditors;
using Murder.Editor.Data;
using Murder.Editor.CustomComponents;

namespace Murder.Editor
{
    public class EditorScene : Scene
    {
        private ImmutableArray<GameAsset> _selectedAssets = ImmutableArray<GameAsset>.Empty;

        private GameAsset? _selectedAsset = null;

        public readonly Lazy<IntPtr> PreviewTexture = new(Game.Instance.ImGuiRenderer.GetNextIntPtr);

        private string _newAssetName = "";

        public static ImFontPtr EditorFont;
        public static ImFontPtr FaFont;

        /// <summary>
        /// Initialized in <see cref="Start()"/>.
        /// </summary>
        internal IList<Type> ComponentTypes = null!;

        public override MonoWorld? World => null;

        bool _f5Lock = true;

        public uint EDITOR_DOCK_ID = 19;

        public override void Start()
        {
            ComponentTypes = new List<Type>();
            foreach (var t in ReflectionHelper.GetAllImplementationsOf<IComponent>())
            {
                ComponentTypes.Add(t);
            }

            foreach (var item in Architect.EditorSettings.OpenedTabs)
            {
                if (Game.Data.TryGetAsset(item) is GameAsset asset)
                {
                    OpenAssetEditor(asset);
                }
            }

            // Start from asset opened in the last session
            int lastSessionTab = Architect.EditorSettings.SelectedTab;
            _f5Lock = true;

            base.Start();
        }

        public override void Draw()
        {
            // We don't need to draw the world when in the editor scene
            Game.Data.SimpleShader.CurrentTechnique.Passes[0].Apply();
            Game.GraphicsDevice.SetRenderTarget(null);
            Game.GraphicsDevice.Clear(Color.Transparent);
        }

        public override void DrawGui()
        {
            Game.Instance.IsMouseVisible = true;
            var screenSize = new System.Numerics.Vector2(Architect.Instance.Window.ClientBounds.Width, Architect.Instance.Window.ClientBounds.Height);
            var assetWindowWidth = 320f * Game.Instance.DPIScale / 100f;

            var staticWindowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDocking;

            ImGui.BeginMainMenuBar();


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

            if (!_f5Lock && Game.Input.Pressed(InputButtons.PlayGame))
            {
                Architect.Instance.PlayGame(Game.Input.Pressed(Keys.LeftShift) || Game.Input.Pressed(Keys.RightShift));
            }

            if (_f5Lock && !Game.Input.Pressed(InputButtons.PlayGame))
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

            if (ImGui.BeginMenu("Reload"))
            {
                if (ImGui.MenuItem("Atlas only", "F2"))
                {
                    Architect.PackAtlas();
                    Architect.Data.RefreshAtlas();
                }
                if (ImGui.MenuItem("Content and Atlas", "F3"))
                {
                    Architect.Instance.ReloadContent();
                }
                if (ImGui.MenuItem("Window", "F4"))
                {
                    Architect.Instance.SaveWindowPosition();
                    Architect.Instance.RefreshWindow();
                }

                if (ImGui.MenuItem("Shaders", "F6"))
                {
                    Architect.Instance.ReloadContent();
                }

                ImGui.Separator();

                ImGui.MenuItem("Only Reload Atlas With Changes", "", ref Architect.EditorSettings.OnlyReloadAtlasWithChanges);

                ImGui.EndMenu();
            }


            if (ImGui.BeginMenu("Util"))
            {
                if (ImGui.MenuItem("Show Metrics"))
                {
                    ImGui.ShowMetricsWindow();
                }
                ImGui.EndMenu();
            }

            if (Architect.Input.Shortcut(Keys.F2))
            {
                Architect.PackAtlas();
                Architect.Data.RefreshAtlas();
            }
            if (Architect.Input.Shortcut(Keys.F3))
            {
                Architect.Instance.SaveWindowPosition();
                Architect.Instance.ReloadContent();
                Architect.Instance.RefreshWindow();
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

            var menuHeight = ImGui.GetItemRectSize().Y;

            ImGui.EndMainMenuBar();

            ImGui.Begin("Explorer", staticWindowFlags);
            {
                ImGui.SetWindowPos(new System.Numerics.Vector2(0, menuHeight));
                ImGui.SetWindowSize(new System.Numerics.Vector2(assetWindowWidth, screenSize.Y - menuHeight));

                ImGui.BeginTabBar("explorer");

                if (ImGui.BeginTabItem("Assets"))
                {
                    DrawAssetsTab();
                    ImGui.EndTabItem();
                }
                DrawAtlasTab();
                DrawSavesTab();

                ImGui.EndTabBar();
            }
            ImGui.End();

            ImGui.Begin("Editor", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar);
            {
                ImGui.DockSpace(EDITOR_DOCK_ID);
                ImGui.SetWindowPos(new System.Numerics.Vector2(assetWindowWidth, menuHeight));
                ImGui.SetWindowSize(new System.Numerics.Vector2(screenSize.X - assetWindowWidth, screenSize.Y - menuHeight));
            }

            // Draw asset editors
            DrawAssetEditors();

        }

        private void DrawAssetEditors()
        {
            GameAsset? closeTab = null;

            for (int i = 0; i < _selectedAssets.Length; i++)
            {
                var tab = _selectedAssets[i];
                bool open = true;

                if (_selectedAsset == tab)
                {
                    ImGui.SetNextWindowFocus();
                    _selectedAsset = null;
                }

                ImGui.SetNextWindowDockID(EDITOR_DOCK_ID, ImGuiCond.Appearing);
                ImGuiWindowFlags fileSaved = tab.FileChanged ? ImGuiWindowFlags.UnsavedDocument : ImGuiWindowFlags.None;
                if (ImGui.Begin($"{tab.GetSimplifiedName()}##{tab.Guid}", ref open, fileSaved))
                {
                    
                    DrawSelectedAsset(tab);
                    ImGui.EndTabItem();
                }

                if (!open)
                {
                    closeTab = tab;
                }
            }
            if (closeTab != null)
            {
                if (CustomEditorsHelper.TryGetCustomEditor(closeTab.GetType(), out var editor))
                {
                    if (editor is AssetEditor assetEditor)
                    {
                        assetEditor.RemoveStage(closeTab);
                    }
                }
                _selectedAssets = _selectedAssets.Remove(closeTab);
            }

            ImGui.End();
            ImGui.EndPopup();
        }

        private void Undo()
        {
            throw new NotImplementedException();
        }

        public void SaveEditorState()
        {
            Architect.EditorSettings.OpenedTabs = new Guid[_selectedAssets.Length];
            for (int i = 0; i < _selectedAssets.Length; i++)
            {
                Architect.EditorSettings.OpenedTabs[i] = _selectedAssets[i].Guid;
            }

            ((EditorDataManager)Architect.Data!).SaveAsset(Architect.EditorSettings);
        }

        private string _atlasSearchBoxTmp = string.Empty;
        private void DrawAtlasTab()
        {
            if (ImGui.BeginTabItem("Atlas"))
            {
                ImGui.SetNextItemWidth(-1);
                ImGui.InputText("##Search", ref _atlasSearchBoxTmp, 256);
                ImGui.BeginChildFrame(891237, new System.Numerics.Vector2(-1,-1));
                foreach (var atlas in Enum.GetValues(typeof(AtlasId)))
                {
                    if ((AtlasId)atlas == AtlasId.None)
                    {
                        if (ImGui.TreeNode("No Atlas"))
                        {
                            foreach (var texture in Game.Data.AvailableUniqueTextures.Where(t=>t.Contains(_atlasSearchBoxTmp)))
                            {
                                ImGui.Selectable(FileHelper.GetPathWithoutExtension(texture), false);
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGuiHelpers.Image(texture, 256, null, Architect.Instance.DPIScale / 100);
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
            var atlas = Architect.Data.FetchAtlas(atlasId);

            if (ImGui.TreeNode(atlasId.GetDescription()))
            {
                foreach (var item in atlas.GetAllEntries().Where(t => t.Name.Contains(_atlasSearchBoxTmp)))
                {
                    ImGui.Selectable(item.Name);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGuiHelpers.Image(item.Name, 256, atlas, Architect.Instance.DPIScale / 100);
                        ImGui.EndTooltip();
                    }
                }
                ImGui.Separator();
                ImGui.TreePop();
            }
        }

        int _selectedAssetToCreate = 0;

        private void DrawAssetsTab()
        {
            // Get all assets
            var assets = Architect.EditorData.GetAllAssets();

            // Draw asset tree
            DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets, 0);
            DrawAssetInList(Architect.EditorData.EditorSettings, Game.Profile.Theme.White, Architect.EditorData.EditorSettings.Name);
            DrawAssetInList(Architect.EditorData.GameProfile, Game.Profile.Theme.White, Architect.EditorData.GameProfile.Name);

            // Button to add a new asset
            CreateAssetButton(typeof(GameAsset));

            //if (ImGui.BeginPopupContextItem())
            //{
            //    if (ImGui.MenuItem("Create New Asset"))
            //    {
            //        SelectAssetIndex(0);
            //        ImGui.OpenPopup("Create Asset");
            //    }
            //    ImGui.EndPopup();
            //}
        }

        private void DrawSavesTab()
        {
            if (ImGui.BeginTabItem("Save data"))
            {
                // Get all assets
                var assets = Architect.EditorData.GetAllSaveAssets();

                // Draw asset tree
                DrawAssetFolder("#\uf07b", Architect.Profile.Theme.White, typeof(GameAsset), assets, 0);

                if (ImGuiHelpers.FadedSelectableWithIcon($"Kill all saves", '\uf54c', false))
                {
                    Architect.EditorData.DeleteAllSaves();
                }

                ImGui.EndTabItem();
            }
        }

        private void CreateAssetButton(Type type)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
            if (ImGuiHelpers.SelectableWithIcon($"", '\uf0fe', false))
            {
                _selectedAssetToCreate = 0;
                _newAssetName = String.Format(Architect.EditorSettings.NewAssetDefaultName, type.Name);
                ImGui.OpenPopup($"Create {type.Name}##Create {type.FullName}");
            }
            ImGui.PopStyleColor();

            DrawCreateAssetModal(type);
        }

        private void DrawCreateAssetModal(Type type)
        {
            if (ImGui.BeginPopup($"Create {type.Name}##Create {type.FullName}"))
            {
                var assetTypes = new List<Type>();
                var searchForType = type;
                var parent = ReflectionHelper.TryFindFirstAbstractOf(type);
                if (parent != typeof(GameAsset))
                    searchForType = parent; 

                foreach (var t in ReflectionHelper.GetEnumerableOfType<GameAsset>())
                {
                    if((searchForType == null || searchForType.IsAssignableFrom(t)) && !t.IsAbstract)
                    {
                        assetTypes.Add(t);
                    }
                }

                if (assetTypes.Count > 0)
                {
                    ImGui.Text("What's the asset type?");
                    if (ImGui.BeginCombo("", assetTypes[_selectedAssetToCreate].Name))
                    {
                        for (int i = 0; i < assetTypes.Count; i++)
                        {
                            if (ImGui.MenuItem(assetTypes[i].Name))
                            {
                                _selectedAssetToCreate = i;
                                _newAssetName = String.Format(Architect.EditorSettings.NewAssetDefaultName, type.Name);
                            }
                        }
                        ImGui.EndCombo();
                    }

                    Type createAssetOfType = assetTypes[_selectedAssetToCreate];
                    ImGui.PushID("NewNameField");
                    ImGui.InputText("", ref _newAssetName, 128, ImGuiInputTextFlags.AutoSelectAll);
                    ImGui.PopID();

                    if (!string.IsNullOrWhiteSpace(_newAssetName))
                    {
                        if (ImGui.Button("Create") || Architect.Input.Pressed(Keys.Enter))
                        {
                            _selectedAsset = Architect.EditorData.CreateNewAsset(createAssetOfType, _newAssetName.Trim());
                            GameLogger.Verify(_selectedAsset is not null);

                            OpenAssetEditor(_selectedAsset);

                            _selectedAsset.Name = _newAssetName.Trim();
                            _selectedAsset.FileChanged = true;
                            ImGui.CloseCurrentPopup();
                        }
                        ImGui.SameLine();
                    }
                }
                else
                {
                    ImGui.Text("No asset type found!\n(You should create one on the C# project)");
                }

                if (ImGui.Button("Cancel") || Architect.Input.Pressed(Keys.Escape))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        private void DrawAssetFolder(string folderName, Vector4 color, Type? createType, IEnumerable<GameAsset> assets, int depth)
        {
            var printName = folderName;
            char? icon = null;

            if (folderName.StartsWith('#'))
            {
                printName = folderName.Substring(2);
                icon = folderName[1];
            }

            var foldersToDraw = new Dictionary<string, (Vector4 color, Type? createType, List<GameAsset> assets)>();
            foreach (var asset in assets)
            {
                var folders = Path.Combine(asset.EditorFolder, asset.Name).Split('\\', '/');
                if (folders.Length > depth + 1)
                {
                    var currentFolder = folders[depth];
                    if (!foldersToDraw.ContainsKey(currentFolder))
                    {
                        // Add create asset button to the folder if necessary
                        var t = !string.IsNullOrWhiteSpace(asset.EditorFolder) && depth == 0? asset.GetType() : null;

                        foldersToDraw[currentFolder] = (asset.EditorColor, t, new List<GameAsset>());
                    }
                    foldersToDraw[currentFolder].assets.Add(asset);
                }
            }
            
            if (icon.HasValue && depth>0)
            {
                ImGuiHelpers.ColorIcon(icon.Value, color);
                ImGui.SameLine();
            }
            if (depth<=1) ImGui.PushStyleColor(ImGuiCol.Text, color);
            if (string.IsNullOrWhiteSpace(printName) || ImGui.TreeNodeEx(printName))
            {
                if (depth <= 1) ImGui.PopStyleColor();
                // TODO: Draw folders in alphabetical order
                foreach (var folder in foldersToDraw)
                {
                    if (folder.Key.StartsWith('#'))
                        DrawAssetFolder(folder.Key, folder.Value.color, folder.Value.createType, folder.Value.assets, depth + 1);
                    else
                        DrawAssetFolder($"#\uf07b{folder.Key}", folder.Value.color, folder.Value.createType, folder.Value.assets, depth + 1);

                }

                foreach (var asset in assets)
                {
                    var folders = Path.Combine(asset.EditorFolder, asset.Name).Split('\\', '/');
                    if (folders.Length > depth + 1)
                        continue;

                    DrawAssetInList(asset, color, folders[^1]);
                }


                // Draw the create asset button
                if (createType!=null && depth>0 && folderName!= "#\uf085Generated")
                    CreateAssetButton(createType);

                if (!string.IsNullOrWhiteSpace(printName))
                    ImGui.TreePop();
            }
            else
            {
                if (depth <= 1) ImGui.PopStyleColor();
            }
        }

        private void DrawAssetInList(GameAsset asset, Vector4 color, string name)
        {
            ImGui.PushID($"TabIconList {asset.Guid}");


            var selectedColor = _selectedAsset == asset ? Game.Profile.Theme.Faded : Game.Profile.Theme.BgFaded;
            ImGui.PushStyleColor(ImGuiCol.Header, selectedColor);

            if (ImGuiHelpers.SelectableWithIconColor($"{name}{(asset.FileChanged ? "*" : "")}", asset.Icon, color, color * 0.6f, _selectedAssets.Contains(asset)))
            {
                OpenAssetEditor(asset);
            }

            ImGui.PopStyleColor();

            if (ImGui.BeginPopupContextItem())
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, asset.Name);
                if (ImGui.MenuItem("Save"))
                {
                    Architect.EditorData.SaveAsset(asset);
                    ImGui.CloseCurrentPopup();
                }
                if (asset is PrefabAsset prefab && ImGui.MenuItem("Create instance"))
                {
                    string instanceName = Architect.EditorData.GetNextName($"{prefab.Name} Instance", Architect.EditorSettings.AssetNamePattern);

                    GameAsset instance = prefab.ToInstanceAsAsset(instanceName);
                    Architect.Data.AddAsset(instance);

                    ImGui.CloseCurrentPopup();
                }
                if (ImGui.MenuItem("Duplicate"))
                {
                    string duplicateName = Architect.EditorData.GetNextName(asset.Name, Architect.EditorSettings.AssetNamePattern);

                    GameAsset instance = asset.Duplicate(duplicateName);
                    Architect.Data.AddAsset(instance);

                    ImGui.CloseCurrentPopup();
                }
                if (asset.CanBeRenamed && ImGui.Selectable("Rename", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    _newAssetName = asset.Name;
                    ImGui.OpenPopup("Asset Name");
                }
                if (asset.CanBeDeleted && ImGui.Selectable("Delete", false, ImGuiSelectableFlags.DontClosePopups))
                {
                    ImGui.OpenPopup("Delete?");
                }

                if (DrawRenameModal(asset))
                    ImGui.CloseCurrentPopup();
                if (DrawDeleteModal(asset))
                    ImGui.CloseCurrentPopup();

                ImGui.EndPopup();
            }
                
            ImGui.PopID();
        }

        private void DrawSelectedAtlasImage(AtlasTexture selectedAtlasImage)
        {
            ImGui.BeginGroup();
            {
                ImGui.Text(selectedAtlasImage.Name);
                ImGui.Image(PreviewTexture.Value, selectedAtlasImage.SourceRectangle.Size.ToVector2());
            }
            ImGui.EndGroup();
        }

        private void DrawSelectedAsset(GameAsset asset)
        {
            GameLogger.Verify(RenderContext is not null);

            var assetType = asset.GetType();
            ImGui.Spacing();
            ImGui.BeginGroup();
            {
                if (asset.FileChanged)
                {
                    ImGuiHelpers.ColorIcon('\uf0c7', Game.Profile.Theme.Red);
                    ImGui.SameLine();
                }

                if (string.IsNullOrWhiteSpace(asset.FilePath))
                    ImGui.TextColored(Microsoft.Xna.Framework.Color.DarkRed.ToSysVector4(), $"(FILE NOT SAVED)");
                else
                    ImGui.TextColored(Microsoft.Xna.Framework.Color.DarkGray.ToSysVector4(), $"({asset.FilePath}){(asset.FileChanged ? "*" : "")}");
                ImGui.SameLine();
                ImGui.TextColored(Microsoft.Xna.Framework.Color.DarkGray.ToSysVector4(), $"({asset.GetType().Name})");
                ImGui.SameLine();

                if (asset.CanBeSaved && (ImGui.Button("Save Asset") || Architect.Input.Shortcut(Keys.S, Keys.LeftControl)))
                {
                    if (CustomEditorsHelper.TryGetCustomEditor(assetType, out var customEditor))
                    {
                        customEditor.PrepareForSaveAsset();
                    }

                    try
                    {
                        Architect.EditorData.SaveAsset(asset);
                    }
                    catch
                    {
                        GameLogger.Error($"Sorry, I was unable to save asset: {asset.Name} :(");
                    }
                }

                if (asset.CanBeDeleted)
                {
                    ImGui.SameLine();
                    if (ImGui.Button("Delete Asset") || Architect.Input.Shortcut(Keys.Delete, Keys.LeftControl))
                    {
                        ImGui.OpenPopup("Delete?");
                    }
                }
                
                if (asset.CanBeRenamed)
                {
                    ImGui.SameLine();
                    if (ImGui.Button("Rename") || Architect.Input.Shortcut(Keys.R, Keys.LeftControl))
                    {
                        _newAssetName = asset.Name;
                        ImGui.OpenPopup("Asset Name");
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("Open Folder"))
                {
                    if (Path.IsPathRooted(asset.FilePath) && Path.GetDirectoryName(asset.FilePath) is string directoryPath)
                    {
                        FileHelper.OpenFolder(directoryPath);
                    }
                    else
                    {
                        var rooted = FileHelper.GetPath(
                            Architect.EditorSettings.AssetPathPrefix, Game.Profile.GameAssetsContentPath,
                            asset.SaveLocation, asset.Name);
                        DirectoryInfo? path = Directory.GetParent(rooted);

                        if (path != null)
                            FileHelper.OpenFolder(path.FullName);
                        else
                            GameLogger.Error($"Couldn't parse the path for {asset.Name}");
                    }
                }

                //Draw modals
                DrawDeleteModal(asset);
                DrawRenameModal(asset);

                ImGui.EndGroup();
            }

            // Draw the custom editor
            if (CustomEditorsHelper.TryGetCustomEditor(assetType, out var editor))
            {
                if (editor is AssetEditor assetEditor)
                {
                    ImGui.SameLine();
                    bool showColliders = assetEditor.ShowColliders;
                    ImGui.Checkbox("Show Colliders", ref showColliders);

                    assetEditor.ShowColliders = showColliders;
                }

                editor.OpenEditor(Game.Instance.ImGuiRenderer, asset);
                editor.DrawEditor();
            }
            else
            {
                _ = CustomComponent.ShowEditorOf(asset);
            }
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
                        asset.FileChanged = true;
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
                if (ImGui.Button("OK"))
                {
                    Architect.EditorData.RemoveAsset(asset);
                    _selectedAssets = _selectedAssets.Remove(asset);
                    _selectedAsset = null;
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

        private void OpenAssetEditor(GameAsset asset)
        {
            GameLogger.Verify(RenderContext is not null);
            
            if (!_selectedAssets.Contains(asset))
            {
                if (_selectedAssets.FirstOrDefault(g => g.Guid == asset.Guid) is GameAsset previousInstance)
                {
                    // We might still have a tab with a different game asset that was loaded before this one changed.
                    // In that case, we will simply reassign it.
                    _selectedAssets = _selectedAssets.Replace(previousInstance, asset);
                }
                else
                {
                    _selectedAssets = _selectedAssets.Add(asset);
                }
            }

            _selectedAsset = asset;
            
            var assetType = asset.GetType();
            if (CustomEditorsHelper.TryGetCustomEditor(assetType, out var editor))
                editor.OpenEditor(Game.Instance.ImGuiRenderer, asset);
        }
    }
}
