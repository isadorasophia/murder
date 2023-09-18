﻿using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Core.Geometry
{
    public readonly struct Polygon
    {
        public static readonly Polygon EMPTY = new();
        public static readonly Polygon DIAMOND = new(new Vector2[] {new (-10,0), new(0, -10), new(10, 0), new(0, 10) });
        public readonly ImmutableArray<Vector2> Vertices = ImmutableArray<Vector2>.Empty;
        
        public Polygon()
        {
            Vertices = ImmutableArray<Vector2>.Empty;
        }
        public Polygon(IEnumerable<Vector2> vertices) { Vertices = vertices.ToImmutableArray(); }

        public Polygon(IEnumerable<Vector2> vertices, Vector2 position)
        {
            var builder = ImmutableArray.CreateBuilder<Vector2>();
            foreach (var v in vertices)
            {
                builder.Add(v + position);
            }

            Vertices = builder.ToImmutable();
        }
        public static Polygon FromRectangle(int x, int y, int width, int height)
        {
            return new Polygon(new Vector2[] {
                new Vector2(x,y),
                new Vector2(x+ width,y),
                new Vector2(x + width,y + height),
                new Vector2(x,y + height)
            });
        }
        public bool Contains(Vector2 vector)
        {
            (float px, float py) = (vector.X, vector.Y);
            bool collision = false;

            int next;
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
        public bool Contains(Point point)
        {
            bool result = false;

            // go through each of the vertices, plus
            // the next vertex in the list
            int next;
            for (int current = 0; current < Vertices.Length; current++)
            {
                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current + 1;
                if (next == Vertices.Length) next = 0;

                var vc = Vertices[current];    // c for "current"
                var vn = Vertices[next];       // n for "next"

                // check if point is within polygon bounds
                if (((vc.Y > point.Y) != (vn.Y > point.Y)) &&
                    (point.X < (vn.X - vc.X) * (point.Y - vc.Y) / (vn.Y - vc.Y) + vc.X))
                {
                    result = !result;
                }
            }

            return result;
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

            if (Contains(circle.Center))
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

            if (Contains(rect.TopLeft))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a line intersects the polygon
        /// </summary>
        /// <param name="line2"></param>
        /// <returns></returns>
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

            if (Contains(line2.Start))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a line intersects with the polygon, and where.
        /// </summary>
        /// <param name="line2"></param>
        /// <param name="hitPoint"></param>
        /// <returns></returns>
        internal bool Intersects(Line2 line2, out Vector2 hitPoint)
        {
            bool intersects = false;
            hitPoint = line2.End;

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
                if (line.TryGetIntersectingPoint(line2, out var currentHitPoint))
                {
                    intersects = true;
                    if ((line2.Start - currentHitPoint).LengthSquared() < (line2.Start - hitPoint).LengthSquared())
                    {
                        hitPoint = currentHitPoint;
                    }
                }
            }

            // the above algorithm only checks if the rectangle
            // is touching the edges of the polygon

            if (Contains(line2.Start))
            {
                hitPoint = line2.Start;
                return true;
            }

            return intersects;
        }

        /// <summary>
        /// Check if a polygon is inside another, if they do, return the minimum translation vector to move the polygon out of the other.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="positionA"></param>
        /// <param name="positionB"></param>
        /// <returns></returns>
        public Vector2? Intersects(Polygon other, Vector2 positionA, Vector2 positionB)
        {
            List<Vector2> axes = GetNormals().ToList(); //[PERF] List? This can be optimized
            axes.AddRange(other.GetNormals());

            float minOverlap = float.MaxValue;
            Vector2? mtvAxis = null;

            foreach (Vector2 axis in axes)
            {
                (float Min, float Max) projectionA = ProjectOntoAxis(axis, positionA);
                (float Min, float Max) projectionB = other.ProjectOntoAxis(axis, positionB);

                if (!GeometryServices.CheckOverlap(projectionA, projectionB))
                {
                    return null; // No overlap, no collision
                }
                else
                {
                    float overlapA = projectionA.Max - projectionB.Min;
                    float overlapB = projectionB.Max - projectionA.Min;

                    bool useOverlapA = overlapA < overlapB;
                    float overlap = useOverlapA ? overlapA : overlapB;

                    if (overlap < minOverlap)
                    {
                        minOverlap = overlap;
                        mtvAxis = axis * (useOverlapA ? 1.0f : -1.0f);
                    }

                    //float overlap = Math.Min(projectionA.Max - projectionB.Min, projectionB.Max - projectionA.Min);

                    //if (overlap < minOverlap)
                    //{
                    //    minOverlap = overlap;
                    //    mtvAxis = axis;
                    //}
                }
            }

            return mtvAxis * minOverlap;
        }

        internal bool CheckOverlap(Polygon polygon)
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
                if (polygon.Intersects(line, out _))
                    return true;
            }

            // the above algorithm only checks if the rectangle
            // is touching the edges of the polygon

            if (Contains(polygon.Vertices[0]))
                return true;

            return false;
        }

        internal Polygon AddPosition(Point position)
        {
            return new Polygon(Vertices, position);
        }


        public (float Min, float Max) ProjectOntoAxis(Vector2 axis, Vector2 offset)
        {
            float min = Vector2.Dot(axis, Vertices[0] + offset);
            float max = min;

            for (int i = 1; i < Vertices.Length; i++)
            {
                float projection = Vector2.Dot(axis, Vertices[i] + offset);
                min = Math.Min(min, projection);
                max = Math.Max(max, projection);
            }

            return (min, max);
        }

        public IEnumerable<Vector2> GetNormals()
        {
            // [PERF] We can cache the normals on creation

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector2 currentVertex = Vertices[i];
                Vector2 nextVertex = Vertices[(i + 1) % Vertices.Length]; // Next + wrap around

                Vector2 edge = nextVertex - currentVertex;
                Vector2 normal = new Vector2(edge.Y, -edge.X);
                
                yield return normal.Normalized();
            }
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
            Vector2 center = GetBoundingBox().Center;
            
            if (flip)
            {
                for (int i = 0; i < Vertices.Length - 1; i++)
                {
                    Vector2 pointA = Vertices[i].Mirror(center);
                    Vector2 pointB = Vertices[i + 1].Mirror(center);
                    RenderServices.DrawLine(batch, pointA + position, pointB + position, color);
                }

                RenderServices.DrawLine(batch, Vertices[Vertices.Length - 1].Mirror(center) + position,
                    Vertices[0].Mirror(center) + position, color);
            }
            else
            {
                for (int i = 0; i < Vertices.Length - 1; i++)
                {
                    Vector2 pointA = Vertices[i];
                    Vector2 pointB = Vertices[i + 1];
                    RenderServices.DrawLine(batch, pointA + position, pointB + position, color);
                }

                RenderServices.DrawLine(batch, Vertices[Vertices.Length - 1] + position,
                    Vertices[0] + position, color);
            }
        }

        /// <summary>
        /// Returns this polygon with a new position. The position is calculated using the vertice 0 as origin.
        /// </summary>
        /// <param name="target">Target position for vertice 0</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Polygon AtPosition(Point target)
        {
            var translatedVertices = new List<Vector2>();
            var delta = target - Vertices[0];
            foreach (var vertex in Vertices)
            {
                translatedVertices.Add(vertex + delta);
            }
            return new Polygon(translatedVertices);
        }

        public Polygon AddPosition(Vector2 add)
        {
            var translatedVertices = new List<Vector2>();
            foreach (var vertex in Vertices)
            {
                translatedVertices.Add(vertex + add);
            }
            return new Polygon(translatedVertices);
        }

        public Polygon WithVerticeAt(int index, Vector2 target)
        {
            return new Polygon(Vertices.SetItem(index, target));
        }
        public Polygon WithNewVerticeAt(int index, Vector2 target)
        {
            return new Polygon(Vertices.Insert(index, target));
        }
        public Polygon RemoveVerticeAt(int index)
        {
            return new Polygon(Vertices.RemoveAt(index));
        }

        public bool IsConvex()
        {
            if (Vertices.Length < 4)
                return true; // A triangle is always convex

            bool? isPositive = null;
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vector2 a = Vertices[i];
                Vector2 b = Vertices[(i + 1) % Vertices.Length];
                Vector2 c = Vertices[(i + 2) % Vertices.Length];

                Vector2 ab = b - a;
                Vector2 bc = c - b;

                float cross = ab.X * bc.Y - ab.Y * bc.X;
                if (isPositive == null)
                {
                    isPositive = cross > 0;
                }
                else if ((cross > 0) != isPositive.Value)
                {
                    return false; // Not convex
                }
            }

            return true; // Convex
        }

        public static List<int> FindConcaveVertices(ImmutableArray<Vector2> points)
        {
            List<int> concaveVertices = new List<int>();
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 prev = points[(i - 1 + points.Length) % points.Length];
                Vector2 current = points[i];
                Vector2 next = points[(i + 1) % points.Length];

                Vector2 edge1 = current - prev;
                Vector2 edge2 = next - current;

                float crossProduct = edge1.X * edge2.Y - edge1.Y * edge2.X;

                if (crossProduct < 0)  // If cross product is negative, the vertex is concave
                {
                    concaveVertices.Add(i);
                }
            }
            return concaveVertices;
        }

        public static List<Polygon> EarClippingTriangulation(Polygon polygon)
        {
            List<Polygon> triangles = new List<Polygon>();

            List<int> reflexVertices = FindConcaveVertices(polygon.Vertices);

            List<Vector2> remainingPolygon = new List<Vector2>(polygon.Vertices);

            while (remainingPolygon.Count > 2)
            {
                for (int i = 0; i < remainingPolygon.Count; i++)
                {
                    Vector2 a = remainingPolygon[(i - 1 + remainingPolygon.Count) % remainingPolygon.Count];
                    Vector2 b = remainingPolygon[i];
                    Vector2 c = remainingPolygon[(i + 1) % remainingPolygon.Count];

                    // Check if it's a reflex vertex
                    if (reflexVertices.Contains(i))
                    {
                        continue;
                    }

                    // Check if any point lies within the triangle
                    bool hasPointInside = false;
                    for (int j = 0; j < remainingPolygon.Count; j++)
                    {
                        if (j == i || j == (i - 1 + remainingPolygon.Count) % remainingPolygon.Count || j == (i + 1) % remainingPolygon.Count)
                        {
                            continue;
                        }
                        if (IsPointInTriangle(remainingPolygon[j], a, b, c))
                        {
                            hasPointInside = true;
                            break;
                        }
                    }

                    if (!hasPointInside)
                    {
                        var newTriangle = new Polygon(new Vector2[] { a, b, c });

                        bool mergeSuccess = false;
                        // Try merging this new triangle with an old one
                        for (int j = 0; j < triangles.Count; j++)
                        {
                            if (Polygon.TryMerge(newTriangle, triangles[j], 1f, out var merged))
                            {
                                triangles[j] = merged;
                                mergeSuccess = true;
                            }
                        }

                        if (!mergeSuccess)
                        {
                            // This is an "ear," so clip it off
                            triangles.Add(newTriangle);
                        }
                        
                        remainingPolygon.RemoveAt(i);
                        reflexVertices = FindConcaveVertices(remainingPolygon.ToImmutableArray());
                        break;
                    }
                }
            }

            return triangles;
        }

        public static bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            // Barycentric coordinate method
            float denominator = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);
            float alpha = ((b.Y - c.Y) * (point.X - c.X) + (c.X - b.X) * (point.Y - c.Y)) / denominator;
            float beta = ((c.Y - a.Y) * (point.X - c.X) + (a.X - c.X) * (point.Y - c.Y)) / denominator;
            float gamma = 1.0f - alpha - beta;

            return alpha > 0 && beta > 0 && gamma > 0;
        }

        /// <summary>
        /// This doesn't work yet
        /// </summary>
        public static List<Polygon> PartitionToConvex(Polygon concave)
        {
            List<Polygon> convexPolygons = new List<Polygon>();

            List<int> concaveVertices = FindConcaveVertices(concave.Vertices);
            List<Vector2> centralPolygonPoints = new List<Vector2>(concave.Vertices);

            while (concaveVertices.Count > 0)
            {
                int concaveIndex = concaveVertices[0];

                // Find the best vertex to create a diagonal
                // This is a simplified example; you should replace it with a more robust algorithm
                int bestVertex = (concaveIndex + 2) % concave.Vertices.Length;

                // Create a new convex polygon using the vertices from concaveIndex to bestVertex
                List<Vector2> newPolygonPoints = new List<Vector2>();
                for (int i = concaveIndex; i != bestVertex; i = (i + 1) % concave.Vertices.Length)
                {
                    newPolygonPoints.Add(concave.Vertices[i]);
                }
                newPolygonPoints.Add(concave.Vertices[bestVertex]);

                Polygon newPolygon = new Polygon(newPolygonPoints.ToArray());
                convexPolygons.Add(newPolygon);

                // Add this diagonal to the central polygon
                centralPolygonPoints.Insert(concaveIndex + 1, concave.Vertices[bestVertex]);

                // Remove the vertices used in the new polygon from the list of concave vertices
                concaveVertices.RemoveAll(index => newPolygonPoints.Contains(concave.Vertices[index]));
            }

            // Finally, add the central polygon to the list of convex polygons
            Polygon centralPolygon = new Polygon(centralPolygonPoints.ToArray());
            convexPolygons.Add(centralPolygon);

            return convexPolygons;
        }
        public Line2[] GetLines()
        {
            int vertexCount = Vertices.Length;
            if (vertexCount < 2) return new Line2[0];  // Can't form a line with less than 2 points

            Line2[] lines = new Line2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 start = Vertices[i];
                Vector2 end = Vertices[(i + 1) % vertexCount]; // Loop back to the first vertex for the last line
                lines[i] = new Line2(start, end);
            }

            return lines;
        }


        public static Polygon? MergePolygons(Polygon a, Polygon b)
        {
            HashSet<Line2> lineSet = new HashSet<Line2>();

            // Add all lines from both polygons to the set.
            // This will automatically remove duplicates since HashSet does not allow them.
            foreach (var line in a.GetLines())
            {
                lineSet.Add(line);
            }
            foreach (var line in b.GetLines())
            {
                lineSet.Add(line);
            }

            // Re-constitute the vertices based on the unique line segments
            Line2 firstLine = lineSet.First();
            lineSet.Remove(firstLine);

            List<Vector2> mergedVertices = new List<Vector2> { firstLine.Start, firstLine.End };

            while (lineSet.Count > 0)
            {
                var last = mergedVertices.Last();
                
                Line2? success = null;
                foreach (var candidate in lineSet)
                {
                    if (candidate.Start.Equals(last))
                    {
                        success = candidate;
                        break;
                    }
                }
                if (success != null)
                {
                    mergedVertices.Add(success.Value.End);
                    lineSet.Remove(success.Value);
                }
                else
                { 
                    // Failed!
                    return Polygon.EMPTY;
                }
            }

            // Check and correct the orientation to be clockwise
            if (!IsClockwise(mergedVertices))
            {
                mergedVertices.Reverse();
            }

            return new Polygon(mergedVertices.ToArray());
        }
        public static bool IsClockwise(List<Vector2> vertices)
        {
            float area = 0;

            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector2 curr = vertices[i];
                Vector2 next = vertices[(i + 1) % vertices.Count]; // Wrap around at the end
                area += (next.X - curr.X) * (next.Y + curr.Y);
            }

            return area >= 0;
        }


        public static bool TryMerge(Polygon a, Polygon b, float minDistance, out Polygon result)
        {
            int commonCount = 0;
            HashSet<Vector2> uniqueVertices = new HashSet<Vector2>();

            // Find common vertices
            foreach (Vector2 vertex in a.Vertices)
            {
                uniqueVertices.Add(vertex);

                int index = b.FindVertexIndexAtPosition(vertex, minDistance);
                if (index >= 0)
                {
                    commonCount++;
                    for (int i = 0; i < b.Vertices.Length; i++)
                    {
                        uniqueVertices.Add(b.Vertices[(index + i) % b.Vertices.Length]);
                    }
                }
            }
            
            // If they share two or more vertices, they share an edge
            if (commonCount >= 2)
            {
                // Assume the vertices are listed in clockwise order.
                result = new Polygon(uniqueVertices);

                // Now check if the resulting polygon is convex
                if (result.IsConvex())
                {
                    return true;
                }
            }

            result = Polygon.EMPTY;
            return false;
        }

        private int FindVertexIndexAtPosition(Vector2 vertex, float minDistance)
        {
            var minDistanceSq = minDistance * minDistance;
            for (int i = 0; i < Vertices.Length; i++)
            {
                if ((vertex - Vertices[i]).LengthSquared() < minDistanceSq)
                {
                    return i;
                }
            }

            return -1;
        }

        public static List<Vector2> ReorderVertices(List<Vector2> vertices)
        {
            // Compute the centroid of the polygon
            Vector2 centroid = new Vector2(0, 0);
            foreach (Vector2 vertex in vertices)
            {
                centroid += vertex;
            }
            centroid /= vertices.Count;

            // Sort vertices based on angle with respect to the centroid
            vertices.Sort((a, b) =>
            {
                float angleA = (float)Math.Atan2(a.Y - centroid.Y, a.X - centroid.X);
                float angleB = (float)Math.Atan2(b.Y - centroid.Y, b.X - centroid.X);
                return angleA.CompareTo(angleB);
            });

            return vertices;
        }

    }
}
