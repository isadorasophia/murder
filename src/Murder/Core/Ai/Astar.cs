using Murder.Core.Geometry;
using Murder.Serialization;

namespace Murder.Core.Ai
{
    internal static class Astar
    {
        public static ComplexDictionary<Point, Point> FindPath(Map map, Point initial, Point target, bool strict)
        {
            var (path, _) = FindPathWithCost(map, initial, target, strict);
            return path;
        }

        internal static (ComplexDictionary<Point, Point> Path, double Cost) FindPathWithCost(
            Map map, Point initial, Point target, bool strict)
        {
            var from = new Dictionary<Point, Point>();

            PriorityQueue<Point, double> open = new();

            // These are all the visited nodes and their respective costs.
            Dictionary<Point, double> costForVisitedNode = new();

            open.Enqueue(initial, 0);
            costForVisitedNode.Add(initial, 0);

            // TODO: We have a very weird "memory leak" when switching between multiple pathfind calculations
            // when loading a new save.
            // For now, we will workaround that by clamping the maximum amount of open nodes.
            while (open.Count > 0 && open.Count < map.Width * map.Height)
            {
                Point current = open.Dequeue();

                if (current == target)
                {
                    break;
                }
                
                foreach (Point p in current.NeighboursWithoutCollision(map, strict))
                {
                    double cost = costForVisitedNode[current] + map.WeightAt(p.X, p.Y);
                    // Increase the cost if it's diagonal
                    if (current.X != p.X && current.Y != p.Y)
                    {
                        cost += 0.414f;
                    }

                    // If we have not visited this node yet, or we just found a cheaper path.
                    if (!costForVisitedNode.ContainsKey(p) || cost < costForVisitedNode[p])
                    {
                        open.Enqueue(p, cost + Heuristic(p, target));
                        costForVisitedNode[p] = cost;

                        from[p] = current;
                    }
                }
            }

            ComplexDictionary<Point, Point> reversePath = [];

            if (!from.ContainsKey(target))
            {
                // Path is actually unreachable.
                return ([], int.MaxValue);
            }

            double totalCost = 0;

            Point next = target;
            while (next != initial)
            {
                Point previous = from[next];

                if (reversePath.ContainsKey(previous))
                {
                    // Cycle might have been detected? Abort immediately.
                    // This has been found while doing an A* while a Preprocess has been taking place.
                    return ([], int.MaxValue);
                }

                reversePath[previous] = next;
                next = previous;

                totalCost += costForVisitedNode[previous];
            }

            return (reversePath, totalCost);
        }

        internal static double Heuristic(Point initial, Point target)
        {
            // Single cost.
            const int D = 1;

            // Diagonal cost.
            const double D2 = 1.414;

            int dx = Math.Abs(initial.X - target.X);
            int dy = Math.Abs(initial.Y - target.Y);

            // Here, we compute the number of steps you take if you can't take a diagonal,
            // then subtract the steps you save by using the diagonal.
            // There are min(dx, dy) diagonal steps, and each one costs D2 BUT saves you
            // 2 * D non-diagonal steps.
            return D * (dx + dy) + (D2 - 2 * D) * Math.Min(dx, dy);
        }
    }
}