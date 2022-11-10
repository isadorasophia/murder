using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using System.Collections.Immutable;
using Murder.Components;
using Murder.Core;
using Murder.Diagnostics;
using Murder.Core.Geometry;
using Murder.Core.Ai;
using Road.Messages;
using Murder.Utilities;

namespace Murder.Systems
{
    [Watch(typeof(PathfindComponent))]
    public class CalculatePathfindSystem : IStartupSystem, IReactiveSystem
    {
        public ValueTask Start(Context context)
        {
            if (context.World.TryGetUnique<MapDimensionsComponent>() is not MapDimensionsComponent d || 
                context.World.TryGetUnique<MapComponent>()?.Map is not Map map)
            {
                GameLogger.Error("Unable to find map dimensions in this world?");
                return default;
            }

            Entity e = context.World.AddEntity(new HAAStarPathfindComponent(d.Width, d.Height));
            e.GetHAAStarPathfind().Data.Refresh(map);

            return default;
        }

        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUnique<MapComponent>().Map;
            foreach (var e in entities)
            {
                CalculatePath(world, map, e);
            }

            return default;
        }

        public ValueTask OnModified(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUnique<MapComponent>().Map;
            foreach (var e in entities)
            {
                if (!e.HasPathfind())
                {
                    continue;
                }

                if (e.TryGetComponent(out RouteComponent route))
                {
                    Point target = e.GetPathfind().Target.ToGridPoint();
                    if (route.Target == target)
                    {
                        continue;
                    }
                }

                CalculatePath(world, map, e);
            }

            return default;
        }

        public ValueTask OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.RemoveRoute();
                entity.SetFriction(0.25f);
            }

            return default;
        }

        private static void CalculatePath(World world, Map map, Entity e)
        {
            PathfindComponent pathfind = e.GetPathfind();
            IMurderTransformComponent position = e.GetGlobalTransform();

            Point initialCell = new(position.Cx, position.Cy);
            Point targetCell = pathfind.Target.ToGridPoint();

            var path = map.FindPath(
                world,
                initial: initialCell,
                target: targetCell,
                pathfind.Algorithm);

            if (path.IsEmpty)
            {
                e.SendMessage(new PathNotPossibleMessage());
                e.RemovePathfind();

                return;
            }

            e.SetRoute(new RouteComponent(path, initialCell, targetCell));
        }
    }
}