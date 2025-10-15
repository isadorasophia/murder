using Bang;
using Murder.Attributes;
using Murder.Core;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Assets;

public class WorldEventsAsset : GameAsset
{
    public override char Icon => '\uf7c0';
    public override string EditorFolder => "#\uf7c0Global Events";

    public override Vector4 EditorColor => "#34ebcf".ToVector4Color();

    [Tooltip("Events that will be placed on all maps (or the specified one)")]
    public readonly ImmutableArray<TriggerEventOn> Watchers = [];

    public virtual void Preprocess(World world, CreateWorldFlags flags)
    {
        if (!flags.HasFlag(CreateWorldFlags.FirstTimeLoaded))
        {
            return;
        }

        Guid worldGuid = world.Guid();

        // Load all events from the asset.
        foreach (TriggerEventOn trigger in Watchers)
        {
            if (trigger.World is Guid guid && guid != worldGuid)
            {
                // Not meant to this world.
                continue;
            }

            world.AddEntity(trigger.CreateComponents());
        }
    }
}

[Flags]
public enum CreateWorldFlags
{
    None = 0,
    FirstTimeLoaded = 1,
    AfterStarted = 2
}

public static class WorldEventsTracker
{
    private static ImmutableDictionary<Guid, GameAsset>? _assets = null;

    public static void PreprocessWorldEvents(World world, CreateWorldFlags flags)
    {
        _assets ??= Game.Data.FilterAllAssets(typeof(WorldEventsAsset));

        foreach ((_, GameAsset asset) in _assets)
        {
            if (asset is not WorldEventsAsset worldEvents)
            {
                continue;
            }

            worldEvents.Preprocess(world, flags);
        }
    }

    public static void ClearCache()
    {
        _assets = null;
    }
}