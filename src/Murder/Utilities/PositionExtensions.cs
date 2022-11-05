using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;

namespace Murder.Utilities
{
    public static class PositionExtensions
    {
        public static PositionComponent GetGlobalPosition(this Entity entity) => entity.GetPosition().GetGlobalPosition();

        public static void SetGlobalPosition(this Entity entity, PositionComponent position)
        {
            // This will make the value relative, if needed.
            if (entity.HasPosition() && entity.Parent is not null)
            {
                entity.SetPosition(position - entity.GetGlobalPosition());
            }
            else
            {
                entity.SetPosition(position);
            }
        }

        public static PositionComponent ToPosition(this in Point position) => new(position.X, position.Y);

        public static PositionComponent ToPosition(this in Vector2 position) => new(position.X, position.Y);

        public static PositionComponent ToPosition(this in System.Numerics.Vector2 position) => new(position.X, position.Y);

        public static Vector2 FromCellToVector2Position(this in Point point) => new(point.X * Grid.CellSize, point.Y * Grid.CellSize);

        public static Point FromWorldToLowerBoundGridPosition(this in Point point) => 
            new(Calculator.FloorToInt(point.X / Grid.CellSize), Calculator.FloorToInt(point.Y / Grid.CellSize));

        public static Vector2 FromCellToVector2CenterPosition(this in Point point) => new((point.X + .5f) * Grid.CellSize, (point.Y + .5f) * Grid.CellSize);

        public static Point FromCellToPointPosition(this in Point point) => new(point.X * Grid.CellSize, point.Y * Grid.CellSize);

        public static System.Numerics.Vector2 ToSysVector2(this PositionComponent position) => new(position.X, position.Y);

        public static Point ToPoint(this PositionComponent position) => new(Calculator.RoundToInt(position.X), Calculator.RoundToInt(position.Y));

        public static Point ToCellPoint(this PositionComponent position) => new(position.Cx, position.Cy);

        public static Vector2 ToVector2(this PositionComponent position) => new(position.X, position.Y);

        public static Vector2 AddToVector2(this PositionComponent position, Vector2 delta) => new(position.X + delta.X, position.Y + delta.Y);

        public static Vector2 AddToVector2(this PositionComponent position, float dx, float dy) => new(position.X + dx, position.Y + dy);

        public static PositionComponent Add(this PositionComponent position, float dx, float dy) => new(position.X + dx, position.Y + dy);

        public static PositionComponent Add(this PositionComponent position, Vector2 delta) => new(position.X + delta.X, position.Y + delta.Y);
        public static PositionComponent Add(this PositionComponent position, Point delta) => new(position.X + delta.X, position.Y + delta.Y);

        internal static bool IsInRange(this PositionComponent @this, PositionComponent other, int radius)
        {
            var distanceSq = (other.Pos - @this.Pos).LengthSquared();
            return distanceSq < radius * radius;
        }

        public static bool IsSameCell(this PositionComponent @this, PositionComponent other) => @this.Cx == other.Cx && @this.Cy == other.Cy;

        public static Point CellPoint(this PositionComponent @this) => new(@this.Cx, @this.Cy);


        /// <summary>
        /// Returns all the neighbour positions of a position with an offset of the grid size.
        /// This does not check the boundaries of a grid.
        /// </summary>
        internal static IEnumerable<PositionComponent> Neighbours(this PositionComponent p, int width, int height)
        {
            const int unit = Grid.CellSize;

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