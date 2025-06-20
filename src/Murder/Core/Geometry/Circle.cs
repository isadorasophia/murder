using Murder.Components;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Core.Geometry
{
    public readonly struct Circle
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Radius;

        internal Vector2 Center => new(X, Y);

        public Circle(float radius)
        {
            X = 0;
            Y = 0;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public Circle AddPosition(PositionComponent position) => new Circle(X + position.X, Y + position.Y, Radius);
        public Circle AddPosition(Point position) => new Circle(X + position.X, Y + position.Y, Radius);
        public Circle AddPosition(Vector2 position) => new Circle(X + position.X, Y + position.Y, Radius);

        public bool Contains(Vector2 vector2) => (new Vector2(X, Y) - vector2).LengthSquared() < MathF.Pow(Radius, 2);
        public bool Contains(Point point) => (new Vector2(X, Y) - point).LengthSquared() < MathF.Pow(Radius, 2);

        internal ImmutableArray<Vector2> MakePolygon()
        {
            var builder = ImmutableArray.CreateBuilder<Vector2>();
            foreach (Vector2 point in GeometryServices.CreateCircle(Radius, 12))
            {
                builder.Add(point.Point() + new Point(X, Y));
            }

            return builder.ToImmutable();
        }

        public static int EstipulateSidesFromRadius(float radius)
        {
            return Math.Min(22, 6 + Calculator.FloorToInt(radius * 0.45f));
        }

    }
}