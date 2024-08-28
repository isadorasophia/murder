using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Ai;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Messages;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Watch(typeof(PathfindComponent))]
    public class CalculatePathfindSystem : IStartupSystem, IReactiveSystem
    {
        public void Start(Context context)
        {
            if (context.World.TryGetUniqueMap()?.Map is not Map map)
            {
                GameLogger.Error("Unable to find map dimensions in this world?");
                return;
            }

            Entity e = context.World.AddEntity(new HAAStarPathfindComponent(map.Width, map.Height));
            e.GetHAAStarPathfind().Data.Refresh(map);
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUniqueMap().Map;
            foreach (var e in entities)
            {
                if (!e.HasPathfind())
                    continue; // [HACK] This entity shouldn't be added to the context here.

                CalculatePath(world, map, e);
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUniqueMap().Map;
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
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            foreach (var entity in entities)
            {
                entity.RemoveRoute();
                entity.SetFriction(0.25f);
            }
        }

        public const int LineOfSightCollisionMask = CollisionLayersBase.BLOCK_VISION | CollisionLayersBase.SOLID | CollisionLayersBase.HOLE | CollisionLayersBase.CARVE;

        private static void CalculatePath(World world, Map map, Entity e)
        {
            PathfindComponent pathfind = e.GetPathfind();
            IMurderTransformComponent position = e.GetGlobalTransform();
            

            Point initialCell = new(position.Cx, position.Cy);
            Point targetCell = pathfind.Target.ToGridPoint();

            int collisionMask;
            if (e.TryGetCustomCollisionMask() is CustomCollisionMask customCollisionMaskComponent)
            {
                collisionMask = customCollisionMaskComponent.CollisionMask;
            }
            else
            {
                collisionMask = LineOfSightCollisionMask;
            }
            // Carve and block vision are always added, no matter what.
            collisionMask |= CollisionLayersBase.CARVE | CollisionLayersBase.BLOCK_VISION;

            PathfindStatusFlags statusFlags = PathfindStatusFlags.None;

            var path = map.FindPath(
                world,
                initial: initialCell,
                target: targetCell,
                pathfind.Algorithm,
                collisionMask, out statusFlags);

            if (path.Count == 0)
            {
                e.SendMessage(new PathNotPossibleMessage());
                e.SetPathfindStatus(statusFlags | PathfindStatusFlags.PathNotFound);
                e.RemovePathfind();

                return;
            }

            e.SetPathfindStatus(statusFlags);
            e.SetRoute(new RouteComponent(path, initialCell, targetCell));
        }
    }
}