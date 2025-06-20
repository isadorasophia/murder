using Murder.Services;
using Murder.Utilities;
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
        public Vector2 Start
        {
            get { return new Vector2(X1, Y1); }
        }

        /// <summary>
        /// The second point of a line as a vector2.
        /// </summary>
        public Vector2 End
        {
            get { return new Vector2(X2, Y2); }
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
            Vector2 A = new(X1, Y1);
            Vector2 B = new(X2, Y2);
            Vector2 C = new(other.X1, other.Y1);
            Vector2 D = new(other.X2, other.Y2);

            Vector2 CmP = new(C.X - A.X, C.Y - A.Y);
            Vector2 r = new(B.X - A.X, B.Y - A.Y);
            Vector2 s = new(D.X - C.X, D.Y - C.Y);

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
            Vector2 a = line1.End - line1.Start;
            Vector2 b = line2.Start - line2.End;
            Vector2 c = line1.Start - line2.Start;

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

        internal bool HasPoint(Vector2 point, float buffer = float.Epsilon)
        {
            float d1 = (point - Start).Length();
            float d2 = (point - End).Length();
            float len = Length();
            float sum = d1 + d2;

            return sum >= len - buffer && sum <= len + buffer;
        }

        public float LengthSquared() => (Start - End).LengthSquared();
        public float Length() => (Start - End).Length();

        public override string ToString()
        {
            return "{X1: " + X1 + " Y1: " + Y1 + " X2: " + X2 + " Y2: " + Y2 + "}";
        }

        public bool TryGetIntersectingPoint(Rectangle rect, out Vector2 hitPoint) => TryGetIntersectingPoint(rect.X, rect.Y, rect.Width, rect.Height, out hitPoint);
        public bool TryGetIntersectingPoint(float x, float y, float width, float height, out Vector2 hitPoint)
        {
            Vector2 startPos = new(X1, Y1);
            Vector2 candidate = new(X2, Y2);
            bool hitSomething = false;

            // The ray starts inside a tile!
            if (GeometryServices.InRect(X1, Y1, x, y, width, height))
            {
                hitPoint = new(X1, Y1);
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
            if (GeometryServices.InRect(X1, Y1, x, y, width, height)) return true;
            if (GeometryServices.InRect(X2, Y2, x, y, width, height)) return true;
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
            bool inside1 = circle.Contains(Start);
            bool inside2 = circle.Contains(End);
            if (inside1 || inside2) return true; // Either the star or end points are inside the circle

            float distX = X1 - X2;
            float distY = Y1 - Y2;
            float lenSquared = (distX * distX) + (distY * distY);
            float dot = (((circle.X - X1) * (X2 - X1)) + ((circle.Y - Y1) * (Y2 - Y1))) / lenSquared;
            Point closest = new(X1 + (dot * (X2 - X1)), Y1 + (dot * (Y2 - Y1)));

            return circle.Contains(closest) && HasPoint(closest);
        }

        /// <summary>
        /// Returns if this line touches a circle and the first point of intersection.
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="hitPoint"></param>
        /// <returns></returns>
        public bool TryGetIntersectingPoint(Circle circle, out Vector2 hitPoint)
        {
            // This would be a bad optmization actualy
            //if (!IntersectsCircle(circle))
            //{
            //    hitPoint = Vector2.Zero;
            //    return false;
            //}

            float dx, dy, A, B, C, det, t;

            dx = X2 - X1;
            dy = Y2 - Y1;

            A = dx * dx + dy * dy;
            B = 2 * (dx * (X1 - circle.X) + dy * (Y1 - circle.Y));
            C = (X1 - circle.X) * (X1 - circle.X) + (Y1 - circle.Y) * (Y1 - circle.Y) - circle.Radius * circle.Radius;

            det = B * B - 4 * A * C;
            if ((A <= 0.0000001) || (det < 0))
            {
                // No real solutions.
                hitPoint = new Vector2(float.NaN, float.NaN);
                return false;
            }
            else if (det == 0)
            {
                // One solution.
                t = -B / (2 * A);

                hitPoint = new Vector2(X1 + t * dx, Y1 + t * dy);
                if (HasPoint(hitPoint))
                    return true;

                return false;
            }
            else
            {
                // Two solutions.
                t = (float)((-B + Math.Sqrt(det)) / (2 * A));
                var intersection1 = new Vector2(X1 + t * dx, Y1 + t * dy);
                t = (float)((-B - Math.Sqrt(det)) / (2 * A));
                var intersection2 = new Vector2(X1 + t * dx, Y1 + t * dy);

                if ((Start - intersection1).LengthSquared() < (Start - intersection2).LengthSquared())
                    hitPoint = intersection1;
                else
                    hitPoint = intersection2;

                bool isInSegment = HasPoint(hitPoint, 1);
                return isInSegment;
            }
        }
        public bool GetClosestPoint(Vector2 point, float maxRange, out Vector2 closest)
        {
            Vector2 lineDir = (End - Start).Normalized();
            Vector2 pointDir = point - Start;

            float t = Vector2.Dot(pointDir, lineDir);

            if (t < 0)
            {
                closest = Start;
            }
            else if (t > Vector2.Distance(Start, End))
            {
                closest = End;
            }
            else
            {
                closest = Start + lineDir * t;
            }

            if (Vector2.Distance(point, closest) > maxRange)
            {
                return false;
            }

            return true;
        }

        public Vector2 GetClosestPoint(Vector2 point)
        {
            Vector2 lineDir = (End - Start).Normalized();
            Vector2 pointDir = point - Start;

            float t = Vector2.Dot(pointDir, lineDir);

            if (t < 0)
            {
                return Start;
            }
            else if (t > Vector2.Distance(Start, End))
            {
                return End;
            }
            else
            {
                return Start + lineDir * t;
            }
        }

        internal Line2 AddPosition(Point point)
        {
            return new Line2(X1 + point.X, Y1 + point.Y, X2 + point.X, Y2 + point.Y);
        }

        #endregion
    }
}