using Bang.Components;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Services;
using System.Numerics;

namespace Murder.Utilities
{
    public static class PositionExtensions
    {
        public static Vector2 GetGlobalPosition(this Entity entity)
        {
            if (entity.TryGetPosition() is not PositionComponent positionComponent)
            {
                if (!entity.IsActive)
                {
                    GameLogger.Error($"Tried to retrieve position of a dead entity: {entity.EntityId}! Expect things to fall apart.");
                }
                else
                {
                    GameLogger.Error($"Tried to retrieve position of an entity without position: {entity.EntityId}! Expect things to fall apart.");
                }

                return Vector2.Zero;
            }

            return positionComponent.GetGlobal();
        }

        public static Vector2? TryGetGlobalPosition(this Entity entity) =>
            entity.GetGlobalPositionIfValid();

        public static void SetLocalPosition(this Entity entity, Vector2 position)
        {
            PositionComponent newPosition = entity.GetPosition().WithLocal(position.X, position.Y);
            entity.SetPosition(newPosition);
        }

        public static void SetGlobalPosition(this Entity entity, Vector2 position)
        {
            if (entity.TryGetPosition() is PositionComponent transform)
            {
                entity.SetPosition(transform.SetGlobal(position));
            }
            else
            {
                entity.SetPosition(position);
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

        public static Point ToCellPoint(this PositionComponent position) => position.Vector2.ToCellPoint();

        public static Point ToCellPoint(this Vector2 v) => new(Calculator.FloorToInt(v.X / Grid.CellSize), Calculator.FloorToInt(v.Y / Grid.CellSize));

        public static Vector2 ToVector2(this PositionComponent position) => position.Vector2;

        public static Vector2 AddToVector2(this PositionComponent position, Vector2 delta) => new(position.X + delta.X, position.Y + delta.Y);

        public static Vector2 AddToVector2(this PositionComponent position, float dx, float dy) => new(position.X + dx, position.Y + dy);

        public static PositionComponent Add(this PositionComponent position, float dx, float dy) => new(position.X + dx, position.Y + dy);

        public static PositionComponent Add(this PositionComponent position, Vector2 delta) => new(position.X + delta.X, position.Y + delta.Y);
        public static PositionComponent Add(this PositionComponent position, Point delta) => new(position.X + delta.X, position.Y + delta.Y);

        internal static bool IsInRange(this PositionComponent @this, PositionComponent other, int radius)
        {
            var distanceSq = (other.Vector2 - @this.Vector2).LengthSquared();
            return distanceSq < radius * radius;
        }

        public static bool IsSameCell(this PositionComponent @this, PositionComponent other) => @this.CellPoint() == other.CellPoint();

        public static Point CellPoint(this PositionComponent @this) => @this.Vector2.ToCellPoint();

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