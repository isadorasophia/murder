using Murder.Core.Geometry;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Core.Particles
{
    public readonly struct EmitterShape
    {
        public readonly EmitterShapeKind Kind = EmitterShapeKind.Point;

        public readonly Rectangle Rectangle = Rectangle.One;
        public readonly Line2 Line = new(new(0, 0), new(1, 0));
        public readonly Circle Circle = new(1);

        public EmitterShape() { }

        public Vector2 GetRandomPosition(Random random)
        {
            switch (Kind)
            {
                case EmitterShapeKind.Point:
                    // Just return the central point
                    return Vector2.Zero;    

                case EmitterShapeKind.Line:
                    // Lerp between the start and the end of the line.
                    return Vector2.Lerp(Line.Start, Line.End, random.NextFloat());

                case EmitterShapeKind.Circle:
                case EmitterShapeKind.CircleOutline:
                    // Creates a normalized vector, then multiply it by the radius
                    return Vector2Helper.FromAngle(random.NextFloat() * MathF.PI * 2) * random.NextFloat() * Circle.Radius + Circle.Center;

                case EmitterShapeKind.Rectangle:
                    // Simply randomize the width and add the position
                    return new Vector2(random.NextFloat(Rectangle.Width) + Rectangle.X, random.NextFloat(Rectangle.Height) + Rectangle.Y);

                default:
                    throw new Exception("Unknown emmiter shape");
            }
        }
    }
}