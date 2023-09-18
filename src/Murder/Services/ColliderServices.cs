﻿using Murder.Components;
using Murder.Core.Geometry;
using Bang.Entities;
using Murder.Utilities;
using Murder.Core;
using System.Numerics;

namespace Murder.Services
{
    public static class ColliderServices
    {
        /// <summary>
        /// Returns the center point of an entity with all its colliders.
        /// </summary>
        public static Point FindCenter(Entity e)
        {
            IntRectangle rect = GetBoundingBox(e);
            return rect.CenterPoint;
        }

        public static IntRectangle GetBoundingBox(Entity e)
        {
            Point position = e.HasTransform() ? e.GetGlobalTransform().Point : Point.Zero;
            if (e.TryGetCollider() is not ColliderComponent collider)
            {
                return IntRectangle.Empty;
            }

            return collider.GetBoundingBox(position);
        }

        public static Vector2 SnapToRelativeGrid(Vector2 position, Vector2 origin)
        {
            Vector2 relative = position - origin;

            return new Vector2(
                relative.X - relative.X % Grid.CellSize,
                relative.Y - relative.Y % Grid.CellSize) + origin;
        }

        public static IntRectangle ToGrid(this IntRectangle rectangle)
        {
            return new IntRectangle(
                Calculator.FloorToInt((float)rectangle.X / Grid.CellSize),
                Calculator.FloorToInt((float)rectangle.Y / Grid.CellSize),
                Calculator.CeilToInt((float)rectangle.Width / Grid.CellSize),
                Calculator.CeilToInt((float)rectangle.Height / Grid.CellSize));
        }

        public static IntRectangle[] GetCollidersBoundingBox(Entity e, bool gridCoordinates)
        {
            Point position = e.HasTransform() ? e.GetGlobalTransform().Point : Point.Zero;
            if (e.TryGetCollider() is not ColliderComponent collider)
            {
                return Array.Empty<IntRectangle>();
            }

            return PhysicsServices.GetCollidersBoundingBox(collider, position, gridCoordinates);
        }
    }
}
