
using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems;

[Filter(typeof(TimeScaleComponent))]
[Watch(typeof(TimeScaleComponent))]
public class TimeScaleSystem : IReactiveSystem, IStartupSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        UpdateEntities(entities);
    }

    private static void UpdateEntities(ImmutableArray<Entity> entities)
    {
        float timeScale = 1f;
        foreach (var e in entities)
        {
            var t = e.GetTimeScale();
            timeScale *= t.Value;
        }
        Game.Instance.TimeScale = timeScale;
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        UpdateEntities(entities);
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        UpdateEntities(entities);
    }

    public void Start(Context context)
    {
        UpdateEntities(context.Entities);
    }
}
