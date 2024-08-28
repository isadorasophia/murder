using Bang;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Core.Ai
{
    public static class PathfindServices
    {
        /// <summary>
        /// Find a path between <paramref name="initial"/> and <paramref name="target"/>.
        /// </summary>
        public static ComplexDictionary<Point, Point> FindPath(this Map? map, World world, Point initial, Point target, PathfindAlgorithmKind kind, int collisionMask, out PathfindStatusFlags statusFlags)
        {
            statusFlags = PathfindStatusFlags.None;

            // If it already sees the target, just go in a straight line!
            if (map == null || map.HasLineOfSight(initial, target, excludeEdges: false, blocking: collisionMask))
            {
                statusFlags = PathfindStatusFlags.HasLineOfSight;
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
                        return [];
                    }

                    return haastar.Search(map, initial, target);

                case PathfindAlgorithmKind.None:
                default:
                    return StraightLine(initial, target);
            }
        }

        private static ComplexDictionary<Point, Point> StraightLine(Point initial, Point target)
        {
            ComplexDictionary<Point, Point> builder = [];
            builder.Add(initial, target);

            return builder;
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
        internal static ReadOnlySpan<Point> NeighboursWithoutCollision(this Point point, Map map, bool strict)
        {
            int index = 0;
            Span<Point> result = new Point[8];

            foreach (Point neighbour in point.Neighbours(map.Width, map.Height, includeDiagonals: true))
            {
                if (strict)
                {
                    // Determine if the movement is diagonal
                    bool isDiagonal = Math.Abs(neighbour.X - point.X) == 1 && Math.Abs(neighbour.Y - point.Y) == 1;
                    if (isDiagonal)
                    {
                        // Check horizontal and vertical neighbors
                        Point horizontal = new Point(neighbour.X, point.Y);
                        Point vertical = new Point(point.X, neighbour.Y);

                        if (!map.IsObstacleOrBlockVision(horizontal) && !map.IsObstacleOrBlockVision(vertical) && !map.IsObstacleOrBlockVision(neighbour))
                        {
                            result[index++] = neighbour;
                        }
                    }
                    else
                    {
                        if (!map.IsObstacle(neighbour))
                        {
                            result[index++] = neighbour;
                        }
                    }
                }
                else
                {
                    if (!map.IsObstacle(neighbour))
                    {
                        result[index++] = neighbour;
                    }
                }
            }

            return result.Slice(0, index);
        }

    }
}