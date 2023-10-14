using ImGuiNET;
using Murder.Assets;
using Murder.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Guid))]
    internal class GuidField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            Guid guid = (Guid)fieldValue!;

            if (AttributeExtensions.FindGameAssetType(member, out GameAssetIdInfo? info))
            {
                bool changed = SearchBox.SearchAsset(ref guid, info.Value);
                return (changed, guid);
            }

            if (AttributeExtensions.IsDefined(member, typeof(InstanceIdAttribute)))
            {
                if (Architect.Instance.ActiveScene is EditorScene editor && editor.CurrentAsset is WorldAsset world)
                {
                    bool changed = SearchBox.SearchInstanceInWorld(ref guid, world);
                    return (changed, guid);
                }
                else
                {
                    ImGui.TextColored(Game.Profile.Theme.Faded, "Can only be set when on a world!");
                }
            }

            return (modified, guid);
        }
    }
}