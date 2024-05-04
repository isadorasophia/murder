
using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems;

[Filter(typeof(TimeScaleComponent))]
[Watch(typeof(TimeScaleComponent))]
public class TimeScaleSystem : IReactiveSystem, IStartupSystem, IExitSystem
{
    private readonly Dictionary<int,float> _entities = new();
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        foreach (var e in entities)
        {
            if (e.TryGetTimeScale()?.Value is float value) {
                _entities[e.EntityId] = value;
            }
        }
        UpdateEntities();
    }

    private void UpdateEntities()
    {
        float timeScale = 1f;
        foreach (var v in _entities.Values)
        {
            timeScale *= v;
        }
        Game.Instance.TimeScale = timeScale;
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        foreach (var e in entities)
        {
            if (e.TryGetTimeScale()?.Value is float value)
            {
                _entities[e.EntityId] = value;
            }
        }
        UpdateEntities();
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        foreach (var e in entities)
        {
            _entities.Remove(e.EntityId);
        }
        UpdateEntities();
    }

    public void Start(Context context)
    {
        UpdateEntities();
    }

    public void Exit(Context context)
    {
        Game.Instance.TimeScale = 1f;
    }
}
