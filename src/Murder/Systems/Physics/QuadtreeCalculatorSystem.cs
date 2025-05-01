using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Effects;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace Murder.Systems;

[Filter(typeof(ColliderComponent), typeof(ITransformComponent))]
[Watch(typeof(ITransformComponent), typeof(ColliderComponent))]
public class QuadtreeCalculatorSystem : IReactiveSystem, IStartupSystem
{
    private readonly HashSet<int> _entitiesOnWatch = new(516);

    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            _entitiesOnWatch.Add(entities[i].EntityId);
        }
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            _entitiesOnWatch.Add(entities[i].EntityId);
        }
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            _entitiesOnWatch.Add(entities[i].EntityId);
        }
    }

    public void OnActivated(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            _entitiesOnWatch.Add(entities[i].EntityId);
        }
    }
    public void OnDeactivated(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            _entitiesOnWatch.Add(entities[i].EntityId);
        }
    }

    public void OnAfterTrigger(World world)
    {
        if (_entitiesOnWatch.Count == 0)
        {
            return;
        }

        Quadtree qt = Quadtree.GetOrCreateUnique(world);
        foreach (int entityId in _entitiesOnWatch)
        {
            if (world.TryGetEntity(entityId) is Entity entity && entity.IsActive && entity.HasCollider())
            {
                qt.RemoveFromCollisionQuadTree(entityId);
                qt.AddToCollisionQuadTree(entity);
            }
            else
            {
                // This entity was removed from the world.
                qt.RemoveFromCollisionQuadTree(entityId);
            }
        }

        _entitiesOnWatch.Clear();
    }

    public void Start(Context context)
    {
        for (int i = 0; i < context.Entities.Length; i++)
        {
            _entitiesOnWatch.Add(context.Entities[i].EntityId);
        }
    }
}