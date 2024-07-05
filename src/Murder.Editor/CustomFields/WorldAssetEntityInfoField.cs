using ImGuiNET;
using Murder.Assets;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;

namespace Murder.Editor;

[CustomFieldOf(typeof(WorldAssetEntityInfo))]
public class WorldAssetEntityInfoField : CustomField
{
    public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
    {
        bool modified = false;
        WorldAssetEntityInfo info = (WorldAssetEntityInfo)fieldValue!;

        modified |= DrawValueWithId(ref info, nameof(WorldAssetEntityInfo.World));

        if (info.World != Guid.Empty && Game.Data.TryGetAsset<WorldAsset>(info.World) is WorldAsset world)
        {
            Guid entityGuid = info.Entity;
            if (SearchBox.SearchInstanceInWorld(ref entityGuid, world))
            {
                info = info with { Entity = entityGuid };
                modified = true;
            }
        }
        else
        {
            ImGui.TextColored(Game.Profile.Theme.Faded, "Select a world guid before choosing an entity!");
        }

        return (modified, info);
    }
}