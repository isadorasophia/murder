using InstallWizard;
using InstallWizard.Core;
using InstallWizard.Core.Graphics;
using InstallWizard.Data;
using InstallWizard.Data.Prefabs;
using InstallWizard.DebugUtilities;
using InstallWizard.Util;
using Editor.CustomComponents;
using Editor.CustomFields;
using Editor.Gui;
using Editor.Reflection;
using Editor.Stages;
using Editor.Util;
using ImGuiNET;
using System.Diagnostics;
using Murder.Assets;

namespace Editor.CustomEditors
{
    [CustomEditorOf(typeof(PrefabAsset))]
    internal class PrefabAssetEditor : AssetEditor
    {
        internal override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _asset = (GameAsset)target;
            
            if (!Stages.ContainsKey(_asset.Guid))
                InitializeStage(new(imGuiRenderer, (PrefabAsset)_asset!), _asset.Guid);
        }

        internal override async ValueTask DrawEditor()
        {
            GameLogger.Verify(Stages is not null);
            GameLogger.Verify(_asset is not null);
            if (ImGui.BeginTable("world table", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, 480 * Architect.Instance.DPIScale/100, 0);
                ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1f, 1);

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.BeginChild(12, new(-1, -1));
                DrawEntity((IEntity)_asset);
                DrawDimensions();
                ImGui.EndChild();

                ImGui.TableNextColumn();
                if (Stages.ContainsKey(_asset.Guid))
                {
                    Stages[_asset.Guid].EditorHook.DrawSelection = false;
                    await Stages[_asset.Guid].Draw();
                }
                ImGui.EndTable();
            }
        }

        private static readonly Lazy<EditorMember> _dimensionsField = 
            new(() => typeof(PrefabAsset).TryGetFieldForEditor(nameof(PrefabAsset.Dimensions))!);

        private void DrawDimensions()
        {
            GameLogger.Verify(_asset is not null);
            GameLogger.Verify(Stages is not null);

            using RectangleBox box = new(color: Game.Profile.Theme.Accent);
            ImGuiExtended.ColorIcon('\uf545', Game.Profile.Theme.Accent);

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
