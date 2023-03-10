using Bang;
using Bang.Components;
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
    [Filter(typeof(ITransformComponent), typeof(ColliderComponent))]
    [Watch(typeof(ITransformComponent))]
    public class TriggerPhysicsSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            CheckCollisions(world, entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            CheckCollisions(world, entities);
        }

        private static void CheckCollisions(World world, ImmutableArray<Entity> entities)
        {
            // Triggers can be touched 👋
            var triggers = PhysicsServices.FilterEntities(world, CollisionLayersBase.TRIGGER);

            // Actors and hitboxes can touch triggers   
            var hitboxes = PhysicsServices.FilterEntities(world, CollisionLayersBase.HITBOX | CollisionLayersBase.ACTOR);
            
            foreach (var e in entities)
            {
                if (e.IsDestroyed)
                    return;

                if (!e.HasCollider())
                {
                    e.RemoveIsColliding();
                    return;
                }

                // Hitboxes interact with triggers. Triggers don't touch other triggers.
                bool thisIsAnActor = (e.GetCollider().Layer & (CollisionLayersBase.TRIGGER)) == 0;
                var others = thisIsAnActor ?
                    triggers : hitboxes;

                foreach (var other in others)
                {
                    if (PhysicsServices.CollidesWith(e, other))
                    {
                        if (!PhysicsServices.IsCollidingWith(other, e.EntityId))
                        {
                            SendCollisionMessages(thisIsAnActor ? other : e, thisIsAnActor ? e : other, CollisionDirection.Enter);
                            PhysicsServices.AddIsColliding(other, e.EntityId);
                            PhysicsServices.AddIsColliding(e, other.EntityId);
                        }
                    }
                    else
                    {
                        bool shouldAlert = PhysicsServices.RemoveIsColliding(other, e.EntityId);
                        shouldAlert |= PhysicsServices.RemoveIsColliding(e, other.EntityId);
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

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            var colliders = world.GetEntitiesWith(typeof(IsCollidingComponent));
            foreach (var deleted in entities)
            {
                bool thisIsAnActor = (deleted.GetCollider().Layer & (CollisionLayersBase.TRIGGER)) == 0;
                
                foreach (var entity in colliders)
                {
                    if (PhysicsServices.RemoveIsColliding(entity, deleted.EntityId))
                    {
                        // Should we really send the ID of the deleted entity?
                        SendCollisionMessages(thisIsAnActor ? deleted : entity, thisIsAnActor ? deleted : entity, CollisionDirection.Exit);
                    }
                }
            }
        }
    }
}
