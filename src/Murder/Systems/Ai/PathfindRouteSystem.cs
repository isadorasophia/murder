using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Systems
{
    [Filter(typeof(RouteComponent))]
    [Watch(typeof(RouteComponent))]
    public class PathfindRouteSystem : IFixedUpdateSystem, IReactiveSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                // We might have deleted the MoveTo component in MoveToSystem.
                MoveToComponent? moveToComponent = e.TryGetMoveTo();

                IMurderTransformComponent position = e.GetGlobalTransform();
                Vector2 currentTarget = moveToComponent?.Target ?? position.ToVector2();

                if (position.IsSameCell(currentTarget.ToPosition()))
                {
                    PathfindComponent pathfindComponent = e.GetPathfind();

                    Vector2 pathfindTarget = pathfindComponent.Target;
                    Point cell = position.CellPoint();

                    if (cell == pathfindTarget.ToGridPoint())
                    {
                        e.SetMoveTo(new MoveToComponent(pathfindTarget));

                        // We have reached our goal, snap to it!
                        e.RemovePathfind();
                        e.SetPathfindStatus(PathfindStatusFlags.PathComplete);
                        e.RemoveRoute();

                        continue;
                    }

                    // Look for our next target tile.
                    RouteComponent route = e.GetRoute();

                    if (!route.Nodes.ContainsKey(cell))
                    {
                        // for some reason, we went off-route. so just improvise and go somewhere else.
                        // First we look for the closest node to our current position.
                        Point closest = route.Nodes.Keys.OrderBy(x => (x - cell).LengthSquared()).First();
                        TargetEntityTo(e, route.Nodes[closest]);
                    }
                    else
                    {
                        TargetEntityTo(e, route.Nodes[cell]);
                    }

                }
            }
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                // Did we delete this because we already got to the target position?
                if (e.TryGetRoute() is RouteComponent route)
                {
                    TargetEntityTo(e, route.Nodes[route.Initial]);
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (e.TryGetRoute() is RouteComponent route)
                {
                    TargetEntityTo(e, route.Nodes[route.Initial]);
                }
            }
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

        private void TargetEntityTo(Entity e, Point nextCell)
        {
            e.SetMoveTo(
                new MoveToComponent(nextCell.FromCellToVector2CenterPosition()));
        }
    }
}