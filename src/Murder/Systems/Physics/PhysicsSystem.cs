using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using static Murder.Services.PhysicsServices;
using Murder.Components;
using Murder.Core;
using Murder.Core.Physics;
using Murder.Utilities;
using Murder.Core.Geometry;
using Murder.Services;
using Murder.Messages;
using Bang.Components;

namespace Murder.Systems
{
    [Filter(typeof(ITransformComponent), typeof(VelocityComponent), typeof(AdvancedCollisionComponent))]
    public class PhysicsSystem : IFixedUpdateSystem
    {
        const int MAX_SLIDE = 4;

        public void FixedUpdate(Context context)
        {
            Map map = context.World.GetUnique<MapComponent>().Map;
            Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;
            List<(Entity entity, Rectangle boundingBox)> entityList = new();
            
            foreach (Entity e in context.Entities)
            {
                bool ignoreCollisions = false;
                var collider = e.GetCollider();
                if (collider.Layer != CollisionLayersBase.ACTOR)
                    ignoreCollisions = true;
                
                var id = e.EntityId;
                int mask = CollisionLayersBase.SOLID | CollisionLayersBase.HOLE;
                if (e.TryGetCustomCollisionMask() is CustomCollisionMask agent)
                    mask = agent.CollisionMask;

                // If the entity has a velocity, we'll move around by checking for collisions first.
                if (e.TryGetVelocity()?.Velocity is Vector2 rawVelocity)
                {
                    Vector2 targetPosition = e.GetGlobalTransform().Vector2 + rawVelocity * Murder.Game.FixedDeltaTime;

                    entityList.Clear();
                    qt.GetEntitiesAt(collider.GetBoundingBox(targetPosition.Point), ref entityList);
                    var collisionEntities = FilterPositionAndColliderEntities(entityList, CollisionLayersBase.SOLID | CollisionLayersBase.HOLE);
                    
                    IMurderTransformComponent relativeStartPosition = e.GetTransform();
                    Vector2 startPosition = relativeStartPosition.GetGlobal().Vector2;

                    // If the entity is inside another, let's see if we can pop it out
                    if (CollidesAt(map, id, collider, startPosition, collisionEntities, mask, out int inside))
                    {
                        var other = context.World.TryGetEntity(inside);
                        e.SendMessage(new IsInsideOfMessage(inside));
                        ignoreCollisions = true;
                        if (other != null)
                        {
                            var center = other.GetGlobalTransform().Vector2;
                            rawVelocity = (startPosition - center).Normalized() * 100;
                        }
                    }
                    
                    Vector2 velocity = rawVelocity * Murder.Game.FixedDeltaTime;

                    Vector2 shouldMove = Vector2.Zero;
                    Vector2 newVelocity = rawVelocity;

                    int xSign = Math.Sign(velocity.X);
                    float bufferX = velocity.X;

                    for (int xStep = 1; xStep < Math.Abs(velocity.X); xStep++)
                    {
                        bool hit = CollidesAt(map, id, collider, startPosition + new Vector2(xStep * xSign, 0), collisionEntities, mask, out int _);
                        if (hit)
                        {
                            for (int slide = 1; slide <= MAX_SLIDE * 2; slide++)
                            {
                                if (!CollidesAt(map, id, collider, startPosition + new Vector2(xStep * xSign, slide), collisionEntities, mask))
                                {
                                    newVelocity.Y += 450 * Murder.Game.FixedDeltaTime * Math.Clamp(slide, 0, MAX_SLIDE);
                                    break;
                                }

                                if (!CollidesAt(map, id, collider, startPosition + new Vector2(xStep * xSign, -slide), collisionEntities, mask))
                                {
                                    newVelocity.Y -= 450 * Murder.Game.FixedDeltaTime * Math.Clamp(slide, 0, MAX_SLIDE);
                                    break;
                                }
                            }
                        }

                        if (ignoreCollisions || !hit)
                        {
                            shouldMove.X += xSign;
                            bufferX -= xSign;
                        }
                        else
                        {
                            newVelocity.X = 0;
                            bufferX = 0;

                            if (hit)
                            {
                                e.SendMessage(new CollidedWithMessage(id));
                            }

                            break;
                        }
                    }
                    if (Math.Abs(bufferX) > float.Epsilon)
                    {
                        if (ignoreCollisions || !CollidesAt(map, id, collider, startPosition + new Vector2(shouldMove.X + xSign, 0), collisionEntities, mask))
                        {
                            shouldMove.X += bufferX;
                        }
                    }

                    int ySign = Math.Sign(velocity.Y);
                    float bufferY = velocity.Y;
                    for (int yStep = 1; yStep < Math.Abs(velocity.Y); yStep++)
                    {
                        bool hit = CollidesAt(map, id, collider, startPosition + new Vector2(shouldMove.X, yStep * ySign), collisionEntities, mask, out int _);

                        if (hit)
                        {
                            for (int slide = 1; slide <= MAX_SLIDE * 2; slide++)
                            {
                                if (!CollidesAt(map, id, collider, startPosition + new Vector2(shouldMove.X + slide, yStep * ySign), collisionEntities, mask))
                                {
                                    newVelocity.X += 450 * Game.FixedDeltaTime * Math.Clamp(slide, 0, MAX_SLIDE);
                                    break;
                                }

                                if (!CollidesAt(map, id, collider, startPosition + new Vector2(shouldMove.X - slide, yStep * ySign), collisionEntities, mask))
                                {
                                    newVelocity.X -= 450 * Game.FixedDeltaTime * Math.Clamp(slide, 0, MAX_SLIDE);
                                    break;
                                }
                            }
                        }
                        if (ignoreCollisions || !hit)
                        {
                            shouldMove.Y += ySign;
                            bufferY -= ySign;
                        }
                        else
                        {
                            newVelocity.Y = 0;
                            bufferY = 0;


                            if (hit)
                            {
                                e.SendMessage(new CollidedWithMessage(id));
                            }

                            break;
                        }
                    }
                    if (Math.Abs(bufferY) > float.Epsilon)
                    {
                        if (ignoreCollisions || !CollidesAt(map, id, collider, startPosition + new Vector2(shouldMove.X, shouldMove.Y + ySign), collisionEntities, mask))
                        {
                            shouldMove.Y += bufferY;
                        }
                    }

                    e.SetTransform(new PositionComponent(relativeStartPosition.ToVector2() + shouldMove));

                    if (newVelocity.LengthSquared() > 0.00001f)
                    {
                        e.SetVelocity(newVelocity);
                    }
                    else
                    {
                        e.RemoveComponent<VelocityComponent>();
                    }
                }
            }
        }
    }
}
