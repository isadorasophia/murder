using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using System.Numerics;
using Murder.Assets;
using Murder.Editor.ImGuiExtended;
using Murder.Diagnostics;
using Murder.Editor.Utilities;

namespace Murder.Editor
{
    public partial class EditorScene
    {
        private string _newAssetName = string.Empty;

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
                    if ((searchForType is null || searchForType.IsAssignableFrom(t)) && !t.IsAbstract)
                    {
                        assetTypes.Add(t);
                    }
                }

                if (assetTypes.Count > 0 && assetTypes.Count > _selectedAssetToCreate)
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
                                string name = !string.IsNullOrEmpty(path) ?
                                    $"{path}{Path.DirectorySeparatorChar}{_newAssetName.Trim()}" :
                                    _newAssetName.Trim();

                                name = AssetsFilter.GetValidName(createAssetOfType, name);

                                _selectedAsset = Architect.EditorData.CreateNewAsset(createAssetOfType, name);
                                GameLogger.Verify(_selectedAsset is not null);

                                OpenAssetEditor(_selectedAsset);

                                // For now, we manually check if the containg folder is the same as the specified path.
                                // TODO: I think this is flakey when there are multiple levels of directories.
                                // TODO reply: Technically, I fixed this?? Leaving this here just in case.
                                //int separatorIndex = _selectedAsset.EditorFolder.LastIndexOf(GameAsset.SkipDirectoryIconCharacter);
                                //if (separatorIndex != -1)
                                //{
                                //    separatorIndex += 2;

                                //    string containingFolder = _selectedAsset.EditorFolder[separatorIndex..];
                                //    if (containingFolder == path)
                                //    {
                                //        name = _newAssetName.Trim();
                                //    }
                                //}

                                _selectedAsset.Name = name;
                                _selectedAsset.FileChanged = true;
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

        private void DrawAssetFolder(string folderName, Vector4 color, Type? createType, IEnumerable<GameAsset> assets) =>
            DrawAssetFolder(folderName, color, createType, assets, 0, string.Empty);
        
        private void DrawAssetFolder(string folderName, Vector4 color, Type? createType, IEnumerable<GameAsset> assets, int depth, string folderRootPath)
        {
            string printName = GetFolderPrettyName(folderName, out char? icon);

            Dictionary<string, (Vector4 color, Type? createType, List<GameAsset> assets)> foldersToDraw = new();
            foreach (GameAsset asset in assets)
            {
                var folders = Path.Combine(asset.EditorFolder, asset.Name).Split('\\', '/');
                if (folders.Length > depth + 1)
                {
                    string currentFolder = folders[depth];
                    if (!foldersToDraw.ContainsKey(currentFolder))
                    {
                        // Add create asset button to the folder if necessary
                        Type t = asset.GetType();

                        foldersToDraw[currentFolder] = (asset.EditorColor, t, new List<GameAsset>());
                    }

                    foldersToDraw[currentFolder].assets.Add(asset);
                }
            }

            IEnumerable<(string folder, Vector4 color, Type? createType, List<GameAsset> assets)> orderedDirectories =
                foldersToDraw.OrderBy(kv => GetFolderPrettyName(kv.Key, out _)).Select(kv => (kv.Key, kv.Value.color, kv.Value.createType, kv.Value.assets));

            if (icon.HasValue && depth > 0)
            {
                ImGuiHelpers.ColorIcon(icon.Value, color);
                ImGui.SameLine();
            }

            if (depth <= 1) ImGui.PushStyleColor(ImGuiCol.Text, color);

            string currentDirectoryPath = depth < 2 ? string.Empty : string.IsNullOrEmpty(folderRootPath) ? printName : $"{folderRootPath}/{printName}";

            bool isFolderOpened = string.IsNullOrWhiteSpace(printName) || ImGui.TreeNodeEx(printName);
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
                foreach ((string folder, Vector4 folderColor, Type? folderCreateType, List<GameAsset> folderAssets) in orderedDirectories)
                {
                    if (folder.StartsWith(GameAsset.SkipDirectoryIconCharacter))
                    {
                        DrawAssetFolder(folder, folderColor, folderCreateType, folderAssets, depth + 1, currentDirectoryPath);
                    }
                    else
                    {
                        DrawAssetFolder($"#\uf07b{folder}", folderColor, folderCreateType, folderAssets, depth + 1, currentDirectoryPath);
                    }
                }

                foreach (GameAsset asset in assets)
                {
                    var folders = Path.Combine(asset.EditorFolder, asset.Name).Split('\\', '/');
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
            string name = type == typeof(GameAsset) ? 
                "asset (pick one!)" : 
                Prettify.FormatAssetName(type.Name);

            ImGui.PushID($"context_create_{type.Name}");
            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);

            bool shouldOpenPopUp = false;
            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.Selectable($"Create new {name}"))
                {
                    shouldOpenPopUp = true;
                }

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
            ImGui.PopID();


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

            if (ImGuiHelpers.SelectableWithIcon($"", '\uf0fe', false))
            {
                _selectedAssetToCreate = 0;

                OpenPopupForCreateAsset(type, path: null);
            }

            ImGui.PopStyleColor();

            DrawCreateAssetModal(type, null);
        }

        private string GetFolderPrettyName(string name, out char? icon)
        {
            if (name.StartsWith(GameAsset.SkipDirectoryIconCharacter))
            {
                if (name.Length >= 2)
                {
                    icon = name[1];
                    return name[2..];
                }
                else
                {
                    GameLogger.Error("Expected an icon and name for the directory name.");
                }
            }

            icon = null;
            return name;
        }
    }
}
