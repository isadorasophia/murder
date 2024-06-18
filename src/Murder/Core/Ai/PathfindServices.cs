using Bang;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Ai
{
    public static class PathfindServices
    {
        /// <summary>
        /// Find a path between <paramref name="initial"/> and <paramref name="target"/>.
        /// </summary>
        public static ImmutableDictionary<Point, Point> FindPath(this Map? map, World world, Point initial, Point target, PathfindAlgorithmKind kind, int collisionMask)
        {
            // If it already sees the target, just go in a straight line!
            if (map == null || map.HasLineOfSight(initial, target, excludeEdges: false, blocking: collisionMask))
            {
                return StraightLine(initial, target);
            }

            switch (kind)
            {
                case PathfindAlgorithmKind.AstarStrict:
                    return Astar.FindPath(map, initial, target, true);

                case PathfindAlgorithmKind.Astar:
                    return Astar.FindPath(map, initial, target, false);

                case PathfindAlgorithmKind.HAAstar:
                    if (world.TryGetUniqueHAAStarPathfind()?.Data is not HAAStar haastar)
                    {
                        GameLogger.Error("Unable to find component for HAAStar. Pathfind will abort!");
                        return ImmutableDictionary<Point, Point>.Empty;
                    }

                    return haastar.Search(map, initial, target);

                case PathfindAlgorithmKind.None:
                default:
                    return StraightLine(initial, target);
            }
        }

        private static ImmutableDictionary<Point, Point> StraightLine(Point initial, Point target)
        {
            var builder = ImmutableDictionary.CreateBuilder<Point, Point>();
            builder.Add(initial, target);

            return builder.ToImmutable();
        }

        /// <summary>
        /// Update all cached pathfind on a map change.
        /// </summary>
        /// <param name="world"></param>
        public static void UpdatePathfind(World world)
        {
            if (world.TryGetUniqueMap()?.Map is not Map map)
            {
                return;
            }

            if (world.TryGetUniqueHAAStarPathfind()?.Data is HAAStar haastar)
            {
                haastar.Refresh(map);
            }
        }
        
        /// <summary>
         /// Returns all the neighbours of a position within a collision map. Strict means that it will only allow diagonal movement if both the horizontal and vertical neighbours are open.
         /// </summary>
        internal static ReadOnlySpan<Point> NeighboursWithoutCollision(this Point p, Map map, bool strict)
        {
            int index = 0;
            Span<Point> result = new Point[8];

            foreach (Point n in p.Neighbours(map.Width, map.Height, includeDiagonals: true))
            {
                if (strict)
                {
                    // Determine if the movement is diagonal
                    bool isDiagonal = Math.Abs(n.X - p.X) == 1 && Math.Abs(n.Y - p.Y) == 1;
                    if (isDiagonal)
                    {
                        // Check horizontal and vertical neighbors
                        Point horizontal = new Point(n.X, p.Y);
                        Point vertical = new Point(p.X, n.Y);

                        if (!map.IsObstacle(horizontal) && !map.IsObstacleOrBlockVision(vertical) && !map.IsObstacleOrBlockVision(n))
                        {
                            result[index++] = n;
                        }
                    }
                    else
                    {
                        if (!map.IsObstacle(n))
                        {
                            result[index++] = n;
                        }
                    }
                }
                else
                {
                    if (!map.IsObstacle(n))
                    {
                        result[index++] = n;
                    }
                }
            }

            return result.Slice(0, index);
        }

    }
}