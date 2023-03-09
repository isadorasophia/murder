using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Physics;
using Murder.Messages.Physics;
using Murder.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (!e.HasCollider())
                {
                    e.RemoveIsColliding();
                    return;
                }
                
                // Hitboxes interact with triggers. Triggers don't touch other triggers.
                var others = (e.GetCollider().Layer & (CollisionLayersBase.TRIGGER)) == 0 ?
                    triggers : hitboxes;

                foreach (var other in others)
                {
                    if (PhysicsServices.CollidesWith(e, other))
                    {
                        e.SendMessage(new OnTriggerEnteredMessage(other.EntityId));
                        PhysicsServices.AddIsColliding(other, e.EntityId);
                    }
                    else
                    {
                        if (PhysicsServices.RemoveIsColliding(other, e.EntityId))
                        {
                            e.SendMessage(new OnTriggerExitMessage(other.EntityId));
                        }
                    }
                }
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            var colliders = world.GetEntitiesWith(typeof(IsCollidingComponent));
            foreach (var deleted in entities)
            {
                foreach (var entity in colliders)
                {
                    if (PhysicsServices.RemoveIsColliding(entity, deleted.EntityId))
                    {
                        // Should we really send the ID of the deleted entity?
                        entity.SendMessage(new OnTriggerExitMessage(deleted.EntityId));
                    }
                }
            }
        }
    }
}
