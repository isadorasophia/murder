using Murder.Components;
using Murder.Core.Geometry;
using Bang.Entities;
using Murder.Utilities;

namespace Murder.Services
{
    public static class ColliderServices
    {
        /// <summary>
        /// Returns the center point of an entity with all its colliders.
        /// </summary>
        public static Point FindCenter(Entity e)
        {
            Point position = e.HasTransform() ? e.GetGlobalTransform().Point : Point.Zero;
            if (e.TryGetCollider() is not ColliderComponent collider)
            {
                return position;
            }

            IntRectangle rect = collider.GetBoundingBox(position);
            return rect.CenterPoint;
        }
    }
}
