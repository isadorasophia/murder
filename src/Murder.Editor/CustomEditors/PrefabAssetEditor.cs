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

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(PrefabAsset))]
    internal class PrefabAssetEditor : AssetEditor
    {
        public override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _asset = (GameAsset)target;
            
            if (!Stages.ContainsKey(_asset.Guid))
                InitializeStage(new(imGuiRenderer, (PrefabAsset)_asset!), _asset.Guid);
        }

        public override void DrawEditor()
        {
            GameLogger.Verify(Stages is not null);
            GameLogger.Verify(_asset is not null);
            if (ImGui.BeginTable("world table", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, 480 * Architect.Instance.DPIScale/100, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1f, 1);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.BeginChild(id: 12, new(-1, -1), false, ImGuiWindowFlags.NoDecoration);
                DrawSelectorPicker();
                DrawEntity((IEntity)_asset);
                DrawDimensions();

                ImGui.EndChild();


                ImGui.TableNextColumn();
                if (Stages.ContainsKey(_asset.Guid))
                {
                    Stages[_asset.Guid].EditorHook.DrawSelection = false;
                    Stages[_asset.Guid].Draw();
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

            ImGui.PopStyleColor();
        }
    }
}
