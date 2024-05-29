using ImGuiNET;
using Murder.Assets;
using Murder.Core.Smart;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields;

[CustomFieldOf(typeof(SmartInt))]
public class SmartIntField : CustomField
{
    public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
    {
        bool modified = false;
        SmartInt target = (SmartInt)fieldValue!;

        Guid guid = target.Asset;
        if (Game.Data.TryGetAsset(target.Asset) is SmartIntAsset asset)
        {
            ImGui.SetNextItemWidth(150);
            if (ImGui.BeginCombo($"##{member.Name}", asset.Titles[target.Index]))
            {
                for (int i = 0; i < asset.Titles.Length; i++)
                {
                    if (ImGui.Selectable(asset.Titles[i]))
                    {
                        modified = true;
                        target = new SmartInt(target.Asset, i, target.Custom);
                    }
                    ImGui.SameLine();
                    ImGui.TextColored(Architect.Profile.Theme.Faded, asset.Values[i].ToString());
                }

                ImGui.EndCombo();
            }

            ImGui.SameLine();
            ImGui.Text(asset.Values[target.Index].ToString());
        }
        else
        {
            int custom = target.Custom;
            ImGui.SetNextItemWidth(150);
            if (ImGui.InputInt("###Custom", ref custom))
            {
                modified = true;
                target = new SmartInt(target.Asset, target.Index, custom);
            }
    
            ImGui.SameLine();
            ImGui.TextColored(Architect.Profile.Theme.Faded, "or");

        }


        ImGui.SameLine();
        if (SearchBox.SearchAsset(ref guid, typeof(SmartIntAsset)))
        {
            modified = true;
            target = new SmartInt(guid, target.Index, target.Custom);
        }

        return (modified, target);
    }
}