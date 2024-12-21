using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;

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

        private readonly Dictionary<int, (Entity, ImmutableHashSet<int>, bool)> _entitiesOnWatch = new(256);

        private void WatchEntities(ImmutableArray<Entity> entities, World world)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                if (_entitiesOnWatch.ContainsKey(entity.EntityId))
                {
                    continue;
                }

                bool isActor = false;
                if (entity.TryGetCollider() is ColliderComponent collider)
                {
                    isActor = (collider.Layer & (CollisionLayersBase.TRIGGER)) == 0;
                }

                if (entity.IsDestroyed)
                {
                    // Destroyed entities are not added to the watch list.
                    // But we message them immediately.

                    if (entity.TryGetCollisionCache() is not CollisionCacheComponent destroyedCollisionCache)
                    {
                        continue;
                    }

                    foreach (var other in destroyedCollisionCache.GetCollidingEntities(world))
                    {
                        entity.SendOnCollisionMessage(other.EntityId, CollisionDirection.Exit);
                        other.SendOnCollisionMessage(entity.EntityId, CollisionDirection.Exit);
                    }

                    continue;
                }

                if (entity.TryGetCollisionCache() is CollisionCacheComponent collisionCache)
                {
                    _entitiesOnWatch[entity.EntityId] = (entity, collisionCache.CollidingWith, isActor);
                }
                else
                {
                    _entitiesOnWatch[entity.EntityId] = (entity, [], isActor);
                }
            }
        }

        public void OnDeactivated(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities, world);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities, world);
        }

        public void OnActivated(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities, world);
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities, world);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities, world);
        }

        public void OnAfterTrigger(World world)
        {
            if (_entitiesOnWatch.Count == 0)
            {
                return;
            }

            Quadtree qt = Quadtree.GetOrCreateUnique(world);

            foreach ((int entityId, (Entity entity, ImmutableHashSet<int> collidingWith, bool thisIsAnActor)) in _entitiesOnWatch)
            {
                // Remove deactivated or destroyed entities from the collision cache and send exit messages.
                if (!entity.IsActive || entity.IsDestroyed)
                {
                    foreach (int otherCachedId in collidingWith)
                    {
                        Entity? otherCached = world.TryGetEntity(otherCachedId);
                        if (otherCached != null && PhysicsServices.RemoveFromCollisionCache(otherCached, entityId))
                        {
                            // Should we really send the ID of the deleted entity?
                            entity.SendOnCollisionMessage(otherCached.EntityId, CollisionDirection.Exit);
                            otherCached.SendOnCollisionMessage(entityId, CollisionDirection.Exit);
                        }
                    }
                }
                else
                {
                    // Check for active entities.
                    CheckCollision(entity, qt, world);
                }
            }

            // Clear the list for the next frame.
            _entitiesOnWatch.Clear();
        }

        private void CheckCollision(Entity e, Quadtree qt, World world)
        {
            if (e.HasIgnoreTriggersUntil())
            {
                return;
            }

            if (!e.HasCollider())
            {
                e.RemoveCollisionCache();
                return;
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

                if (other.HasIgnoreTriggersUntil() || !other.HasCollider())
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
        private static void SendCollisionMessages(Entity trigger, Entity actor, CollisionDirection direction)
        {
            actor.SendOnCollisionMessage(trigger.EntityId, direction);
            trigger.SendOnCollisionMessage(actor.EntityId, direction);
        }
    }
}