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

namespace Murder.Systems.Physics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(PositionComponent), typeof(ColliderComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(IgnoreTriggersUntilComponent))]
    [Watch(typeof(PositionComponent))]
    [Requires(typeof(QuadtreeCalculatorSystem))]
    public class TriggerPhysicsSystem : IReactiveSystem
    {
        private readonly List<NodeInfo<Entity>> _others = [];

        // Used for reclycing over the same collision cache.
        private readonly HashSet<int> _collisionVisitedEntities = new(516);

        private readonly Dictionary<int, (Entity, ImmutableHashSet<int>)> _entitiesOnWatch = new(256);

        private void WatchEntities(ImmutableArray<Entity> entities, World world)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                if (_entitiesOnWatch.ContainsKey(entity.EntityId))
                {
                    continue;
                }

                if (entity.IsDestroyed)
                {
                    // Destroyed entities are not added to the watch list.
                    // But we message them immediately.

                    if (entity.TryGetCollisionCache() is not CollisionCacheComponent destroyedCollisionCache)
                    {
                        continue;
                    }

                    foreach (Entity other in destroyedCollisionCache.GetCollidingEntities(world))
                    {
                        entity.SendOnCollisionMessage(other.EntityId, CollisionDirection.Exit);
                        other.SendOnCollisionMessage(entity.EntityId, CollisionDirection.Exit);
                    }

                    continue;
                }

                if (entity.TryGetCollisionCache() is CollisionCacheComponent collisionCache)
                {
                    _entitiesOnWatch[entity.EntityId] = (entity, collisionCache.CollidingWith);
                }
                else
                {
                    _entitiesOnWatch[entity.EntityId] = (entity, []);
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

            foreach ((int entityId, (Entity entity, ImmutableHashSet<int> collidingWith)) in _entitiesOnWatch)
            {
                // Remove deactivated or destroyed entities from the collision cache and send exit messages.
                if (!entity.IsActive)
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
            if (!e.HasCollider())
            {
                e.RemoveCollisionCache();
                return;
            }

            if (ShouldIgnoreEntity(e))
            {
                return;
            }

            ColliderComponent collider = e.GetCollider();

            // Actors and Hitboxes interact with triggers.
            // Triggers don't touch other triggers, and so on.
            bool isActorOrHitbox = IsActorOrHitbox(e);
            bool isTrigger = IsTrigger(e);

            if (!isActorOrHitbox && !isTrigger)
            {
                return;
            }

            _others.Clear();
            Rectangle boundingBox = collider.GetBoundingBox(e.GetGlobalPosition().ToPoint(), e.FetchScale());
            qt.Collision.Retrieve(boundingBox, _others);

            CollisionCacheComponent collisionCache = e.TryGetCollisionCache() ?? new CollisionCacheComponent();
            bool changed = false;

            // Just sends messages and updates the *other* entity's cache.
            // Does NOT touch our local collisionCache or call SetCollisionCache on e.
            void NotifyRemoval(Entity other)
            {
                PhysicsServices.RemoveFromCollisionCache(other, e.EntityId);
                SendCollisionMessages(isActorOrHitbox ? other : e, isActorOrHitbox ? e : other, CollisionDirection.Exit);
            }

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
                    SendCollisionMessages(isActorOrHitbox ? other : e, isActorOrHitbox ? e : other, CollisionDirection.Exit);
                }

                return true;
            }

            for (int i = 0; i < _others.Count; i++)
            {
                NodeInfo<Entity> node = _others[i];
                Entity other = node.EntityInfo;

                if (!other.IsActive || other.EntityId == e.EntityId)
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

                bool isOtherActorOrHitbox = IsActorOrHitbox(other);
                bool isOtherTrigger = IsTrigger(other);

                if (!(isActorOrHitbox && isOtherTrigger) && !(isTrigger && isOtherActorOrHitbox))
                {
                    // Collider is not a valid pair, so we skip it.
                    // Valid pairs are:
                    // - Actor/Hitbox -> Trigger
                    // - Trigger -> Actor/Hitbox
                    continue;
                }

                _collisionVisitedEntities.Add(other.EntityId);

                // Check if there's a previous collision happening here
                if (!collisionCache.HasId(other.EntityId))
                {
                    if (PhysicsServices.CollidesWith(e, other)) // <=== This is the actual (and expensive) physics check
                    {
                        // If no previous collision is detected, send messages and add this ID to current collision cache.
                        SendCollisionMessages(isActorOrHitbox ? other : e, isActorOrHitbox ? e : other, CollisionDirection.Enter);
                        PhysicsServices.AddToCollisionCache(other, e.EntityId);

                        collisionCache = collisionCache.Add(other.EntityId);
                        changed = true;
                    }
                }
                else if (!PhysicsServices.CollidesWith(e, other))
                {
                    // Remove from our local copy, notify the other guy
                    collisionCache = collisionCache.Remove(other.EntityId);
                    changed = true;
                    NotifyRemoval(other);
                }
            }

            // Now, check for remaining entities that were not notified regarding the collision.
            foreach (int entityId in collisionCache.CollidingWith)
            {
                if (_collisionVisitedEntities.Contains(entityId))
                {
                    // Already verified, go away
                    continue;
                }

                if (world.TryGetEntity(entityId) is Entity other)
                {
                    NotifyRemoval(other);
                }

                collisionCache = collisionCache.Remove(entityId);
                changed = true;
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

        private bool ShouldIgnoreEntity(Entity e)
        {
            if (e.HasIgnoreTriggersUntil() || !e.HasCollider() || e.HasFollowPosition())
            {
                return true;
            }

            if (!IsActorOrHitbox(e) && e.HasMoveToPerfect())
            {
                return true;
            }

            return false;
        }

        private bool IsActorOrHitbox(Entity e)
        {
            ColliderComponent collider = e.GetCollider();
            return collider.HasLayer(CollisionLayersBase.ACTOR) || collider.HasLayer(CollisionLayersBase.HITBOX);
        }

        private bool IsTrigger(Entity e)
        {
            ColliderComponent collider = e.GetCollider();
            return collider.HasLayer(CollisionLayersBase.TRIGGER);
        }
    }
}