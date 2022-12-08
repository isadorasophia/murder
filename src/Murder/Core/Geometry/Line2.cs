using Murder.Utilities;
using static System.Net.WebRequestMethods;
using System.Numerics;

namespace Murder.Core.Geometry
{
    /// <summary>
    /// Class for a simple line with two points.
    /// This is based on a Otter2d class: https://github.com/kylepulver/Otter/blob/master/Otter/Utility/Line2.cs
    /// </summary>
    public readonly struct Line2
    {

        #region Public Fields

        /// <summary>
        /// The X position for the first point.
        /// </summary>
        public readonly float X1;

        /// <summary>
        /// The Y position for the first point.
        /// </summary>
        public readonly float Y1;

        /// <summary>
        /// The X position for the second point.
        /// </summary>
        public readonly float X2;

        /// <summary>
        /// The Y position for the second point.
        /// </summary>
        public readonly float Y2;

        #endregion

        #region Public Properties

        /// <summary>
        /// The first point of the line as a vector2.
        /// </summary>
        public Vector2 PointA
        {
            get { return new Vector2(X1, Y1); }
        }

        /// <summary>
        /// The second point of a line as a vector2.
        /// </summary>
        public Vector2 PointB
        {
            get { return new Vector2(X2, Y2); }
        }

        /// <summary>
        /// A in the line equation Ax + By = C.
        /// </summary>
        public float A
        {
            get { return Y2 - Y1; }
        }

        /// <summary>
        /// B in the line equation Ax + By = C.
        /// </summary>
        public float B
        {
            get { return X1 - X2; }
        }

        /// <summary>
        /// C in the line equation Ax + By = C.
        /// </summary>
        public float C
        {
            get { return A * X1 + B * Y1; }
        }

        /// <summary>
        /// The bottom most Y position of the line.
        /// </summary>
        public float Bottom
        {
            get { return Math.Max(Y1, Y2); }
        }

        /// <summary>
        /// The top most Y position of the line.
        /// </summary>
        public float Top => Math.Min(Y1, Y2);
        
        /// <summary>
        /// The left most X position of the line.
        /// </summary>
        public float Left => Math.Min(X1, X2);

        /// <summary>
        /// The right most X position of the line.
        /// </summary>
        public float Right => Math.Max(X1, X2);

        public float Width => Right - Left;
        public float Height => Bottom - Top;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Line2.
        /// </summary>
        /// <param name="x1">X of the first point</param>
        /// <param name="y1">Y of the first point</param>
        /// <param name="x2">X of the second point</param>
        /// <param name="y2">Y of the second point</param>
        public Line2(float x1, float y1, float x2, float y2)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
        }

