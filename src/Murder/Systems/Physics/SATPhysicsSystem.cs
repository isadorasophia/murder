using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Threading.Tasks;

namespace Murder.Systems.Physics
{
    [Filter(typeof(ITransformComponent), typeof(VelocityComponent))]
    public class SATPhysicsSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            Map map = context.World.GetUnique<MapComponent>().Map;
            Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;
            List<(Entity entity, Rectangle boundingBox)> entityList = new();
            foreach (Entity e in context.Entities)
            {
                bool ignoreCollisions = false;
                var collider = e.TryGetCollider();
                var id = e.EntityId;

                int mask = CollisionLayersBase.SOLID | CollisionLayersBase.HOLE;
                if (e.TryGetCustomCollisionMask() is CustomCollisionMask agent)
                    mask = agent.CollisionMask;

                // If the entity has a velocity, we'll move around by checking for collisions first.
                if (e.TryGetVelocity()?.Velocity is Vector2 currentVelocity)
                {
                    Vector2 velocity = currentVelocity * Game.FixedDeltaTime;
                    IMurderTransformComponent relativeStartPosition = e.GetTransform();
                    Vector2 startPosition = relativeStartPosition.GetGlobal().ToVector2();

                    if (MathF.Abs(currentVelocity.X) < 0.5f && MathF.Abs(currentVelocity.Y) < 0.5f)
                    {
                        ignoreCollisions = true;
                    }

                    if (collider is not null && collider.Value.Layer == CollisionLayersBase.TRIGGER)
                    {
                        ignoreCollisions = true;
                    }

                    if (collider is null)
                    {
                        ignoreCollisions = true;
                    }

                    if (ignoreCollisions)
                    {
                        e.SetGlobalPosition(startPosition + velocity);
                    }
                    else
                    {
                        entityList.Clear();
                        qt.GetEntitiesAt(collider!.Value.GetBoundingBox((startPosition + velocity).Point), ref entityList);
                        var collisionEntities = PhysicsServices.FilterPositionAndColliderEntities(entityList, CollisionLayersBase.SOLID | CollisionLayersBase.HOLE);
                        
                        if (PhysicsServices.GetMtvAt(map, id, collider.Value, startPosition + velocity, collisionEntities, mask, out int hitId) is Vector2 translationVector
                                && translationVector.HasValue)
                        {
                            e.SendMessage(new CollidedWithMessage(hitId));
                            e.SetGlobalPosition(startPosition + velocity - translationVector);

                            // Slide the speed accordingly
                            Vector2 edgePerpendicularToMTV = new Vector2(-translationVector.Y, translationVector.X);
                            Vector2 normalizedEdge = edgePerpendicularToMTV.Normalized();
                            float dotProduct = Vector2.Dot(currentVelocity, normalizedEdge);
                            currentVelocity = normalizedEdge * dotProduct;
                        }
                        else
                        {
                            // No collision detected, move freely!
                            e.SetGlobalPosition(startPosition + velocity);
                        }
                    }

                    // Makes sure that the velocity snaps to zero if it's too small.
                    if (currentVelocity.Manhattan() > 0.0001f)
                    {
                        e.SetVelocity(currentVelocity);
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
