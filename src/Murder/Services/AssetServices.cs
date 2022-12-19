using Bang;
using Bang.Entities;
using Murder.Diagnostics;
using Murder.Assets;
using Murder.Components;
using Murder;
using Bang.Components;

public static class AssetServices
{
    /// <summary>
    /// Creates an entity using the EntityAsset with the provided GUID and adds it to the world.
    /// </summary>
    public static Entity Create(World world, Guid guid, params IComponent[] components)
    {
        if (TryCreate(world, guid, components) is Entity entity)
        {
            if (entity.Parent != null)
                return world.GetEntity(entity.Parent.Value);
            else
                return entity;
        }

        GameLogger.Fail("Unable to create ally!");
        throw new InvalidOperationException("Unable to create requested entity.");
    }

    /// <summary>
    /// Try to create an entity using the EntityAsset with the provided GUID and adds it to the world.
    /// </summary>
    public static Entity? TryCreate(World world, Guid guid, params IComponent[] components)
    {
        if (Game.Data.TryGetAsset<PrefabAsset>(guid) is not PrefabAsset asset)
        {
            return null;
        }

        int id = asset.Create(world, components);
        if (world.TryGetEntity(id) is Entity entity)
        {
            return entity;
        }

        return null;
    }

    public static PrefabAsset GetAsset(this PrefabRefComponent component) => Game.Data.GetAsset<PrefabAsset>(component.AssetGuid);

    public static PrefabAsset? TryGetAsset(this PrefabRefComponent component) => Game.Data.TryGetAsset<PrefabAsset>(component.AssetGuid);
}