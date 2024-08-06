using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems.Physics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(ITransformComponent), typeof(ColliderComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(IgnoreTriggersUntilComponent))]
    [Watch(typeof(ITransformComponent))]
    public class TriggerPhysicsSystem : IReactiveSystem
    {
        private readonly List<NodeInfo<Entity>> _others = new();

        // Used for reclycing over the same collision cache.
        private readonly HashSet<int> _collisionVisitedEntities = new(516);

        public void OnActivated(World world, ImmutableArray<Entity> entities)
        {
            CheckCollisions(world, entities);
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            CheckCollisions(world, entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            CheckCollisions(world, entities);
        }

        private void CheckCollisions(World world, ImmutableArray<Entity> entities)
        {
            Quadtree qt = Quadtree.GetOrCreateUnique(world);
            foreach (Entity e in entities)
            {
                if (e.HasIgnoreTriggersUntil() || !e.IsActive)
                {
                    continue;
                }

                if (!e.HasCollider())
                {
                    e.RemoveCollisionCache();
                    continue;
                }

                ColliderComponent collider = e.GetCollider();

                // Actors and Hitboxes interact with triggers.
                // Triggers don't touch other triggers, and so on.
                bool thisIsAnActor = (collider.Layer & (CollisionLayersBase.ACTOR)) != 0;

                _others.Clear();
                qt.Collision.Retrieve(collider.GetBoundingBox(e.GetGlobalTransform().Point), _others);

                CollisionCacheComponent collisionCache = e.TryGetCollisionCache() ?? new CollisionCacheComponent();

                bool changed = false;
                bool RemoveFromCollisionCache(Entity e, Entity? other)
                {
                    // Start by notifying the other entity.
                    if (other is null)
                    {
                        return false;
                    }

                    bool shouldAlert = PhysicsServices.RemoveFromCollisionCache(other, e.EntityId);

                    // And check the entity itself.
                    if (collisionCache.CollidingWith.Contains(other.EntityId))
                    {
                        collisionCache = collisionCache.Remove(other.EntityId);
                        e.SetCollisionCache(collisionCache);

                        changed = true;
                        shouldAlert = true;
                    }

                    if (shouldAlert)
                    {
                        SendCollisionMessages(thisIsAnActor ? other : e, thisIsAnActor ? e : other, CollisionDirection.Exit);
                    }

                    return true;
                }

                for (int i = 0; i < _others.Count; i++)
                {
                    NodeInfo<Entity> node = _others[i];
                    Entity other = node.EntityInfo;
                    if (!other.IsActive)
                    {
                        continue;
                    }

                    if (other.EntityId == e.EntityId)
                    {
                        continue;
                    }

                    if (other.EntityId == e.Parent || other.Parent == e.EntityId)
                    {
                        continue;
                    }

                    if (other.HasIgnoreTriggersUntil())
                    {
                        continue;
                    }

                    ColliderComponent otherCollider = other.GetCollider();
                    if (thisIsAnActor && otherCollider.Layer == CollisionLayersBase.ACTOR ||
                        !thisIsAnActor && otherCollider.Layer == CollisionLayersBase.TRIGGER)
                    {
                        continue;
                    }

                    _collisionVisitedEntities.Add(other.EntityId);

                    // Check if there's a previous collision happening here
                    if (!collisionCache.HasId(other.EntityId))
                    {
                        if (PhysicsServices.CollidesWith(e, other)) // This is the actual physics check
                        {

                            // If no previous collision is detected, send messages and add this ID to current collision cache.
                            SendCollisionMessages(thisIsAnActor ? other : e, thisIsAnActor ? e : other, CollisionDirection.Enter);
                            PhysicsServices.AddToCollisionCache(other, e.EntityId);

                            collisionCache = collisionCache.Add(other.EntityId);
                            changed = true;
                        }
                    }
                    else if (!PhysicsServices.CollidesWith(e, other))
                    {
                        RemoveFromCollisionCache(e, other);
                    }
                }

                // Now, check for remaining entities that were not notified regarding the collision.
                foreach (int entityId in collisionCache.CollidingWith)
                {
                    if (_collisionVisitedEntities.Contains(entityId))
                    {
                        // Already verified.
                        continue;
                    }

                    Entity? other = world.TryGetEntity(entityId);
                    RemoveFromCollisionCache(e, other);
                }

                if (changed)
                {
                    e.SetCollisionCache(collisionCache);
                }

                _collisionVisitedEntities.Clear();
            }
        }

        private static void SendCollisionMessages(Entity trigger, Entity actor, CollisionDirection direction)
        {
            actor.SendOnCollisionMessage(trigger.EntityId, direction);
            trigger.SendOnCollisionMessage(actor.EntityId, direction);
        }

        public void OnDeactivated(World world, ImmutableArray<Entity> entities)
        {
            RemoveCollisions(world, entities);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            RemoveCollisions(world, entities);
        }

        private static void RemoveCollisions(World world, ImmutableArray<Entity> entities)
        {
            var colliders = world.GetEntitiesWith(typeof(CollisionCacheComponent));
            foreach (Entity deleted in entities)
            {
                bool thisIsAnActor = (deleted.GetCollider().Layer & (CollisionLayersBase.TRIGGER)) == 0;

                foreach (var entity in colliders)
                {
                    if (PhysicsServices.RemoveFromCollisionCache(entity, deleted.EntityId))
                    {
                        // Should we really send the ID of the deleted entity?
                        SendCollisionMessages(!thisIsAnActor ? deleted : entity, thisIsAnActor ? deleted : entity, CollisionDirection.Exit);
                    }
                }

                deleted.RemoveCollisionCache();
            }
        }
    }
}