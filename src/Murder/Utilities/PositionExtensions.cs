using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using System.Numerics;

namespace Murder.Utilities
{
    public static class PositionExtensions
    {
        public static IMurderTransformComponent GetGlobalTransform(this Entity entity)
        {
            if (entity.TryGetMurderTransform() is not IMurderTransformComponent transform)
            {
                if (!entity.IsActive)
                {
                    GameLogger.Error($"Tried to retrieve position of a dead entity: {entity.EntityId}! Expect things to fall apart.");
                }
                else
                {
                    GameLogger.Error($"Tried to retrieve position of an entity without position: {entity.EntityId}! Expect things to fall apart.");
                }

                return new PositionComponent();
            }

            return transform.GetGlobal();
        }

        public static void SetGlobalTransform<T>(this Entity entity, T transform) where T : IMurderTransformComponent
        {
            // This will make the value relative, if needed.
            if (entity.HasTransform() && entity.TryFetchParent() is Entity parent)
            {
                if (parent.HasTransform())
                {
                    entity.SetTransform(transform.Subtract(parent.GetGlobalTransform()));
                }
                else
                {
                    entity.SetTransform(transform);
                }
            }
            else if (transform is not null)
            {
                entity.SetTransform(transform);
            }
        }

        public static void SetLocalPosition(this Entity entity, Vector2 position)
        {
            IMurderTransformComponent newTransform = entity.GetMurderTransform().With(position);
            entity.SetTransform(newTransform);
        }
        public static void SetGlobalPosition(this Entity entity, Vector2 position)
        {
            IMurderTransformComponent newTransform;
            if (entity.HasTransform())
            {
                IMurderTransformComponent transform = entity.GetGlobalTransform();
                if (transform.Vector2 == position)
                {
                    return;
                }

                newTransform = entity.GetGlobalTransform().With(position);
            }
            else
            {
                newTransform = new PositionComponent(position);
            }

            // This will make the value relative, if needed.
            if (entity.HasTransform() && entity.TryFetchParent() is Entity parent && parent.HasTransform())
            {
                entity.SetTransform(newTransform.Subtract(parent.GetGlobalTransform()));
            }
            else
            {
                entity.SetTransform(newTransform);
            }
        }

        public static PositionComponent ToPosition(this in Point position) => new(position.X, position.Y);

        public static PositionComponent ToPosition(this in Vector2 position) => new(position.X, position.Y);

        public static Vector2 FromCellToVector2Position(this in Point point) => new(point.X * Grid.CellSize, point.Y * Grid.CellSize);

        public static Point FromWorldToLowerBoundGridPosition(this in Point point) =>
            new(Calculator.FloorToInt(point.X / Grid.CellSize), Calculator.FloorToInt(point.Y / Grid.CellSize));

        public static Vector2 FromCellToVector2CenterPosition(this in Point point) => new((point.X + .5f) * Grid.CellSize, (point.Y + .5f) * Grid.CellSize);

        public static Point FromCellToPointPosition(this in Point point) => new(point.X * Grid.CellSize, point.Y * Grid.CellSize);

        public static Vector2 ToSysVector2(this PositionComponent position) => new(position.X, position.Y);

        public static Point ToPoint(this PositionComponent position) => new(Calculator.RoundToInt(position.X), Calculator.RoundToInt(position.Y));

        public static Point ToCellPoint(this IMurderTransformComponent position) => new(position.Cx, position.Cy);

        public static Point ToCellPoint(this Vector2 v) => new((int)Math.Floor(v.X / Grid.CellSize), (int)Math.Floor(v.Y / Grid.CellSize));

        public static Vector2 ToVector2(this IMurderTransformComponent position) => new(position.X, position.Y);

        public static Vector2 AddToVector2(this IMurderTransformComponent position, Vector2 delta) => new(position.X + delta.X, position.Y + delta.Y);

        public static Vector2 AddToVector2(this PositionComponent position, float dx, float dy) => new(position.X + dx, position.Y + dy);

        public static PositionComponent Add(this PositionComponent position, float dx, float dy) => new(position.X + dx, position.Y + dy);

        public static PositionComponent Add(this PositionComponent position, Vector2 delta) => new(position.X + delta.X, position.Y + delta.Y);
        public static PositionComponent Add(this PositionComponent position, Point delta) => new(position.X + delta.X, position.Y + delta.Y);

        internal static bool IsInRange(this IMurderTransformComponent @this, IMurderTransformComponent other, int radius)
        {
            var distanceSq = (other.Vector2 - @this.Vector2).LengthSquared();
            return distanceSq < radius * radius;
        }

        public static bool IsSameCell(this IMurderTransformComponent @this, IMurderTransformComponent other) => @this.Cx == other.Cx && @this.Cy == other.Cy;

        public static Point CellPoint(this IMurderTransformComponent @this) => new(@this.Cx, @this.Cy);

        /// <summary>
        /// Returns all the neighbour positions of a position with an offset of the grid size.
        /// This does not check the boundaries of a grid.
        /// </summary>
        internal static IEnumerable<Vector2> Neighbours(this Vector2 p, int width, int height, float unit = 1)
        {
            unit *= Grid.CellSize;

            // [ ] [ ] [ ]
            // [ ]  x  [ ]
            // [ ] [x] [ ]
            if (p.Y + unit < height)
            {
                yield return new(p.X, p.Y + unit);
            }

            // [ ] [ ] [ ]
            // [ ]  x  [ ]
            // [ ] [ ] [x]
            if (p.Y + unit < height && p.X + unit < width)
            {
                yield return new(p.X + unit, p.Y + unit);
            }

            // [ ] [ ] [ ]
            // [ ]  x  [ ]
            // [x] [ ] [ ]
            if (p.X > 0 && p.Y + unit < height)
            {
                yield return new(p.X - unit, p.Y + unit);
            }

            // [x] [ ] [ ]
            // [ ]  x  [ ]
            // [ ] [ ] [ ]
            if (p.X > 0 && p.Y > 0)
            {
                yield return new(p.X - unit, p.Y - unit);
            }

            // [ ] [x] [ ]
            // [ ]  x  [ ]
            // [ ] [ ] [ ]
            if (p.Y > 0)
            {
                yield return new(p.X, p.Y - unit);
            }

            // [ ] [ ] [x]
            // [ ]  x  [ ]
            // [ ] [ ] [ ]
            if (p.Y > 0 && p.X + unit < width)
            {
                yield return new(p.X + unit, p.Y - unit);
            }

            // [ ] [ ] [ ]
            // [x]  x  [ ]
            // [ ] [ ] [ ]
            if (p.X > 0)
            {
                yield return new(p.X - unit, p.Y);
            }

            // [ ] [ ] [ ]
            // [ ]  x  [x]
            // [ ] [ ] [ ]
            if (p.X + unit < width)
            {
                yield return new(p.X + unit, p.Y);
            }
        }
    }
}