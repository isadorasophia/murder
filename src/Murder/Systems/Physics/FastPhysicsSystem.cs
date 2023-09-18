﻿using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Physics;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems
{
    [Filter(typeof(ITransformComponent), typeof(VelocityComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(AdvancedCollisionComponent))]
    internal class FastPhysicsSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            Map map = context.World.GetUnique<MapComponent>().Map;
            Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;
            List<NodeInfo<Entity>> entityList = new();
                
            foreach (Entity e in context.Entities)
            {
                bool ignoreCollisions = false;
                var collider = e.TryGetCollider();
                var id = e.EntityId;

                int mask = CollisionLayersBase.SOLID | CollisionLayersBase.HOLE;
                if (e.TryGetCustomCollisionMask() is CustomCollisionMask agent)
                    mask = agent.CollisionMask;

                // If the entity has a velocity, we'll move around by checking for collisions first.
                if (e.TryGetVelocity()?.Velocity is Vector2 rawVelocity)
                {
                    Vector2 velocity = rawVelocity * Game.FixedDeltaTime;
                    IMurderTransformComponent relativeStartPosition = e.GetTransform();
                    Vector2 startPosition = relativeStartPosition.GetGlobal().ToVector2();
                    Vector2 newVelocity = rawVelocity;
                    Vector2 shouldMove = Vector2.Zero;

                    if (MathF.Abs(rawVelocity.X) <0.5f && MathF.Abs(rawVelocity.Y) < 0.5f)
                    {
                        ignoreCollisions = true;
                    }

                    if (collider is not null && collider.Value.Layer == CollisionLayersBase.TRIGGER)
                    {
                        ignoreCollisions = true;    
                    }
                    
                    // We will try without this for now, if units start to get stuck inside other colliders, we will need to change this
                    //if (ignoreCollisions || collider is null || PhysicsServices.CollidesAt(map, id, collider.Value, startPosition, collisionEntities))
                    //{
                    //    ignoreCollisions = true;
                    //}

                    if (ignoreCollisions || collider is null)
                    {
                        shouldMove = velocity;
                    }
                    else 
                    {
                        entityList.Clear();
                        qt.GetCollisionEntitiesAt(collider.Value.GetBoundingBox((startPosition + velocity).Point()), entityList);
                        var collisionEntities = PhysicsServices.FilterPositionAndColliderEntities(entityList, CollisionLayersBase.SOLID | CollisionLayersBase.HOLE);

                        if (!PhysicsServices.CollidesAt(map, id, collider.Value, startPosition + velocity, collisionEntities, mask, out int hitId))
                        {
                            shouldMove = velocity;
                        }
                        else
                        {
                            e.SendMessage(new CollidedWithMessage(hitId));
                            if (ignoreCollisions || !PhysicsServices.CollidesAt(map, id, collider!.Value, startPosition + new Vector2(velocity.X, 0), collisionEntities, mask))
                            {
                                shouldMove.X = velocity.X;
                            }
                            else
                            {
                                newVelocity.X *= .5f;
                            }
                            if (ignoreCollisions || !PhysicsServices.CollidesAt(map, id, collider!.Value, startPosition + new Vector2(0, velocity.Y), collisionEntities, mask))
                            {
                                shouldMove.Y = velocity.Y;
                            }
                            else
                            {
                                newVelocity.Y *= .5f;
                            }
                        }
                    }

                    if (shouldMove.Manhattan()>0)
                    {
                        e.SetTransform(new PositionComponent(relativeStartPosition.ToVector2() + shouldMove));
                    }

                    if (newVelocity.Manhattan() > 0.00001f)
                    {
                        e.SetVelocity(newVelocity);
                    }
                    else
                    {
                        e.RemoveVelocity();
                    }
                }
            }
        }
    }
}
