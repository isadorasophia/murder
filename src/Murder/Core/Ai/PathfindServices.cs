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
        public static ImmutableDictionary<Point, Point> FindPath(this Map? map, World world, Point initial, Point target, PathfindAlgorithmKind kind)
        {
            // If it already sees the target, just go in a straight line!
            if (map == null || map.HasLineOfSight(initial, target, excludeEdges: false, blocking: CollisionLayersBase.BLOCK_VISION | CollisionLayersBase.SOLID | CollisionLayersBase.HOLE))
            {
                kind = PathfindAlgorithmKind.None;
                return StraightLine(initial, target);
            }

            switch (kind)
            {
                case PathfindAlgorithmKind.Astar:
                    return Astar.FindPath(map, initial, target);

                case PathfindAlgorithmKind.HAAstar:
                    if (world.TryGetUnique<HAAStarPathfindComponent>()?.Data is not HAAStar haastar)
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
            if (world.TryGetUnique<MapComponent>()?.Map is not Map map)
            {
                return;
            }

            if (world.TryGetUnique<HAAStarPathfindComponent>()?.Data is HAAStar haastar)
            {
                haastar.Refresh(map);
            }
        }

        /// <summary>
        /// Returns all the neighbours of a position within a collision map.
        /// </summary>
        internal static ReadOnlySpan<Point> NeighboursWithoutCollision(this Point p, Map map)
        {
            int index = 0;
            Span<Point> result = new Point[8];

            foreach (Point n in p.Neighbours(map.Width, map.Height, includeDiagonals: true))
            {
                if (!map.IsObstacle(n))
                {
                    result[index++] = n;
                }
            }

            return result.Slice(0, index);
        }
    }
}