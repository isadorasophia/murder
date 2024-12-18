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
public class QuadtreeCalculatorSystem : IReactiveSystem, IFixedUpdateSystem
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

    public void FixedUpdate(Context context)
    {
        var qt = Quadtree.GetOrCreateUnique(context.World);

        foreach (var entityId in _entitiesOnWatch)
        {
            if (context.World.TryGetEntity(entityId) is Entity entity)
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
}