        /// <summary>
        /// Create a new Line2.
        /// </summary>
        /// <param name="start">X,Y of the first point</param>
        /// <param name="end">X,Y of the second point</param>
        public Line2(Vector2 start, Vector2 end)
        {
            X1 = start.X;
            Y1 = start.Y;
            X2 = end.X;
            Y2 = end.Y;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Intersection test on another line. (http://ideone.com/PnPJgb)
        /// </summary>
        /// <param name="other">The line to test against</param>
        /// <returns></returns>
        public bool Intersects(Line2 other)
        {
            //A = X1, Y1; B = X2, Y2; C = other.X1, other.Y1; D = other.X2, other.Y2;
            Vector2 A = new Vector2(X1, Y1);
            Vector2 B = new Vector2(X2, Y2);
            Vector2 C = new Vector2(other.X1, other.Y1);
            Vector2 D = new Vector2(other.X2, other.Y2);

            Vector2 CmP = new Vector2(C.X - A.X, C.Y - A.Y);
            Vector2 r = new Vector2(B.X - A.X, B.Y - A.Y);
            Vector2 s = new Vector2(D.X - C.X, D.Y - C.Y);

            float CmPxr = CmP.X * r.Y - CmP.Y * r.X;
            float CmPxs = CmP.X * s.Y - CmP.Y * s.X;
            float rxs = r.X * s.Y - r.Y * s.X;

            if (CmPxr == 0f)
            {
                // Lines are collinear, and so intersect if they have any overlap

                return ((C.X - A.X < 0f) != (C.X - B.X < 0f))
                        || ((C.Y - A.Y < 0f) != (C.Y - B.Y < 0f));
            }

            if (rxs == 0f)
                return false; // Lines are parallel.

            float rxsr = 1f / rxs;
            float t = CmPxs * rxsr;
            float u = CmPxr * rxsr;

            return (t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f);
        }
        public bool TryGetIntersectingPoint(Line2 other, out Vector2 hitPoint)
        {
            return TryGetIntersectingPoint(this, other, out hitPoint);
        }

        public static bool TryGetIntersectingPoint(Line2 line1, Line2 line2, out Vector2 hitPoint)
        {
            Vector2 a = line1.PointB - line1.PointA;
            Vector2 b = line2.PointA - line2.PointB;
            Vector2 c = line1.PointA - line2.PointA;

            float alphaNumerator = b.Y * c.X - b.X * c.Y;
            float betaNumerator = a.X * c.Y - a.Y * c.X;
            float denominator = a.Y * b.X - a.X * b.Y;

            if (Calculator.IsAlmostZero(denominator))
            {
                hitPoint = default;
                return false;
            }
            else if (denominator > 0)
            {
                if (alphaNumerator < 0 || alphaNumerator > denominator || betaNumerator < 0 || betaNumerator > denominator)
                {
                    hitPoint = default;
                    return false;
                }
            }
            else if (alphaNumerator > 0 || alphaNumerator < denominator || betaNumerator > 0 || betaNumerator < denominator)
            {
                hitPoint = default;
                return false;
            }
            // If lines intersect, then the intersection point can be found
            float px = (line1.X1 * line1.Y2 - line1.Y1 * line1.X2) * (line2.X1 - line2.X2) - (line1.X1 - line1.X2) * (line2.X1 * line2.Y2 - line2.Y1 * line2.X2);
            float py = (line1.X1 * line1.Y2 - line1.Y1 * line1.X2) * (line2.Y1 - line2.Y2) - (line1.Y1 - line1.Y2) * (line2.X1 * line2.Y2 - line2.Y1 * line2.X2);

            hitPoint = new Vector2(px, py) / denominator;
            return true;
        }

        internal bool HasPoint(Vector2 point)
        {
            float d1 = (PointA - point).LengthSquared();
            float d2 = (PointB - point).LengthSquared();
            float sum = d1 + d2;
            float len = LengthSquared();

            float buffer = float.Epsilon;
            return sum >= len - buffer && sum <= len + buffer;
        }

        public float LengthSquared() => (PointA - PointB).LengthSquared();

        public override string ToString()
        {
            return "{X1: " + X1 + " Y1: " + Y1 + " X2: " + X2 + " Y2: " + Y2 + "}";
        }

        public bool TryGetIntersectingPoint(Rectangle rect, out Vector2 hitPoint) => TryGetIntersectingPoint(rect.X, rect.Y, rect.Width, rect.Height, out hitPoint);
        public bool TryGetIntersectingPoint(float x, float y, float width, float height, out Vector2 hitPoint)
        {
            Vector2 startPos = new Vector2(X1, Y1);
            Vector2 candidate = new Vector2(X2, Y2);
            bool hitSomething = false;

            // The ray starts inside a tile!
            if (Calculator.InRect(X1, Y1, x, y, width, height))
            {
                hitPoint = new Vector2(X1, Y1);
                return true;
            }
            
            Vector2 hitLine;
            if (TryGetIntersectingPoint(new Line2(x, y, x + width, y), out hitLine))
            {
                if ((hitLine - startPos).LengthSquared() < (candidate - startPos).LengthSquared())
                {
                    candidate = hitLine;
                    hitSomething = true;
                }
            }
            if (TryGetIntersectingPoint(new Line2(x + width, y, x + width, y + height), out hitLine))
            {
                if ((hitLine - startPos).LengthSquared() < (candidate - startPos).LengthSquared())
                {
                    candidate = hitLine;
                    hitSomething = true;
                }
            }
            if (TryGetIntersectingPoint(new Line2(x, y + height, x + width, y + height), out hitLine))
            {
                if ((hitLine - startPos).LengthSquared() < (candidate - startPos).LengthSquared())
                {
                    candidate = hitLine;
                    hitSomething = true;
                }
            }
            if (TryGetIntersectingPoint(new Line2(x, y, x, y + height), out hitLine))
            {
                if ((hitLine - startPos).LengthSquared() < (candidate - startPos).LengthSquared())
                {
                    candidate = hitLine;
                    hitSomething = true;
                }
            }

            hitPoint = candidate;
            return hitSomething;
        }

        public bool IntersectsRect(Rectangle rect) => IntersectsRect(rect.X, rect.Y, rect.Width, rect.Height);
        /// <summary>
        /// Check intersection against a rectangle.
        /// </summary>
        /// <param name="x">X Position of the rectangle.</param>
        /// <param name="y">Y Position of the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <returns>True if the line intersects any line on the rectangle, or if the line is inside the rectangle.</returns>
        public bool IntersectsRect(float x, float y, float width, float height)
        {
            if (Calculator.InRect(X1, Y1, x, y, width, height)) return true;
            if (Calculator.InRect(X2, Y2, x, y, width, height)) return true;
            if (Intersects(new Line2(x, y, x + width, y))) return true;
            if (Intersects(new Line2(x + width, y, x + width, y + height))) return true;
            if (Intersects(new Line2(x + width, y + height, x, y + height))) return true;
            if (Intersects(new Line2(x, y + height, x, y))) return true;

            return false;
        }

        /// <summary>
        /// Check the intersection against a circle.
        /// </summary>
        public bool IntersectsCircle(Circle circle)
        {
            bool inside1 = circle.Contains(PointA);
            bool inside2 = circle.Contains(PointB);
            if (inside1 || inside2) return true; // Either the star or end points are inside the circle

            var len = LengthSquared();
            
            float dot = (((circle.X - X1) * (X2 - X1)) + ((circle.Y - Y1) * (Y2 - Y1))) / MathF.Pow(len, 2);
            Vector2 closest = new Vector2(X1 + (dot * (X2 - X1)), Y1 + (dot * (Y2 - Y1)));
            
            bool onSegment = HasPoint(closest);
            if (!onSegment) return false; // The closest point is outside the line

            return circle.Contains(closest);
        }
        
        #endregion

    }
}