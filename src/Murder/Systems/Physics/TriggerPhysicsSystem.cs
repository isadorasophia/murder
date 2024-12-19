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

namespace Murder.Systems.Physics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(ITransformComponent), typeof(ColliderComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(IgnoreTriggersUntilComponent))]
    [Watch(typeof(ITransformComponent))]
    public class TriggerPhysicsSystem : IReactiveSystem, IFixedUpdateSystem
    {
        private readonly List<NodeInfo<Entity>> _others = new();

        // Used for reclycing over the same collision cache.
        private readonly HashSet<int> _collisionVisitedEntities = new(516);

        private readonly Dictionary<int, bool> _entitiesOnWatch = new(256);

        private void WatchEntities(ImmutableArray<Entity> entities)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                if (entity.TryGetCollider() is not ColliderComponent collider)
                {
                    // This entity no longer has a collider
                    continue;
                }
                _entitiesOnWatch[entity.EntityId] = (collider.Layer & (CollisionLayersBase.TRIGGER)) == 0;
            }
        }

        public void OnDeactivated(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities);
        }
        public void OnActivated(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities);
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            WatchEntities(entities);
        }

        public void FixedUpdate(Context context)
        {
            if (_entitiesOnWatch.Count == 0)
            {
                return;
            }

            ImmutableArray<Entity> collisionsCache = context.World.GetEntitiesWith(typeof(CollisionCacheComponent));
            Quadtree qt = Quadtree.GetOrCreateUnique(context.World);

            foreach ((int entityId, bool thisIsAnActor) in _entitiesOnWatch)
            {
                Entity? entity = context.World.TryGetEntity(entityId);

                // Remove deactivated or destroyed entities from the collision cache and send exit messages.
                if (entity is null || !entity.IsActive || entity.IsDestroyed)
                {
                    foreach (Entity otherCached in collisionsCache)
                    {
                        if (PhysicsServices.RemoveFromCollisionCache(otherCached, entityId))
                        {
                            // Should we really send the ID of the deleted entity?
                            otherCached.SendOnCollisionMessage(entityId, CollisionDirection.Exit);

                            // There's no need to send the message back, as the entity is already destroyed.
                        }
                    }

                    entity?.RemoveCollisionCache();
                    continue;
                }

                // Check for active entities.
                CheckCollision(entity, qt, context.World);
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