using Murder.Core.Geometry;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Core.Ai
{
    public partial class HAAStar
    {
        public ComplexDictionary<Point, Point> Search(Map map, Point initial, Point target)
        {
            if (_pendingCalculate != null && !_pendingCalculate.IsCompleted)
            {
                // Use astar while the calculation is pending!
                return Astar.FindPath(map, initial, target, false);
            }

            (Node nInitial, bool discardInitial) = InsertNode(map, initial);
            (Node nTarget, bool discardTarget) = InsertNode(map, target);

            ComplexDictionary<Point, Point> path = SearchForPath(nInitial, nTarget);

            if (discardInitial)
            {
                RemoveNode(nInitial);
            }

            if (discardTarget)
            {
                RemoveNode(nTarget);
            }

            return path;
        }

        private (Node node, bool discardNodeAfter) InsertNode(Map map, Point p)
        {
            bool discardNodeAfter = true;

            if (_nodes.TryGetValue(p, out Node? n))
            {
                // Reuse the same node we already had.
                // But make sure we don't discard after this operation!
                discardNodeAfter = false;
            }
            else
            {
                n = AddNode(p, map.WeightAt(p));
                ConnectToBorder(map, n);
            }

            return (n, discardNodeAfter);
        }

        private void ConnectToBorder(Map map, Node o)
        {
            foreach (Point p in _nodesPerCluster[o.Cluster])
            {
                if (p == o.P)
                {
                    // Skip the same node in this cluster.
                    continue;
                }

                var (path, cost) = Astar.FindPathWithCost(map, o.P, p, false);

                if (cost < int.MaxValue)
                {
                    AddEdge(o, _nodes[p], path, cost);
                }
            }
        }

        private ComplexDictionary<Point, Point> SearchForPath(Node initial, Node target)
        {
            var from = new Dictionary<Node, Node>();

            PriorityQueue<Node, double> open = new();

            // These are all the visited nodes and their respective costs.
            Dictionary<Point, double> costForVisitedNode = new();

            open.Enqueue(initial, 0);
            costForVisitedNode.Add(initial.P, 0);

            while (open.Count > 0 && open.Count < _nodes.Count)
            {
                Node current = open.Dequeue();

                if (current == target)
                {
                    break;
                }

                foreach ((Point p, double costToNode) in current.Neighbours)
                {
                    double cost = costForVisitedNode[current.P] + costToNode;

                    // If we have not visited this node yet, or we just found a cheaper path.
                    if (!costForVisitedNode.ContainsKey(p) || cost < costForVisitedNode[p])
                    {
                        Node n = _nodes[p];

                        open.Enqueue(n, cost + Astar.Heuristic(p, target.P));
                        costForVisitedNode[p] = cost;

                        from[n] = current;
                    }
                }
            }

            ComplexDictionary<Point, Point> reversePath = [];

            if (!from.ContainsKey(target))
            {
                // Path is actually unreachable.
                return reversePath;
            }

            Node next = target;
            while (next != initial)
            {
                Node previous = from[next];

                if (reversePath.ContainsKey(previous.P))
                {
                    // Cycle might have been detected? Abort immediately.
                    return [];
                }

                foreach ((Point innerFrom, Point innerTo) in previous.PathTo(next.P))
                {
                    // This path has not been refined, so we might find cycles here!
                    // Make sure we are setting the dictionary and overwriting in such cases.
                    reversePath[innerFrom] = innerTo;
                }

                next = previous;
            }

            return reversePath;
        }
    }
}