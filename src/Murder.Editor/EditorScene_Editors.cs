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
using System.Collections.Immutable;

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

                SharedRenderContext = new(Game.GraphicsDevice, new(320, 240), useCustomShader: false /* editors will tweak this */);
                SharedRenderContext.RenderToScreen = false;
            }

            public void Dispose()
            {
                SharedRenderContext.Dispose();
                Editor.Dispose();
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

        private bool _initializedEditors = false;

        private void DrawAssetEditors()
        {
            GameAsset? closeTab = null;

            foreach (var currentAsset in _selectedAssets.Values.ToImmutableArray())
            {
                bool show = true;

                // TODO: [Pedro?] Fix this. For some reason, after reopening the editor
                // ImGui thinks that it can open *all* the tabs until it settles down,
                // which makes the window "flicker". I am very annoyed with this.
                if (_initializedEditors && _tabToSelect == currentAsset.Guid)
                {
                    ImGui.SetNextWindowFocus();
                    _tabToSelect = Guid.Empty;
                }

                ImGui.SetNextWindowDockID(EDITOR_DOCK_ID, ImGuiCond.Appearing);
                ImGuiWindowFlags fileSaved = currentAsset.FileChanged ? ImGuiWindowFlags.UnsavedDocument : ImGuiWindowFlags.None;
                if (ImGui.Begin($"{currentAsset.Icon} {currentAsset.GetSimplifiedName()}##{currentAsset.Guid}", ref show, fileSaved) &&
                    _initializedEditors)
                {
                    if (_selectedTab != currentAsset.Guid)
                    {
                        _selectedTab = currentAsset.Guid;
                        OpenAssetEditor(currentAsset, false);
                    }

                    DrawSelectedAsset(currentAsset);
                }

                ImGui.End();

                if (!show)
                {
                    closeTab = currentAsset;
                }
            }

            _initializedEditors = true;

            if (closeTab != null)
            {
                DeleteAssetEditor(closeTab.Guid);
                _selectedAssets.Remove(closeTab.Guid);
                _selectedTab = Guid.Empty;
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
                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip(asset.Guid.ToString());
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

                ImGui.SameLine();
                if (ImGui.Button("Discard Changes"))
                {
                    ImGui.OpenPopup("Discard?");
                }

                //Draw modals
                DrawDeleteModal(asset);
                DrawRenameModal(asset);
                DrawDiscardModal(asset);
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

                    if (showColliders)
                    {
                        ImGui.SameLine();
                        bool keepShapes = assetEditor.KeepColliderShapes;
                        ImGui.Checkbox("Keep Collider Shapes", ref keepShapes);
                        assetEditor.KeepColliderShapes = keepShapes;
                    }

                    ImGui.SameLine();
                    bool showReflection = assetEditor.ShowReflection;
                    ImGui.Checkbox("Show Reflection", ref showReflection);
                    assetEditor.ShowReflection = showReflection;

                }

                if (customEditor.Editor is WorldAssetEditor worldEditor)
                {
                    ImGui.SameLine();
                    bool showPuzzles = worldEditor.ShowPuzzles;
                    ImGui.Checkbox("Show Puzzles", ref showPuzzles);

                    ImGui.SameLine();
                    bool showCamera = worldEditor.ShowCameraBounds;
                    ImGui.Checkbox("Show Camera Bounds", ref showCamera);

                    ImGui.SameLine();
                    bool hideStatic = worldEditor.HideStatic;
                    ImGui.Checkbox("Hide Static Entities", ref hideStatic);

                    worldEditor.ShowPuzzles = showPuzzles;
                    worldEditor.ShowCameraBounds = showCamera;
                    worldEditor.HideStatic = hideStatic;

                    if (worldEditor.ShowCameraBounds)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button("Reset Camera Bounds"))
                        {
                            worldEditor.ResetCameraBounds();
                        }
                    }
                }

                if (CurrentAsset?.Guid != asset.Guid)
                {
                    customEditor.Editor.OpenEditor(Architect.Instance.ImGuiRenderer, customEditor.SharedRenderContext, asset, false);
                }

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
                editorInstance.Editor.CloseEditor(guid);
                editorInstance.Counter--;

                if (editorInstance.Counter == 0)
                {
                    _editors.Remove(editorType);

                    editorInstance.Dispose();
                }
            }

            _guidToEditors.Remove(guid);
        }

        public Guid OpenAssetEditor(GameAsset asset, bool overwrite)
        {
            GameLogger.Verify(RenderContext is not null);

            _selectedAssets[asset.Guid] = asset;
            _tabToSelect = _tabToSelect == asset.Guid ? Guid.Empty : asset.Guid;

            if (GetOrCreateAssetEditor(asset) is CustomEditorInstance editor)
            {
                editor.Editor.OpenEditor(Architect.Instance.ImGuiRenderer, editor.SharedRenderContext, asset, overwrite);
            }
            
            return asset.Guid;
        }
    }
}
