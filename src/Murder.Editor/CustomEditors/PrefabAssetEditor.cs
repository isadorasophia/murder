using Murder.Editor;
using ImGuiNET;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Core.Graphics;
using Bang;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(PrefabAsset))]
    internal class PrefabAssetEditor : AssetEditor
    {
        protected override void OnSwitchAsset(ImGuiRenderer imGuiRenderer, RenderContext renderContext, bool forceInit)
        {
            if (_asset is null)
            {
                return;
            }

            if (forceInit || !Stages.ContainsKey(_asset.Guid))
            {
                InitializeStage(new(imGuiRenderer, renderContext, (PrefabAsset)_asset), _asset.Guid);
            }

            // Disable custom shaders on prefab editors.
            renderContext.SwitchCustomShader(enable: false);

            _lastOpenedEntity = _asset as IEntity;
        }

        private IEntity? _lastOpenedEntity = null;

        public override IEntity? SelectedEntity => _lastOpenedEntity;

        public override void DrawEditor()
        {
            GameLogger.Verify(Stages is not null);
            GameLogger.Verify(_asset is not null);
            
            if (ImGui.BeginTable("prefab_table", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, 480, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1f, 1);

                if (ImGui.TableNextColumn())
                {
                    ImGui.BeginChild(id: 12, ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding, false, ImGuiWindowFlags.NoDecoration);
                    DrawSelectorPicker();
                    DrawEntity((IEntity)_asset, _asset is not PrefabAsset);
                    DrawDimensions();

                    ImGui.EndChild();
                }

                if (ImGui.TableNextColumn())
                {
                    if (Stages.ContainsKey(_asset.Guid))
                    {
                        Stages[_asset.Guid].EditorHook.DrawSelection = false;
                        Stages[_asset.Guid].Draw();
                    }
                }

                ImGui.EndTable();
            }
        }

        private static readonly Lazy<EditorMember> _dimensionsField =
            new(() => typeof(PrefabAsset).TryGetFieldForEditor(nameof(PrefabAsset.Dimensions))!);

        private void DrawSelectorPicker()
        {
            if (_asset is PrefabAsset prefab)
            {
                ImGui.SameLine();
                if (ImGuiHelpers.ColoredIconButton('\uf1fb', $"#show_{prefab.Guid}", prefab.ShowOnPrefabSelector))
                {
                    prefab.ShowOnPrefabSelector = !prefab.ShowOnPrefabSelector;
                    prefab.FileChanged = true;
                }

                ImGuiHelpers.HelpTooltip("Show this entity on the world entity picker.");
            }
        }

        private void DrawDimensions()
        {
            GameLogger.Verify(_asset is not null);
            GameLogger.Verify(Stages is not null);

            using RectangleBox box = new(color: Game.Profile.Theme.Accent);
            ImGuiHelpers.ColorIcon('\uf545', Game.Profile.Theme.Accent);

            ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Accent);

            int availableWidth = (int)ImGui.GetWindowContentRegionMax().X;

            ImGui.PushItemWidth(availableWidth * .24f);

            PrefabAsset prefab = (PrefabAsset)_asset;
            if (ImGui.TreeNodeEx("Dimensions"))
            {
                object copy = prefab.Dimensions;
                if (CustomComponent.ProcessInput(prefab, _dimensionsField.Value, () => (CustomComponent.ShowEditorOf(copy), copy)))
                {
                    prefab.FileChanged = true;
                }

                Stages[_asset.Guid].AddDimension(prefab.Guid, prefab.Dimensions);

                ImGui.TreePop();
            }
            else if (Stages.ContainsKey(_asset.Guid))
            {
                Stages[_asset.Guid].ClearDimension(prefab.Guid);
            }

            ImGui.PopItemWidth();

            ImGui.PopStyleColor();
        }
    }
}
