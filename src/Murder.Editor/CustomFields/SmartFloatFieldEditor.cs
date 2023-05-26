
using ImGuiNET;
using Murder.Assets;
using Murder.Core.Smart;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields;


[CustomFieldOf(typeof(SmartFloat))]
internal class SmartFloatFieldEditor : CustomField
{
    public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
    {
        bool modified = false;
        SmartFloat target = (SmartFloat)fieldValue!;
        
        Guid guid = target.Asset;
        if (Game.Data.TryGetAsset(target.Asset) is SmartFloatAsset asset)
        {
            if (ImGui.BeginCombo(member.Name, asset.Titles[target.Index]))
            {
                for (int i = 0; i < asset.Titles.Length; i++)
                {
                    if (ImGui.Selectable(asset.Titles[i]))
                    {
                        modified = true;
                        target = new SmartFloat(target.Asset, i, target.Custom);
                    }
                    ImGui.SameLine();
                    ImGui.TextColored(Architect.Profile.Theme.Faded, asset.Values[i].ToString());
                }
                
                ImGui.EndCombo();
            }

            ImGui.Text(asset.Values[target.Index].ToString());
        }
        else
        {
            float custom = target.Custom;
            if (ImGui.InputFloat("###Custom", ref custom))
            {
                modified = true;
                target = new SmartFloat(target.Asset, target.Index, custom);
            }
        }


        if (SearchBox.SearchAsset(ref guid, typeof(SmartFloatAsset)))
        {
            modified = true;
            target = new SmartFloat(guid, target.Index, target.Custom);
        }

        return (modified, target);
    }
}
