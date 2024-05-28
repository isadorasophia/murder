using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;
using System.Numerics;
using Murder.Diagnostics;
using Bang;
using System.Collections.Immutable;
using Murder.Services;
using System.Runtime.CompilerServices;

namespace Murder.Systems
{
    [Watch(typeof(TetheredComponent))]
    [Filter(ContextAccessorFilter.AllOf, typeof(TetheredComponent), typeof(PositionComponent))]
    internal class TetherSystem : IFixedUpdateSystem, IReactiveSystem
    {
        private readonly Dictionary<int, HashSet<int>> _reverseConnectionCache = new();
        private readonly HashSet<int> _edges = new();

        public void FixedUpdate(Context context)
        {
            if (!_reverseConnectionCache.Any())
            {
                BuildConnectionsCache(context.World, context.Entities);
            }

            foreach (var eId in _edges)
            {
                if (context.World.TryGetEntity(eId) is Entity entity)
                {
                    TryAdjustPosition(context.World, entity, null);
                }
            }
        }
        
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            _reverseConnectionCache.Clear();
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            _reverseConnectionCache.Clear();
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            _reverseConnectionCache.Clear();
        }

        private void BuildConnectionsCache(World world, ImmutableArray<Entity> entities)
        {
            // Build a dictionary of entities and which entities are tethered to them
            // Assumes that the dictionary is empty
            foreach (var e in entities)
            {
                var tetherComponent = e.GetTethered();

                foreach (var point in tetherComponent.TetherPoints)
                {
                    if (!_reverseConnectionCache.ContainsKey(point.Target))
                    {
                        _reverseConnectionCache[point.Target] = new HashSet<int>();
                    }

                    _reverseConnectionCache[point.Target].Add(e.EntityId);

                    // If the target is not tethered to anything, add it to the edges
                    if (world.TryGetEntity(point.Target) is Entity target && !target.HasTethered())
                    {
                        _edges.Add(target.EntityId);
                    }
                }
            }

        }
        /// <summary>
        /// Tries to adjust the position of an entity towards its tether point. Returns false if the distance snapped.
        /// </summary>
        private void TryAdjustPosition(World world, Entity e, int? targetId)
        {
            // First, move try to bind self and target
            // Get the tether point and pull the entity towards it
            if (targetId is not null &&
                world.TryGetEntity(targetId.Value) is Entity target &&
                e.TryGetTethered() is TetheredComponent tetheredComponent &&
                tetheredComponent.TetherPointsDict.TryGetValue(targetId.Value, out var tetherPoint))
            {
                Vector2 position = e.GetGlobalTransform().Vector2;
                Vector2 targetPosition = target.GetGlobalTransform().Vector2;
                Vector2 direction = targetPosition - position;

                float distance = direction.Length();

                // Static entities don't move, but still need to adjust the target
                bool isStatic = e.HasStatic();

                // This is good enough, don't adjust
                if (distance < tetherPoint.Length)
                {
                    return;
                }

                float adjustmentDistance = distance - tetherPoint.Length;
                Vector2 adjustment = direction.NormalizedWithSanity() * adjustmentDistance;

                // If I'm too far from the target, snap to the threshold point
                if (distance > tetherPoint.SnapDistance)
                {
                    // Snapping is not working as intended, so we're disabling it for now
                    // e.SendThetherSnapMessage(target.EntityId);
                    // target.SendThetherSnapMessage(e.EntityId);
                    if (!isStatic)
                    {
                        e.SetGlobalPosition(targetPosition - direction.NormalizedWithSanity() * (tetherPoint.SnapDistance));
                    }
                    
                    if (!target.HasStatic())
                    // If the entity is static, just snap the target
                    {
                        target.SetGlobalPosition(position + direction.NormalizedWithSanity() * (tetherPoint.SnapDistance));
                    }
                }

                // Adjust the target's velocity if it's not static
                if (!target.HasStatic())
                {
                    // float factor = 
                    // Pull the target towards the entity, but not as much as the entity
                    target.AddVelocity(-(0.25f * adjustment) / Game.FixedDeltaTime);
                }

                if (!isStatic)
                {
                    e.AddVelocity((0.75f * adjustment) / Game.FixedDeltaTime);
                }
            }

            // Now do the same for each attached entity
            if (_reverseConnectionCache.TryGetValue(e.EntityId, out var previousEntities))
            {
                foreach (var previous in previousEntities)
                {
                    if (world.TryGetEntity(previous) is not Entity previousEntity)
                    {
                        GameLogger.Warning($"Couldn't find previous entity {previous} for {e.EntityId}, skipping tether point");
                        continue;
                    }

                    TryAdjustPosition(world, previousEntity, e.EntityId);
                }
            }
        }
    }
}
