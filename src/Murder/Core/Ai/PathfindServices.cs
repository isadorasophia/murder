using System.Collections.Immutable;
using Bang;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Utilities;

namespace Murder.Core.Ai
{
    public static class PathfindServices
    {
        /// <summary>
        /// Find a path between <paramref name="initial"/> and <paramref name="target"/>.
        /// </summary>
        public static ImmutableDictionary<Point, Point> FindPath(this Map map, World world, Point initial, Point target, PathfindAlgorithmKind kind)
        {
            // If it already sees the target, just go in a straight line!
            if (map.HasLineOfSight(initial, target, excludeEdges: false, blocking: GridCollisionType.IsObstacle))
            {
                kind = PathfindAlgorithmKind.None;
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
                    var builder = ImmutableDictionary.CreateBuilder<Point, Point>();
                    builder.Add(initial, target);

                    return builder.ToImmutable();
            }
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
        internal static IEnumerable<Point> NeighboursWithoutCollision(this Point p, Map map)
        {
            foreach (Point n in p.Neighbours(map.Width, map.Height, includeDiagonals: true))
            {
                if (!map.IsObstacle(n))
                {
                    yield return n;
                }
            }
        }
    }
}
