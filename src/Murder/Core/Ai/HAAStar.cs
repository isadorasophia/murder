using Murder.Core.Geometry;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Core.Ai
{
    public partial class HAAStar
    {
        public const int CLUSTER_SIZE = 5;

        private readonly ImmutableDictionary<Point, Cluster> _clusters;

        public readonly int ClusterWidth;

        public readonly int ClusterHeight;

        private readonly Dictionary<Edge, ImmutableArray<ImmutableArray<Edge>>> _neighbours = new();

        private readonly Dictionary<Point, Node> _nodes = new();

        public ImmutableDictionary<Point, Node> DebugNodes = ImmutableDictionary<Point, Node>.Empty;

        private readonly Dictionary<Point, List<Point>> _nodesPerCluster = new();

        private Task? _pendingCalculate;

        private CancellationTokenSource _cts = new();

        private readonly object _lock = new();

        public HAAStar(int width, int height)
        {
            var builder = ImmutableDictionary.CreateBuilder<Point, Cluster>();

            ClusterWidth = Calculator.CeilToInt(width / CLUSTER_SIZE);
            ClusterHeight = Calculator.CeilToInt(height / CLUSTER_SIZE);

            for (int y = 0; y < ClusterHeight; y++)
            {
                for (int x = 0; x < ClusterWidth; x++)
                {
                    Point p = new(x, y);
                    builder.Add(p, new(p));
                }
            }

            _clusters = builder.ToImmutable();
        }

        private Point CoordinateToCluster(Point p) =>
            new(Calculator.FloorToInt(p.X / CLUSTER_SIZE), Calculator.FloorToInt(p.Y / CLUSTER_SIZE));

        private void Preprocess(Map map, CancellationToken token)
        {
            lock (_lock)
            {
                Task? pendingCalculate = _pendingCalculate;
                if (pendingCalculate is not null && pendingCalculate.IsCompleted)
                {
                    pendingCalculate = null;
                }

                _pendingCalculate = Task.Run(async delegate
                {
                    // so the rest of this does not operate under the lock.
                    await Task.Yield();

                    if (pendingCalculate is not null)
                    {
                        try
                        {
                            await pendingCalculate;
                        }
                        catch
                        {
                            // dismiss exceptions at this point.
                        }
                    }

                    Clear();

                    AbstractMaze(map, token);
                    BuildGraph(map, token);

                    DebugNodes = _nodes.ToImmutableDictionary();
                });
            }
        }

        public void Refresh(Map map)
        {
            _cts.Cancel();
            _cts = new();

            Preprocess(map, _cts.Token);
        }

        private void Clear()
        {
            _neighbours.Clear();

            _nodes.Clear();
            DebugNodes.Clear();

            _nodesPerCluster.Clear();
        }

        private void AbstractMaze(Map map, CancellationToken token)
        {
            foreach (Point cluster in _clusters.Keys)
            {
                foreach (Point n in cluster.Neighbours(ClusterWidth, ClusterHeight))
                {
                    Edge e = new(cluster, n);
                    if (!_neighbours.ContainsKey(e))
                    {
                        var entrances = BuildEntrances(map, _clusters[cluster], _clusters[n]);
                        if (!entrances.IsEmpty)
                        {
                            _neighbours.Add(e, entrances);
                        }
                    }

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        private void BuildGraph(Map map, CancellationToken token)
        {
            // First, start by creading nodes and edges for all the inter-edges.
            foreach ((Edge edge, ImmutableArray<ImmutableArray<Edge>> entrances) in _neighbours)
            {
                foreach (ImmutableArray<Edge> edges in entrances)
                {
                    Debug.Assert(!edges.IsEmpty);

                    // pick a point the be the node for our graph.
                    Edge midEdge = edges[edges.Length / 2];

                    Node n1 = AddNode(midEdge.A, map.WeightAt(midEdge.A));
                    Node n2 = AddNode(midEdge.B, map.WeightAt(midEdge.B));

                    AddEdge(n1, n2);
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }

            foreach (var (cluster, nodes) in _nodesPerCluster)
            {
                foreach (Point p1 in nodes)
                {
                    Node n1 = _nodes[p1];

                    foreach (Point p2 in nodes)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        Node n2 = _nodes[p2];
                        if (n2.HasNeighbour(p1))
                        {
                            continue;
                        }

                        var (path, cost) = Astar.FindPathWithCost(map, p1, p2);

                        if (cost < int.MaxValue)
                        {
                            AddEdge(n1, n2, path, cost);
                        }
                    }
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private ImmutableArray<ImmutableArray<Edge>> BuildEntrances(Map map, Cluster clusterA, Cluster clusterB)
        {
            // [a] <-> [b]
            if (clusterA.X < clusterB.X)
            {
                var allEntrances = ImmutableArray.CreateBuilder<ImmutableArray<Edge>>();
                var currentEntrance = ImmutableArray.CreateBuilder<Edge>();

                int xa = clusterA.Right;
                int xb = clusterB.Left;

                int initialY = clusterA.Y * CLUSTER_SIZE;

                for (int y = initialY; y < initialY + CLUSTER_SIZE; ++y)
                {
                    Edge e = new(new(xa, y), new(xb, y));
                    if (map.IsObstacleOrBlockVision(e.A) || map.IsObstacleOrBlockVision(e.B))
                    {
                        if (currentEntrance.Any())
                        {
                            allEntrances.Add(currentEntrance.ToImmutable());
                        }

                        currentEntrance.Clear();
                    }
                    else
                    {
                        currentEntrance.Add(e);
                    }
                }

                if (currentEntrance.Any())
                {
                    allEntrances.Add(currentEntrance.ToImmutable());
                }

                return allEntrances.ToImmutable();
            }
            // [b] <-> [a]
            else if (clusterA.X > clusterB.X)
            {
                return BuildEntrances(map, clusterB, clusterA);
            }

            // [a]
            //  ^
            //  |
            // [b]
            if (clusterA.Y < clusterB.Y)
            {
                var allEntrances = ImmutableArray.CreateBuilder<ImmutableArray<Edge>>();
                var currentEntrance = ImmutableArray.CreateBuilder<Edge>();

                int ya = clusterA.Bottom;
                int yb = clusterB.Top;

                int initialX = clusterA.X * CLUSTER_SIZE;

                for (int x = initialX; x < initialX + CLUSTER_SIZE; ++x)
                {
                    Edge e = new(new(x, ya), new(x, yb));
                    if (map.IsObstacleOrBlockVision(e.A) || map.IsObstacleOrBlockVision(e.B))
                    {
                        if (currentEntrance.Any())
                        {
                            allEntrances.Add(currentEntrance.ToImmutable());
                        }

                        currentEntrance.Clear();
                    }
                    else
                    {
                        currentEntrance.Add(e);
                    }
                }

                if (currentEntrance.Any())
                {
                    allEntrances.Add(currentEntrance.ToImmutable());
                }

                return allEntrances.ToImmutable();
            }
            // [b]
            //  |
            //  v
            // [a]
            else if (clusterA.Y > clusterB.Y)
            {
                return BuildEntrances(map, clusterB, clusterA);
            }

            throw new InvalidOperationException("Comparing entrance for the same cluster?");
        }
    }
}