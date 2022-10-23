using Newtonsoft.Json;
using Murder.Core.Geometry;
using Bang.Entities;
using Murder.Components;

namespace Murder.Core.Physics
{
    public class Quadtree
    {
        [JsonIgnore]
        public readonly QTNode<Entity> Collision;

        [JsonIgnore]
        public readonly QTNode<(
            Entity entity,
            PositionComponent position,
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

            // TODO: [Step 1] Run generator and uncomment this!
            //foreach (var e in entities)
            //{
            //    var pos = e.GetGlobalPosition();
            //    if (e.TryGetCollider() is ColliderComponent collider)
            //    {
            //        Collision.Insert(e, collider.GetBoundingBox(pos));
            //    }

            //    if (e.TryGetPushAway() is PushAwayComponent pushAway)
            //    {
            //        PushAway.Insert(
            //            (
            //                e,
            //                e.GetGlobalPosition(),
            //                e.GetPushAway(),
            //                e.TryGetVelocity()?.Velocity ?? Vector2.Zero
            //            ), new Rectangle(pos.X, pos.Y, pushAway.Size, pushAway.Size));
            //    }
            //}
        }

        public void GetEntitiesAt(Rectangle boundingBox, ref List<(Entity entity, Rectangle boundingBox)> list)
        {
            list.Clear();
            Collision.Retrieve(list, boundingBox);
        }
    }
}
