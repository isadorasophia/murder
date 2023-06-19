using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Messages.Physics;
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
            var qt = Quadtree.GetOrCreateUnique(world);
            foreach (var e in entities)
            {
                _others.Clear();
                if (e.HasIgnoreTriggersUntil())
                    // [BUG] This should never happen
                    continue;

                if (!e.IsActive)
                    continue;

                if (!e.HasCollider())
                {
                    e.RemoveCollisionCache();
                    continue;
                }

                var collider = e.GetCollider();

                // Actors and Hitboxes interact with triggers.
                // Triggers don't touch other triggers, and so on.
                bool thisIsAnActor = (collider.Layer & (CollisionLayersBase.TRIGGER)) == 0;

                //var others = thisIsAnActor ?
                //    triggers : hitboxes;
                qt.Collision.Retrieve(collider.GetBoundingBox(e.GetGlobalTransform().Point), _others);
                
                var collisionCache = e.TryGetCollisionCache() ?? new CollisionCacheComponent();
                foreach (var node in _others)
                {
                    Entity other = node.EntityInfo;
                    if (!other.IsActive)
                        continue;

                    if (other.EntityId == e.EntityId)
                        continue;

                    if (other.EntityId == e.Parent || other.Parent == e.EntityId)
                        continue;

                    if (other.HasIgnoreTriggersUntil())
                        continue;
                    
                    var otherCollider = other.GetCollider();
                    if (thisIsAnActor && otherCollider.Layer == CollisionLayersBase.ACTOR || !thisIsAnActor && otherCollider.Layer == CollisionLayersBase.TRIGGER)
                        continue;
                    
                    if (PhysicsServices.CollidesWith(e, other)) // This is the actual physics check
                    {
                        // Check if there's a previous collision happening here
                        if (!collisionCache.HasId(other.EntityId))
                        {
                            // If no previous collision is detected, send messages and add this ID to current collision cache.
                            SendCollisionMessages(thisIsAnActor ? other : e, thisIsAnActor ? e : other, CollisionDirection.Enter);
                            PhysicsServices.AddToCollisionCache(other, e.EntityId);
                            e.SetCollisionCache(collisionCache.Add(other.EntityId));
                        }
                    }
                    else
                    {
                        bool shouldAlert = PhysicsServices.RemoveFromCollisionCache(other, e.EntityId);
                        shouldAlert = PhysicsServices.RemoveFromCollisionCache(e, other.EntityId) || shouldAlert;
                        if (shouldAlert)
                        {
                            SendCollisionMessages(thisIsAnActor ? other : e, thisIsAnActor ? e : other, CollisionDirection.Exit);
                        }
                    }
                }
            }
        }

        private static void SendCollisionMessages(Entity trigger, Entity actor, CollisionDirection direction)
        {
            actor.SendMessage(new OnTriggerEnteredMessage(trigger.EntityId, direction));
            trigger.SendMessage(new OnActorEnteredOrExitedMessage(actor.EntityId, direction));
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
            foreach (var deleted in entities)
            {
                bool thisIsAnActor = (deleted.GetCollider().Layer & (CollisionLayersBase.TRIGGER)) == 0;

                foreach (var entity in colliders)
                {
                    if (PhysicsServices.RemoveFromCollisionCache(entity, deleted.EntityId))
                    {
                        // Should we really send the ID of the deleted entity?
                        SendCollisionMessages(thisIsAnActor ? deleted : entity, thisIsAnActor ? deleted : entity, CollisionDirection.Exit);
                    }
                }

                deleted.RemoveCollisionCache();
            }
        }
    }
}
