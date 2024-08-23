using Bang;
using Murder.Components;
using Murder.Core;

namespace Murder.Services;

public static class WorldServices
{
    public static Guid Guid(this World world) => ((MonoWorld)world).WorldAssetGuid;

    public static int GuidToEntityId(World world, Guid guid)
    {
        InstanceToEntityLookupComponent? instanceToEntity = world.TryGetUniqueInstanceToEntityLookup();
        if (instanceToEntity is null)
        {
            return -1;
        }

        if (!instanceToEntity.Value.InstancesToEntities.TryGetValue(guid, out int entityId))
        {
            return -1;
        }

        return entityId;
    }
}