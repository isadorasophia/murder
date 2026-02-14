using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Systems;

[Filter(typeof(PositionComponent), typeof(ColliderComponent))]
[Watch(typeof(PositionComponent), typeof(ColliderComponent))]
public class QuadtreeCalculatorSystem : IReactiveSystem, IStartupSystem
{
    private readonly HashSet<int> _toUpdate = new(256);
    private readonly HashSet<int> _toRemove = new(64);

    public void Start(Context context)
    {
        foreach (Entity e in context.Entities)
        {
            TriggerAppropriateAction(e);
        }
    }

    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            TriggerAppropriateAction(entities[i]);
        }
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            TriggerAppropriateAction(entities[i]);
        }
    }

    public void OnActivated(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            TriggerAppropriateAction(entities[i]);
        }
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            TriggerAppropriateAction(entities[i]);
        }
    }

    public void OnDeactivated(World world, ImmutableArray<Entity> entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            TriggerAppropriateAction(entities[i]);
        }
    }

    private void TriggerAppropriateAction(Entity e)
    {
        int id = e.EntityId;

        if (e is null || !e.IsActive || !e.HasPosition() || !e.HasCollider())
        {
            _toRemove.Add(id);
            _toUpdate.Remove(id);
        }
        else
        {
            _toUpdate.Add(id);
            _toRemove.Remove(id);
        }
    }

    public void OnAfterTrigger(World world)
    {
        if (_toUpdate.Count == 0 && _toRemove.Count == 0)
        {
            return;
        }

        Quadtree qt = Quadtree.GetOrCreateUnique(world);

        // Handle removals
        foreach (int entityId in _toRemove)
        {
            qt.RemoveFromCollisionQuadTree(entityId);
        }

        // Handle updates/additions
        foreach (int entityId in _toUpdate)
        {
            if (world.TryGetEntity(entityId) is not Entity entity || !entity.IsActive || 
                entity.TryGetCollider() is not ColliderComponent collider)
            {
                qt.TryRemoveFromCollisionQuadTree(entityId); // it might never been added in the first place, which is fine.
                continue;
            }

            Vector2 pos = entity.GetGlobalPosition();

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

        _toUpdate.Clear();
        _toRemove.Clear();
    }
}