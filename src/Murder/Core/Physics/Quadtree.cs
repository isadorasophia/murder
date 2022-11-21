using Newtonsoft.Json;
using Murder.Core.Geometry;
using Bang.Entities;
using Murder.Components;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Core.Physics
{
    public class Quadtree
    {
        [JsonIgnore]
        public readonly QTNode<Entity> Collision;

        [JsonIgnore]
        public readonly QTNode<(
            Entity entity,
            IMurderTransformComponent position,
            PushAwayComponent pushAway,
            Vector2 velocity
            )> PushAway;

        public Quadtree(Rectangle mapBounds)
        {
            Collision = new(0, mapBounds);
            PushAway = new(0, mapBounds);
        }

        public void UpdateQuadTree(IEnumerable<Entity> entities)
        {
            Collision.Clear();
            PushAway.Clear();

            foreach (var e in entities)
            {
                IMurderTransformComponent pos = e.GetGlobalTransform();
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    Collision.Insert(e, collider.GetBoundingBox(pos.Point));
                }

                if (e.TryGetPushAway() is PushAwayComponent pushAway)
                {
                    PushAway.Insert(
                        (
                            e,
                            e.GetGlobalTransform(),
                            e.GetPushAway(),
                            e.TryGetVelocity()?.Velocity ?? Vector2.Zero
                        ), new Rectangle(pos.X, pos.Y, pushAway.Size, pushAway.Size));
                }
            }
        }

        public void GetEntitiesAt(Rectangle boundingBox, out List<(Entity entity, Rectangle boundingBox)> list)
        {
            list = new();
            
            Collision.Retrieve(boundingBox, ref list);
        }
    }
}
