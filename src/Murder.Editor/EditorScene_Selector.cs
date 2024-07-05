using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Attributes;
using Murder.Data;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Murder.Editor
{
    public partial class EditorScene
    {
        private string _newAssetName = string.Empty;
        private Guid _openAsset;

        private bool _colapseAll = false;
        private readonly List<string> _expandTo = new List<string>();
        private readonly Regex _nonAlphaNumeric = new Regex("[^a-zA-Z0-9 -]");

        private void DrawCreateAssetModal(Type type, string? path)
        {
            if (ImGui.BeginPopup(CreatePopupAssetForType(type, path)))
            {
                var assetTypes = new List<Type>();
                var searchForType = type;
                var parent = ReflectionHelper.TryFindFirstAbstractOf(type);
                if (parent != typeof(GameAsset))
                    searchForType = parent;

                foreach (var t in ReflectionHelper.GetAllImplementationsOf<GameAsset>())
                {
                    if ((searchForType is null || searchForType.IsAssignableFrom(t)) && 
                        !t.IsAbstract && 
                        !Attribute.IsDefined(t, typeof(HideInEditorAttribute)))
                    {
                        assetTypes.Add(t);
                    }
                }

                if (assetTypes.Count > 0)
                {
                    if (assetTypes.Count > _selectedAssetToCreate)
                    {
                        ImGui.Text("What's the asset type?");
                        if (ImGui.BeginCombo("", assetTypes[_selectedAssetToCreate].Name))
                        {
                            for (int i = 0; i < assetTypes.Count; i++)
                            {
                                if (ImGui.MenuItem(assetTypes[i].Name))
                                {
                                    _selectedAssetToCreate = i;
                                }
                            }

                            ImGui.EndCombo();
                        }
                    }
                    else
                    {
                        _selectedAssetToCreate = 0;
                    }

                    Type createAssetOfType = assetTypes[_selectedAssetToCreate];
                    ImGui.PushID("NewNameField");
                    ImGui.InputText("", ref _newAssetName, 128, ImGuiInputTextFlags.AutoSelectAll);
                    ImGui.PopID();

                    if (!string.IsNullOrWhiteSpace(_newAssetName))
                    {
                        if (createAssetOfType.GetConstructor(Type.EmptyTypes) != null)
                        {
                            if (ImGui.Button("Create") || Architect.Input.Pressed(Keys.Enter))
                            {
                                GameAsset asset = Architect.EditorData.CreateNewAsset(createAssetOfType, _newAssetName.Trim());

                                if (!string.IsNullOrEmpty(path))
                                {
                                    Regex escapeRegex = new(@"#.|[^\x00-\x7F]");

                                    string gameAssetPath = escapeRegex.Replace(asset.EditorFolder, string.Empty);
                                    path = escapeRegex.Replace(path, string.Empty);

                                    if (!gameAssetPath.Contains(path, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        string name = $"{path}{Path.DirectorySeparatorChar}{asset.Name}";

                                        asset.Name = AssetsFilter.GetValidName(createAssetOfType, name);
                                    }
                                }


                                _selectedTab = OpenAssetEditor(asset, false);
                                if (CurrentAsset != null)
                                {
                                    CurrentAsset.FileChanged = true;
                                }

                                ImGui.CloseCurrentPopup();
                            }
                        }
                        else
                        {
                            ImGuiHelpers.DisabledButton("Create");
                            ImGuiHelpers.HelpTooltip("No generic constructor found for this asset");
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

        private void DrawAssetFolder(string folderName, Vector4 color, Type? createType, IEnumerable<GameAsset> assets, bool unfoldAll) =>
            DrawAssetFolder(folderName, color, createType, assets, 0, string.Empty, unfoldAll);

        private Dictionary<string, IEnumerable<(string folder, Vector4 color, Type? createType, List<GameAsset> assets)>>? _folders = null;

        private readonly object _foldersLock = new();

        /// <summary>
        /// Clear the cache on search.
        /// </summary>
        private bool _clearedFoldersOnSearch = true;

        private void DrawAssetFolder(string folderName, Vector4 color, Type? createType, IEnumerable<GameAsset> assets, int depth, string folderRootPath, bool unfoldAll)
        {
            if (folderName.StartsWith(GameDataManager.SKIP_CHAR) || folderName.StartsWith("_"))
            {
                // Skip folders that start with "_".
                return;
            }

            string printName = GetFolderPrettyName(folderName, out char? icon);

            IEnumerable<(string folder, Vector4 color, Type? createType, List<GameAsset> assets)>? subfolders;
            lock (_foldersLock)
            {
                if (_folders is null)
                {
                    _folders = [];
                }

                // Initialize folders if necessary up to this point...
                if (!_folders.TryGetValue(folderName, out subfolders))
                {
                    Dictionary<string, (Vector4 color, Type? createType, List<GameAsset> assets)> builder = [];

                    foreach (GameAsset asset in assets)
                    {
                        string[] folders = asset.GetSplitNameWithEditorPath();
                        if (folders.Length > depth + 1)
                        {
                            string currentFolder = folders[depth];
                            if (!builder.TryGetValue(currentFolder, out var folderInfo))
                            {
                                // Add create asset button to the folder if necessary
                                Type t = asset.GetType();

                                folderInfo = (asset.EditorColor, t, []);
                                builder[currentFolder] = folderInfo;
                            }

                            folderInfo.assets.Add(asset);
                        }
                    }

                    foreach (var s in builder)
                    {
                        s.Value.assets.Sort((x, y) => x.Name.CompareTo(y.Name));
                    }

                    subfolders =
                        builder.OrderBy(kv => GetFolderPrettyName(kv.Key, out _)).Select(kv => (kv.Key, kv.Value.color, kv.Value.createType, kv.Value.assets));

                    _folders[folderName] = subfolders;
                }
            }

            if (icon.HasValue && depth > 0)
            {
                ImGuiHelpers.ColorIcon(icon.Value, color);
                ImGui.SameLine();
            }

            if (depth <= 1) ImGui.PushStyleColor(ImGuiCol.Text, color);

            string currentDirectoryPath = depth < 2 ? string.Empty : string.IsNullOrEmpty(folderRootPath) ? printName : $"{folderRootPath}/{printName}";
            if (_expandTo.Count > 0 &&
                _nonAlphaNumeric.Replace(_expandTo[0], "").Equals(_nonAlphaNumeric.Replace(printName,""), StringComparison.InvariantCultureIgnoreCase))
            {
                    ImGui.SetNextItemOpen(true);
                    _expandTo.RemoveAt(0);
            }
            else if (_colapseAll)
            {
                ImGui.SetNextItemOpen(false);
            }

            bool isFolderOpened = string.IsNullOrWhiteSpace(printName) || ImGui.TreeNodeEx(printName,
                ((unfoldAll && (printName != "Generated")) ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None));
            

            if (createType is not null && printName != "Generated")
            {
                DrawAssetContextMenu(createType, folderPath: currentDirectoryPath);
            }

            if (depth <= 1) ImGui.PopStyleColor();

            if (createType is not null && createType != typeof(GameAsset))
            {
                DrawCreateAssetModal(createType, path: currentDirectoryPath);
            }

            if (isFolderOpened)
            {
                foreach ((string folder, Vector4 folderColor, Type? folderCreateType, List<GameAsset> folderAssets) in subfolders)
                {
                    if (folder.StartsWith(GameAsset.SkipDirectoryIconCharacter))
                    {
                        DrawAssetFolder(folder, folderColor, folderCreateType, folderAssets, depth + 1, currentDirectoryPath, unfoldAll);
                    }
                    else
                    {
                        DrawAssetFolder(GetNameWithDirectoryIcon(folder), folderColor, folderCreateType, folderAssets, depth + 1, currentDirectoryPath, unfoldAll);
                    }
                }

                foreach (GameAsset asset in assets)
                {
                    string[] folders = asset.GetSplitNameWithEditorPath();
                    if (folders.Length > depth + 1)
                    {
                        continue;
                    }

                    DrawAssetInList(asset, color, folders[^1]);
                }

                if (!string.IsNullOrWhiteSpace(printName))
                {
                    ImGui.TreePop();
                }
            }
        }

        private static string CreatePopupAssetForType(Type t, string? path) => $"Create {t.Name}_{path}##Create {t.FullName}";

        private void DrawAssetContextMenu(Type type, string? folderPath = null)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);

            bool shouldOpenPopUp = false;
            if (ImGui.BeginPopupContextItem($"context_create_{type.Name}"))
            {
                string name = type == typeof(GameAsset) ?
                    "asset (pick one!)" :
                    Prettify.FormatAssetName(type.Name);

                if (ImGui.Selectable($"Create new {name}"))
                {
                    shouldOpenPopUp = true;
                }

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();

            if (shouldOpenPopUp)
            {
                OpenPopupForCreateAsset(type, folderPath);
            }
        }

        private void OpenPopupForCreateAsset(Type t, string? path)
        {
            ImGui.OpenPopup(CreatePopupAssetForType(t, path));

            _newAssetName = string.Format(
                Architect.EditorSettings.NewAssetDefaultName,
                t == typeof(GameAsset) ? "asset" : Prettify.FormatAssetName(t.Name));
        }

        private void DrawAssetInList(GameAsset asset, Vector4 color, string name)
        {
            ImGui.PushID($"TabIconList {asset.Guid}");

            var selectedColor = CurrentAsset == asset ? Game.Profile.Theme.Faded : Game.Profile.Theme.BgFaded;
            ImGui.PushStyleColor(ImGuiCol.Header, selectedColor);

            bool isFavourite = Architect.EditorSettings.FavoriteAssets.Contains(asset.Guid);
            if (isFavourite)
            {
                ImGui.SetCursorPosX(ImGui.GetCursorPosX()- 22);
                ImGui.TextColored(Game.Profile.Theme.Yellow, "\uf005");
                ImGui.SameLine();
            }

            if (ImGuiHelpers.SelectableWithIconColor($"{name}{(asset.FileChanged ? "*" : "")}", asset.Icon, color, color * 0.8f, _selectedAssets.ContainsKey(asset.Guid)))
            {
                _openAsset = asset.Guid;
                OpenAssetEditor(asset, false);
            }

            ImGui.PopStyleColor();

            if (ImGui.BeginPopupContextItem())
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, asset.Name);
                ImGui.Separator();

                if (ImGui.MenuItem("Open"))
                {
                    OpenAssetEditor(asset, false);
                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.MenuItem("Save"))
                {
                    Architect.EditorData.SaveAsset(asset);
                    ImGui.CloseCurrentPopup();
                }
                if (asset is PrefabAsset prefab && ImGui.MenuItem("Create instance"))
                {
                    string instanceName = Architect.EditorData.GetNextName(typeof(PrefabAsset), $"{prefab.Name} Instance", Architect.EditorSettings.AssetNamePattern);

                    GameAsset instance = prefab.ToInstanceAsAsset(instanceName);
                    Architect.Data.AddAsset(instance);

                    ImGui.CloseCurrentPopup();
                }
                if (ImGui.MenuItem("Duplicate"))
                {
                    string duplicateName = Architect.EditorData.GetNextName(asset.GetType(), asset.Name, Architect.EditorSettings.AssetNamePattern);

                    GameAsset copy = asset.Duplicate(duplicateName);
                    Architect.Data.AddAsset(copy);

                    OpenAssetEditor(copy, true);
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
                {
                    ImGui.CloseCurrentPopup();
                }

                if (DrawDeleteModal(asset))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

        }

        private void CreateAssetButton(Type type)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);

            if (ImGuiHelpers.SelectableWithIcon($"Create new...", '\uf0fe', false))
            {
                _selectedAssetToCreate = 0;

                OpenPopupForCreateAsset(type, path: null);
            }

            ImGui.PopStyleColor();

            DrawCreateAssetModal(type, null);
        }

        /* [PERF] Stop allocating a thousand strings! */
        // Allocate a cache of strings so we don't end up allocating a thousand strings for each icon.
        private readonly Dictionary<string, string> _stringCache = new();

        private string GetNameWithDirectoryIcon(string name)
        {
            if (!_stringCache.TryGetValue(name, out string? result))
            {
                result = string.Concat("\uf07b", name);

                _stringCache[name] = result;
            }

            return result;
        }

        private readonly Dictionary<string, (string, char?)> _prettyNames = new(StringComparer.OrdinalIgnoreCase);

        private string GetFolderPrettyName(string name, out char? icon)
        {
            icon = null;

            if (!name.StartsWith(GameAsset.SkipDirectoryIconCharacter))
            {
                return name;
            }

            if (_prettyNames.TryGetValue(name, out (string Name, char? Char) result))
            {
                icon = result.Char;
                return result.Name;
            }

            string prettyName = name;
            if (name.Length >= 2)
            {
                icon = name[1];
                prettyName = name[2..];

                _prettyNames.Add(name, (prettyName, icon));
            }
            else
            {
                GameLogger.Error("Expected an icon and name for the directory name.");
            }

            return prettyName;
        }

        public Guid OpenOnTreeView(GameAsset asset, bool colapseAllOthers)
        {
            _colapseAll = colapseAllOthers;
            _openAsset = asset.Guid;
            _expandTo.AddRange(asset.GetSplitNameWithEditorPath());
            return asset.Guid;
        }

        public void OnAssetRenamedOrAddedOrDeleted()
        {
            lock (_foldersLock)
            {
                // Clear cache.
                _folders = null;
            }
        }
    }
}