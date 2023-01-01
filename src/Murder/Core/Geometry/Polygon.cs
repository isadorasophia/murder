using Murder.Components;
using Murder.Core.Graphics;
using Murder.Services;
using System.Collections.Immutable;
using System.Reflection;

namespace Murder.Core.Geometry
{
    public readonly struct Polygon
    {
        public readonly ImmutableArray<Point> Vertices = ImmutableArray<Point>.Empty;
        
        public Polygon()
        {
            Vertices = ImmutableArray<Point>.Empty;
        }
        public Polygon(IEnumerable<Point> vertices) { Vertices = vertices.ToImmutableArray(); }

        public Polygon(IEnumerable<Point> vertices, Point position)
        {
            var builder = ImmutableArray.CreateBuilder<Point>();
            foreach (var v in vertices)
            {
                builder.Add(v + position);
            }

            Vertices = builder.ToImmutable();
        }

        internal bool HasVector2(Vector2 vector)
        {
            (float px, float py) = (vector.X, vector.Y);
            bool collision = false;

            int next = 0;
            for (int current = 0; current < Vertices.Length; current++)
            {
                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == Vertices.Length) next = 0;

                var vc = Vertices[current];    // c for "current"
                var vn = Vertices[next];       // n for "next"

                if (((vc.Y > py) != (vn.Y > py)) && (px < (vn.X - vc.X) * (py - vc.Y) / (vn.Y - vc.Y) + vc.X))
                {
                    collision = !collision;
                }
            }
            return collision;
        }

        internal bool Intersect(Circle circle)
        {
            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current = 0; current < Vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == Vertices.Length) next = 0;

                // get the Vectors at our current position
                // this makes our if statement a little cleaner
                var vc = Vertices[current];    // c for "current"
                var vn = Vertices[next];       // n for "next"

                // check for collision between the circle and
                // a line formed between the two vertices
                var line = new Line2(vc, vn);
                bool collision = line.IntersectsCircle(circle);
                if (collision) return true;
            }

            // the above algorithm only checks if the circle
            // is touching the edges of the polygon

            if (HasVector2(circle.Center))
                return true;

            return false;
        }


        internal bool Intersect(Rectangle rect)
        {
            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current = 0; current < Vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == Vertices.Length) next = 0;

                // get the Vectors at our current position
                // this makes our if statement a little cleaner
                var vc = Vertices[current];    // c for "current"
                var vn = Vertices[next];       // n for "next"

                // check for collision between the rect and
                // a line formed between the two vertices
                var line = new Line2(vc, vn);
                if (line.IntersectsRect(rect))
                    return true;
            }

            // the above algorithm only checks if the rectangle
            // is touching the edges of the polygon

            if (HasVector2(rect.TopLeft))
                return true;

            return false;
        }


        internal bool Intersects(Line2 line2)
        {
            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current = 0; current < Vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == Vertices.Length) next = 0;

                // get the Vectors at our current position
                // this makes our if statement a little cleaner
                var vc = Vertices[current];    // c for "current"
                var vn = Vertices[next];       // n for "next"

                // check for collision between the rect and
                // a line formed between the two vertices
                var line = new Line2(vc, vn);
                if (line.Intersects(line2))
                    return true;
            }

            // the above algorithm only checks if the rectangle
            // is touching the edges of the polygon

            if (HasVector2(line2.PointA))
                return true;
            
            return false;
        }

        internal bool Intersect(Polygon polygon)
        {
            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current = 0; current < Vertices.Length; current++)
            {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == Vertices.Length) next = 0;

                // get the Vectors at our current position
                // this makes our if statement a little cleaner
                var vc = Vertices[current];    // c for "current"
                var vn = Vertices[next];       // n for "next"

                // check for collision between the rect and
                // a line formed between the two vertices
                var line = new Line2(vc, vn);
                if (polygon.Intersects(line))
                    return true;
            }

            // the above algorithm only checks if the rectangle
            // is touching the edges of the polygon

            if (HasVector2(polygon.Vertices[0]))
                return true;

            return false;
        }

        internal Polygon AddPosition(Point position)
        {
            return new Polygon(Vertices, position);
        }

        public Rectangle GetBoundingBox()
        {
            var minX = Vertices.Min(v => v.X);
            var minY = Vertices.Min(v => v.Y);
            var maxX = Vertices.Max(v => v.X);
            var maxY = Vertices.Max(v => v.Y);

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public void Draw(Batch2D batch, Vector2 position, bool flip, Color color)
        {
            Point center = GetBoundingBox().Center.Point;
            
            if (flip)
            {
                for (int i = 0; i < Vertices.Length - 1; i++)
                {
                    Point pointA = Vertices[i].Mirror(center);
                    Point pointB = Vertices[i + 1].Mirror(center);
                    RenderServices.DrawLine(batch, pointA + position, pointB + position, color);
                }

                RenderServices.DrawLine(batch, Vertices[Vertices.Length - 1].Mirror(center) + position,
                    Vertices[0].Mirror(center) + position, color);
            }
            else
            {
                for (int i = 0; i < Vertices.Length - 1; i++)
                {
                    Point pointA = Vertices[i];
                    Point pointB = Vertices[i + 1];
                    RenderServices.DrawLine(batch, pointA + position, pointB + position, color);
                }

                RenderServices.DrawLine(batch, Vertices[Vertices.Length - 1] + position,
                    Vertices[0] + position, color);
            }
        }
    }
}
