using Murder.Core.Geometry;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Ai
{
    public partial class HAAStar
    {
        private readonly struct Cluster
        {
            public readonly Point P;

            public int X => P.X;
            public int Y => P.Y;

            public readonly int Top;
            public readonly int Bottom;

            public readonly int Left;
            public readonly int Right;

            public Cluster(Point coord)
            {
                P = coord;

                Top = coord.Y * CLUSTER_SIZE;
                Bottom = (coord.Y + 1) * CLUSTER_SIZE - 1;

                Left = coord.X * CLUSTER_SIZE;
                Right = (coord.X + 1) * CLUSTER_SIZE - 1;
            }
        }

        public readonly struct Edge : IEquatable<Edge>
        {
            public readonly Point A;
            public readonly Point B;

            public Edge(Point a, Point b) => (A, B) = (a, b);

            public override bool Equals(object? obj) => obj is Edge e && this.Equals(e);

            public bool Equals(Edge other)
            {
                return (A == other.A && B == other.B) || (A == other.B && B == other.A);
            }

            public override int GetHashCode()
            {
                return A.GetHashCode() + B.GetHashCode();
            }
        }

        public class Node
        {
            public readonly Point P;

            public readonly Point Cluster;

            public readonly int Weight = 1;

            public int X => P.X;
            public int Y => P.Y;

            public Node(Point p, Point c, int weight) =>
                (P, Cluster, Weight) = (p, c, weight);

            public readonly Dictionary<Point, double> Neighbours = new();

            private readonly HashSet<Point> _neighboursCache = new();
            private readonly Dictionary<Point, ComplexDictionary<Point, Point>> _paths = new();

            public void AddEdge(Point p, ComplexDictionary<Point, Point> path, double cost)
            {
                Neighbours.Add(p, cost);

                _neighboursCache.Add(p);
                _paths.Add(p, path);
            }

            public void RemoveEdge(Point p)
            {
                Neighbours.Remove(p);

                _neighboursCache.Remove(p);
                _paths.Remove(p);
            }

            public bool HasNeighbour(Point p)
            {
                return _neighboursCache.Contains(p);
            }

            public ComplexDictionary<Point, Point> PathTo(Point p)
            {
                return _paths[p];
            }
        }

        private void AddEdge(Node n1, Node n2)
        {
            n1.AddEdge(n2.P, new ComplexDictionary<Point, Point> { { n1.P, n2.P } }, n2.Weight);
            n2.AddEdge(n1.P, new ComplexDictionary<Point, Point> { { n2.P, n1.P } }, n1.Weight);
        }

        private void AddEdge(Node n1, Node n2, ComplexDictionary<Point, Point> path, double cost, bool directed = false)
        {
            ComplexDictionary<Point, Point> path1;
            ComplexDictionary<Point, Point> path2;

            if (path.ContainsKey(n1.P))
            {
                path1 = path;
                path2 = path.Reverse(n2.P, n1.P);
            }
            else
            {
                path1 = path.Reverse(n1.P, n2.P);
                path2 = path;
            }

            n1.AddEdge(n2.P, path1, cost);

            if (!directed)
            {
                n2.AddEdge(n1.P, path2, cost);
            }
        }

        private Node AddNode(Point p, int weight)
        {
            if (_nodes.TryGetValue(p, out Node? n))
            {
                // Node is already present!
                return n;
            }

            Point clusterId = CoordinateToCluster(p);
            n = new(p, clusterId, weight);
            _nodes.Add(p, n);

            if (!_nodesPerCluster.ContainsKey(clusterId))
            {
                _nodesPerCluster[clusterId] = new();
            }

            _nodesPerCluster[clusterId].Add(p);

            return n;
        }

        private void RemoveNode(Node n)
        {
            _nodes.Remove(n.P);

            if (_nodesPerCluster.TryGetValue(n.Cluster, out var nodes))
            {
                nodes.Remove(n.P);

                foreach (Point node in nodes)
                {
                    _nodes[node].RemoveEdge(n.P);
                }
            }
        }
    }
}