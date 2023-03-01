using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.CustomComponents;
using Murder.Editor.CustomEditors;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Utilities;

namespace Murder.Editor
{
    public partial class EditorScene
    {
        private record CustomEditorInstance : IDisposable
        {
            public readonly RenderContext SharedRenderContext;
            public readonly CustomEditor Editor;
            public int Counter = 1;

            public CustomEditorInstance(Type t)
            {
                Editor = (CustomEditor)Activator.CreateInstance(t)!;

                SharedRenderContext = new(Game.GraphicsDevice, new(320, 240, 2), useCustomShader: false);
                SharedRenderContext.RenderToScreen = false;
            }

            public void Dispose()
            {
                SharedRenderContext.Dispose();
            }
        }

        /// <summary>
        /// Maps the assets guids to their corresponding editor type.
        /// </summary>
        private readonly Dictionary<Guid, Type> _guidToEditors = new();

        /// <summary>
        /// Maps the custom editor types with their active instance.
        /// </summary>
        private readonly Dictionary<Type, CustomEditorInstance> _editors = new();

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
                if (ImGui.Begin($"{tab.Icon} {tab.GetSimplifiedName()}##{tab.Guid}", ref open, fileSaved))
                {
                    _assetShown = tab;
                    DrawSelectedAsset(tab);
                }

                ImGui.End();

                if (!open)
                {
                    closeTab = tab;

                    if (tab == _assetShown)
                    {
                        _assetShown = null;
                    }
                }
            }

            if (closeTab != null)
            {
                DeleteAssetEditor(closeTab.Guid);
                _selectedAssets = _selectedAssets.Remove(closeTab);
            }
        }

        private void DrawSelectedAsset(GameAsset asset)
        {
            GameLogger.Verify(RenderContext is not null);

            ImGui.Spacing();

            CustomEditorInstance? customEditor = GetOrCreateAssetEditor(asset);

            // Draw the editor header
            if (ImGui.BeginChild("Asset Editor", new System.Numerics.Vector2(-1, -1), false))
            {
                if (asset.FileChanged)
                {
                    ImGuiHelpers.ColorIcon('\uf0c7', Game.Profile.Theme.Red);
                    ImGui.SameLine();
                }

                if (string.IsNullOrWhiteSpace(asset.FilePath))
                {
                    ImGui.TextColored(Microsoft.Xna.Framework.Color.DarkRed.ToSysVector4(), $"(FILE NOT SAVED)");
                }
                else
                {
                    ImGui.TextColored(Microsoft.Xna.Framework.Color.DarkGray.ToSysVector4(), $"({asset.FilePath}){(asset.FileChanged ? "*" : "")}");
                }

                ImGui.SameLine();
                ImGui.TextColored(Microsoft.Xna.Framework.Color.DarkGray.ToSysVector4(), $"({asset.GetType().Name})");
                ImGui.SameLine();

                if (asset.CanBeSaved && (ImGui.Button("Save Asset") || Architect.Input.Shortcut(Keys.S, Keys.LeftControl)))
                {
                    customEditor?.Editor.PrepareForSaveAsset();

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
                    string? path = asset.GetEditorAssetDirectoryPath();
                    if (path is not null)
                    {
                        FileHelper.OpenFolder(path);
                    }
                    else
                    {
                        GameLogger.Error($"Couldn't parse the path for {asset.Name}");
                    }
                }

                //Draw modals
                DrawDeleteModal(asset);
                DrawRenameModal(asset);
            }

            // Draw the custom editor
            if (customEditor is not null)
            {
                if (customEditor.Editor is AssetEditor assetEditor)
                {
                    ImGui.SameLine();
                    bool showColliders = assetEditor.ShowColliders;
                    ImGui.Checkbox("Show Colliders", ref showColliders);

                    assetEditor.ShowColliders = showColliders;
                }

                if (customEditor.Editor is WorldAssetEditor worldEditor)
                {
                    ImGui.SameLine();

                    bool showPuzzles = worldEditor.ShowPuzzles;
                    ImGui.Checkbox("Show Puzzles", ref showPuzzles);

                    worldEditor.ShowPuzzles = showPuzzles;
                }

                customEditor.Editor.OpenEditor(Architect.Instance.ImGuiRenderer, customEditor.SharedRenderContext, asset);
                customEditor.Editor.DrawEditor();
            }
            else
            {
                asset.FileChanged |= CustomComponent.ShowEditorOf(ref asset);
            }

            ImGui.EndChild();
        }

        private CustomEditorInstance? GetOrCreateAssetEditor(GameAsset asset)
        {
            if (_guidToEditors.TryGetValue(asset.Guid, out Type? editorType))
            {
                return _editors[editorType];
            }

            if (!CustomEditorsHelper.TryGetCustomEditor(asset.GetType(), out editorType))
            {
                // This means that there is no custom editor for this asset.
                // We will manually serialize this by looking at its fields.
                return null;
            }

            _guidToEditors.Add(asset.Guid, editorType);

            if (_editors.TryGetValue(editorType, out CustomEditorInstance? editorInstance))
            {
                editorInstance.Counter++;
                return editorInstance;
            }

            // Otherwise, create an editor instance.
            editorInstance = new(editorType);
            _editors.Add(editorType, editorInstance);

            return editorInstance;
        }

        private void DeleteAssetEditor(Guid guid)
        {
            if (_guidToEditors.TryGetValue(guid, out Type? editorType) &&
                _editors.TryGetValue(editorType, out CustomEditorInstance? editorInstance))
            {
                if (editorInstance.Editor is AssetEditor assetEditor)
                {
                    assetEditor.RemoveStage(guid);
                }

                editorInstance.Counter--;

                if (editorInstance.Counter == 0)
                {
                    _editors.Remove(editorType);

                    editorInstance.Dispose();
                }
            }

            _guidToEditors.Remove(guid);
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

            if (GetOrCreateAssetEditor(asset) is CustomEditorInstance editor)
            {
                editor.Editor.OpenEditor(Architect.Instance.ImGuiRenderer, editor.SharedRenderContext, asset);
            }
        }
    }
}
