using ImGuiNET;
using Murder.Components;
using Murder.Core;
using Murder.Editor.Components;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Stages;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using System.Diagnostics;

namespace Murder.Editor.CustomEditors;

internal partial class WorldAssetEditor
{
    protected virtual bool DrawPathfindEditor(Stage stage)
    {
        if (_world is null)
        {
            return false;
        }

        ShowGrid = stage.EditorHook.DrawPathfind = true; 

        bool modified = false;

        using TableMultipleColumns table = new("editor_pathfind_settings", ["\uf1de", "Tile Settings"], flags: ImGuiTableFlags.BordersInnerH,
            (ImGuiTableColumnFlags.WidthFixed, -1), (ImGuiTableColumnFlags.WidthFixed, (int)ImGui.GetWindowContentRegionMax().X));

        // Do this so we can have a padding space between tables. There is probably a fancier api for this.
        ImGui.TableSetupColumn("Table");
        ImGui.TableSetupColumn("System List", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.NoHeaderLabel);
        ImGui.TableHeadersRow();

        ImGui.TableNextColumn();

        _ = DrawRoomDimensionsWithTable(stage.EditorHook);

        // Do this so we can have a padding space between tables. There is probably a fancier api for this.
        ImGui.TableNextRow();
        ImGui.TableNextColumn();

        ImGui.PopItemWidth();

        return modified;
    }

    private bool DrawRoomDimensionsWithTable(EditorHook hook)
    {
        bool modified = false;

        ImGui.Text("Weight");
        ImGui.TableNextColumn();

        ImGui.PushItemWidth(-1);
        ImGui.PushID("weight_set");
        modified |= CustomField.DrawValue(ref hook, nameof(EditorHook.CurrentPathfindWeight));
        ImGui.PopID();
        ImGui.PopItemWidth();

        ImGui.TableNextRow();
        ImGui.TableNextColumn();

        ImGui.Text("Mask");
        ImGui.TableNextColumn();

        ImGui.PushItemWidth(150);
        ImGui.PushID("collision_set");
        modified |= CustomField.DrawValue(ref hook, nameof(EditorHook.CurrentPathfindCollisionMask));
        ImGui.PopID();
        ImGui.PopItemWidth();

        return modified;
    }
}