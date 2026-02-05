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
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Systems;

[Filter(typeof(ColliderComponent), typeof(PositionComponent))]
[Watch(typeof(PositionComponent), typeof(ColliderComponent))]
public class QuadtreeCalculatorSystem : IReactiveSystem, IStartupSystem
{
    private readonly HashSet<int> _toUpdate = new(256);
    private readonly HashSet<int> _toRemove = new(64);
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
            _toUpdate.Add(entities[i].EntityId);
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
            _toUpdate.Add(entities[i].EntityId);
    }

    public void OnActivated(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
            _toUpdate.Add(entities[i].EntityId);
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            int id = entities[i].EntityId;
            _toUpdate.Remove(id);  // Don't update if also removing
            _toRemove.Add(id);
        }
    }

    public void OnDeactivated(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            int id = entities[i].EntityId;
            _toUpdate.Remove(id);
            _toRemove.Add(id);
        }
    }
    public void OnAfterTrigger(World world)
    {
        if (_toUpdate.Count == 0 && _toRemove.Count == 0)
            return;

        Quadtree qt = Quadtree.GetOrCreateUnique(world);

        // Handle removals
        foreach (int entityId in _toRemove)
        {
            qt.RemoveFromCollisionQuadTree(entityId);
        }

        // Handle updates/additions
        foreach (int entityId in _toUpdate)
        {
            if (world.TryGetEntity(entityId) is Entity entity && entity.IsActive)
            {
                Vector2 pos = entity.GetGlobalPosition();
                ColliderComponent collider = entity.GetCollider();
                Rectangle bounds = collider.GetBoundingBox(pos, entity.FetchScale());
                
                qt.Collision.Update(entityId, entity, bounds);

                // Same for PushAway
                if (entity.TryGetPushAway() is PushAwayComponent pushAway)
                {
                    float halfSize = pushAway.Size * 0.5f;
                    Rectangle pushBounds = new(pos.X - halfSize, pos.Y - halfSize, pushAway.Size, pushAway.Size);
                    qt.PushAway.Update(entityId, (entity, pos, pushAway, entity.TryGetVelocity()?.Velocity ?? Vector2.Zero), pushBounds);
                }
            }
        }

        _toUpdate.Clear();
        _toRemove.Clear();
    }

    public void Start(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            _toUpdate.Add(e.EntityId);
        }
    }
}