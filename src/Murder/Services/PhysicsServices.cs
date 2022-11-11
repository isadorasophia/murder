using Bang;
using Bang.Contexts;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Prefabs;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Services
{
    public static class PhysicsServices
    {
        public static IntRectangle GetCarveBoundingBox(this ColliderComponent collider, Point position)
        {
            IntRectangle rect = collider.GetBoundingBox(position);

            var floorToGridWithThreshold = (float input) =>
            {
                int floor = Grid.FloorToGrid(input);

                float remainingGrid = input - floor * Grid.CellSize;
                if (remainingGrid != 0 && remainingGrid > Grid.CellSize * .8f)
                {
                    return Grid.CeilToGrid(input);
                }

                return floor;
            };

            var ceilToGridWithThreshold = (float input) =>
            {
                int ceil = Grid.CeilToGrid(input);

                float remainingGrid = ceil * Grid.CellSize - input;
                if (remainingGrid != 0 && remainingGrid > Grid.CellSize * .8f)
                {
                    return Grid.FloorToGrid(input);
                }

                return ceil;
            };

            int top = floorToGridWithThreshold(rect.Top);
            int left = floorToGridWithThreshold(rect.Left);

            int right = ceilToGridWithThreshold(rect.Right);
            int bottom = ceilToGridWithThreshold(rect.Bottom);

            return new(left, top, Math.Max(right - left, 1), Math.Max(bottom - top, 1));
        }

        public static IntRectangle GetBoundingBox(this ColliderComponent collider, Point position)
        {
            int left = int.MaxValue;
            int right = int.MinValue;
            int top = int.MaxValue;
            int bottom = int.MinValue;
            foreach (var shape in collider.Shapes)
            {
                var rect = shape.GetBoundingBox();
                left = Math.Min(left, Calculator.FloorToInt(rect.Left));
                right = Math.Max(right, Calculator.FloorToInt(rect.Right));
                top = Math.Min(top, Calculator.CeilToInt(rect.Top));
                bottom = Math.Max(bottom, Calculator.CeilToInt(rect.Bottom));
            }

            return new(left + position.X, top + position.Y, right - left, bottom - top);
        }

        /// <summary>
        /// Get bounding box of an entity that contains both <see cref="ColliderComponent"/>
        /// and <see cref="PositionComponent"/>.
        /// </summary>
        public static IntRectangle GetColliderBoundingBox(this Entity target)
        {
            ColliderComponent collider = target.GetCollider();
            Vector2 position = target.GetGlobalTransform().Vector2;

            return collider.GetBoundingBox(position.Point);
        }

        public readonly struct RaycastHit
        {
            public readonly Point Tile;
            public readonly Entity? Entity;
            public readonly Vector2 Point;

            public RaycastHit()
            {
                Entity = null;
                Tile = default;
                Point = Vector2.Zero;
            }

            public RaycastHit(Point tile, Vector2 point)
            {
                Tile = tile;
                Entity = null;
                Point = point;
            }

            public RaycastHit(Entity? entity, Vector2 point) : this()
            {
                Tile = default;
                Entity = entity;
                Point = point;
            }

        }

        public static bool RaycastTiles(World world, Vector2 startPosition, Vector2 endPosition, GridCollisionType flags, out RaycastHit hit)
        {
            Map map = world.GetUnique<MapComponent>().Map;
            
            (int x0, int y0) = (startPosition / Grid.CellSize).Floor().XY;
            (int x1, int y1) = (endPosition / Grid.CellSize).Floor().XY;
            
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int x = x0;
            int y = y0;
            int n = 1 + dx + dy;
            int x_inc = (x1 > x0) ? 1 : -1;
            int y_inc = (y1 > y0) ? 1 : -1;
            int error = dx - dy;
            dx *= 2;
            dy *= 2;

            for (; n > 0; --n)
            {
                if (map.GetCollision(x, y).HasFlag(flags))
                {
                    var box = new Rectangle(x * Grid.CellSize, y * Grid.CellSize, Grid.CellSize, Grid.CellSize);
                    Line2 line = new(startPosition, endPosition);
                
                    if (line.TryGetIntersectingPoint(box, out var intersectTilePoint))
                    {
                        hit = new RaycastHit(new Point(x,y), intersectTilePoint);
                        return true;
                    }
                }

                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                }
                else
                {
                    y += y_inc;
                    error += dx;
                }
            }

            hit = default;
            return false;
        } 

        /// <summary>
        /// TODO: Implement
        /// </summary>
        public static bool Raycast(World world, Vector2 startPosition, Vector2 endPosition, bool onlySolids, IEnumerable<int> ignoreEntities, out RaycastHit hit)
        {
            hit = default;
            Map map = world.GetUnique<MapComponent>().Map;
            Line2 line = new(startPosition, endPosition);
            bool hitSomething = false;
            float closest = float.MaxValue;

            if (RaycastTiles(world,startPosition,endPosition, GridCollisionType.IsObstacle, out var hitTile))
            {
                line = new(startPosition, hitTile.Point);
                hit = hitTile;
                hitSomething = true;

                closest = (startPosition - hitTile.Point).LengthSquared();
            }
            
            var qt = world.GetUnique<QuadtreeComponent>().Quadtree;

            float minX = Math.Clamp(MathF.Min(startPosition.X, endPosition.X), 0, map.Width * Grid.CellSize);
            float maxX = Math.Clamp(MathF.Max(startPosition.X, endPosition.X), 0, map.Width * Grid.CellSize);
            float minY = Math.Clamp(MathF.Min(startPosition.Y, endPosition.Y), 0, map.Height * Grid.CellSize);
            float maxY = Math.Clamp(MathF.Max(startPosition.Y, endPosition.Y), 0, map.Height * Grid.CellSize);

            List<(Entity entity, Rectangle boundingBox)> possibleEntities = new();
            qt.GetEntitiesAt(new Rectangle(minX, minY, maxX - minX, maxY - minY), ref possibleEntities);
            
            
            foreach (var e in possibleEntities)
            {
                if (ignoreEntities.Contains(e.entity.EntityId))
                    continue;

                var position = e.entity.GetGlobalTransform();
                if (e.entity.TryGetCollider() is ColliderComponent collider)
                {
                    if (!collider.Solid && onlySolids)
                        continue;

                    
                    foreach (var shape in collider.Shapes)
                    {
                        var otherPosition = e.entity.GetGlobalTransform().Point;

                        switch (shape)
                        {
                            case CircleShape circle:
                                // TODO: Add missing position
                                if (line.IntersectsCircle(circle.Circle))
                                {
                                    if ((startPosition - otherPosition).LengthSquared() < closest)
                                    {
                                        closest = (startPosition - hitTile.Point).LengthSquared();
                                        hit = new RaycastHit(e.entity, otherPosition);
                                        hitSomething = true;
                                    }

                                    continue;
                                }
                                break;
                            case BoxShape rect:
                                if (line.IntersectsRect(rect.Rectangle + position.Point))
                                {
                                    if ((startPosition - otherPosition).LengthSquared() < closest)
                                    {
                                        closest = (startPosition - hitTile.Point).LengthSquared();
                                        hit = new RaycastHit(e.entity, otherPosition);
                                        hitSomething = true;
                                    }

                                    continue;
                                }
                                break;
                            case LazyShape lazy:
                                if (line.IntersectsRect(lazy.Rectangle(position.Point)))
                                {
                                    if ((startPosition - otherPosition).LengthSquared() < closest)
                                    {
                                        closest = (startPosition - hitTile.Point).LengthSquared();
                                        hit = new RaycastHit(e.entity, otherPosition);
                                        hitSomething = true;
                                    }

                                    continue;
                                }
                                break;
                            case PolygonShape polygon:
                                if (polygon.Polygon.Intersects(line))
                                {
                                    if ((startPosition - otherPosition).LengthSquared() < closest)
                                    {
                                        closest = (startPosition - hitTile.Point).LengthSquared();
                                        hit = new RaycastHit(e.entity, otherPosition);
                                        hitSomething = true;
                                    }

                                    continue;
                                }
                                break;
                        }
                }
                }
            }

            return hitSomething;
        }
        

        /// <summary>
        /// Find an eligible position to place an entity <paramref name="e"/> in the world that does not collide
        /// with other entities and targets <paramref name="target"/>.
        /// This will return immediate neighbours if <paramref name="target"/> is already occupied.
        /// </summary>
        public static Vector2? FindNextAvailablePosition(World world, Entity e, Vector2 target)
        {
            Map map = world.GetUnique<MapComponent>().Map;
            var collisionEntities = FilterPositionAndColliderEntities(world, solidOnly: true);

            return FindNextAvailablePosition(world, e, target, map, collisionEntities, new());
        }

        private static Vector2? FindNextAvailablePosition(
            World world,
            Entity e,
            Vector2 target,
            Map map,
            ImmutableArray<(int id, ColliderComponent colider, IMurderTransformComponent position)> collisionEntities,
            HashSet<Vector2> checkedPositions,
            bool onlyCheckForNeighbours = true)
        {
            if (checkedPositions.Contains(target))
            {
                return null;
            }

            if (!onlyCheckForNeighbours)
            {
                // Try our target position!
                if (!CollidesAt(map, ignoreId: e.EntityId, e.GetCollider(), target, collisionEntities))
                {
                    return target;
                }
            }
            else
            {
                // Let's add ourselves so we don't recurse over ourselves.
                checkedPositions.Add(target);

                // Okay, that didn't work. Let's fallback to our neighbours in that case.
                foreach (Vector2 neighbour in target.Neighbours(world))
                {
                    if (FindNextAvailablePosition(world, e, neighbour, map, collisionEntities, checkedPositions, onlyCheckForNeighbours: false) is Vector2 position)
                    {
                        return position;
                    }
                }

                // That also didn't work!! So let's try again, but this time, iterate over each of the neighbours.
                foreach (Vector2 neighbour in target.Neighbours(world))
                {
                    if (FindNextAvailablePosition(world, e, neighbour, map, collisionEntities, checkedPositions, onlyCheckForNeighbours: true) is Vector2 position)
                    {
                        return position;
                    }
                }
            }

            // Everything's crowded.
            return null;
        }

        /// <summary>
        /// Get all the neighbours of a position within the world.
        /// This does not check for collision (yet)!
        /// </summary>
        public static IEnumerable<Vector2> Neighbours(this Vector2 position, World world)
        {
            int width = 256;
            int height = 256;

            if (world.TryGetUnique<MapComponent>() is MapComponent map)
            {
                width = map.Width;
                height = map.Height;
            }

            return position.Neighbours(width * Grid.CellSize, height * Grid.CellSize);
        }

        public static ImmutableArray<(int id, ColliderComponent colider, IMurderTransformComponent position)> FilterPositionAndColliderEntities(IEnumerable<(Entity entity, Rectangle boundingBox)> entities, bool solidOnly)
        {
            var builder = ImmutableArray.CreateBuilder<(int id, ColliderComponent colider, IMurderTransformComponent position)>();
            foreach (var e in entities)
            {
                if (e.entity.IsDestroyed)
                    continue;
                
                var collider = e.entity.GetCollider();
                if (!solidOnly || (collider.Solid && !e.entity.HasNotSolid()))
                {
                    builder.Add(
                    (
                        e.entity.EntityId,
                        collider,
                        e.entity.GetGlobalTransform()
                    ));
                }
            }
            var collisionEntities = builder.ToImmutable();
            return collisionEntities;
        }


        public static ImmutableArray<(int id, ColliderComponent colider, IMurderTransformComponent position)> FilterPositionAndColliderEntities(IEnumerable<Entity> entities, bool solidOnly)
        {
            var builder = ImmutableArray.CreateBuilder<(int id, ColliderComponent colider, IMurderTransformComponent position)>();
            foreach (var e in entities)
            {
                var collider = e.GetCollider();
                if (!solidOnly || (collider.Solid && !e.HasNotSolid()))
                {
                    builder.Add(
                    (
                        e.EntityId,
                        collider,
                        e.GetGlobalTransform()
                    ));
                }
            }
            var collisionEntities = builder.ToImmutable();
            return collisionEntities;
        }

        public static ImmutableArray<(int id, ColliderComponent colider, IMurderTransformComponent position)> FilterPositionAndColliderEntities(World world, Func<Entity, bool> filter)
        {
            var builder = ImmutableArray.CreateBuilder<(int id, ColliderComponent colider, IMurderTransformComponent position)>();
            foreach (var e in world.GetEntitiesWith(ContextAccessorFilter.AllOf, typeof(ColliderComponent), typeof(PositionComponent)))
            {
                var colider = e.GetCollider();
                if (filter(e))
                {
                    builder.Add((
                        e.EntityId,
                        colider,
                        e.GetGlobalTransform()
                        ));
                }
            }
            var collisionEntities = builder.ToImmutable();
            return collisionEntities;
        }
        public static ImmutableArray<(int id, ColliderComponent colider, IMurderTransformComponent position)> FilterPositionAndColliderEntities(World world, bool solidOnly)
        {
            var builder = ImmutableArray.CreateBuilder<(int id, ColliderComponent colider, IMurderTransformComponent position)>();
            foreach (var e in world.GetEntitiesWith(ContextAccessorFilter.AllOf, typeof(ColliderComponent), typeof(PositionComponent)))
            {
                var colider = e.GetCollider();
                if (!solidOnly || (colider.Solid && !e.HasNotSolid()))
                {
                    builder.Add((
                        e.EntityId,
                        colider,
                        e.GetGlobalTransform()
                        ));
                }
            }
            var collisionEntities = builder.ToImmutable();
            return collisionEntities;
        }

        public static ImmutableArray<(int id, ColliderComponent colider, IMurderTransformComponent position)> FilterPositionAndColliderEntities(World world, bool solidOnly, params Type[] requireComponents)
        {
            var builder = ImmutableArray.CreateBuilder<(int id, ColliderComponent colider, IMurderTransformComponent position)>();
            Type[] filter = new Type[requireComponents.Length + 2];
            filter[0] = typeof(ColliderComponent);
            filter[1] = typeof(PositionComponent);
            for (int i = 0; i < requireComponents.Length; i++)
            {
                filter[i+2] = requireComponents[i];
            }

            foreach (var e in world.GetEntitiesWith(ContextAccessorFilter.AllOf, filter))
            {
                var colider = e.GetCollider();
                if (!solidOnly || colider.Solid)
                {
                    builder.Add((
                        e.EntityId,
                        colider,
                        e.GetGlobalTransform()
                        ));
                }
            }
            var collisionEntities = builder.ToImmutable();
            return collisionEntities;
        }

        public static IEnumerable<int> GetAllCollisionsAt(IMurderTransformComponent position, ColliderComponent collider, int ignoreId, IEnumerable<(int id, ColliderComponent colider, PositionComponent position)> others)
        {
            // Now, check against other entities.
            foreach (var other in others)
            {
                var otherCollider = other.colider;
                if (ignoreId == other.id) continue; // That's me!

                foreach (var shape in collider.Shapes)
                {
                    foreach (var otherShape in otherCollider.Shapes)
                    {
                        if (CollidesWith(shape, position.Point, otherShape, other.position.ToPoint()))
                        {
                            yield return other.id;
                        }
                    }
                }
            }
        }

        public static bool CollidesAt(in Map map, int ignoreId, ColliderComponent collider, Vector2 position, IEnumerable<(int id, ColliderComponent colider, IMurderTransformComponent position)> others)
        {
            return CollidesAt(map, ignoreId, collider, position, others, out _);
        }
        public static bool CollidesAt(in Map map, int ignoreId, ColliderComponent collider, Vector2 position, IEnumerable<(int id, ColliderComponent colider, IMurderTransformComponent position)> others, out int hitId)
        {
            hitId = -1;

            // First, check if there is a collision against a tile.
            if (PhysicsServices.CollidesAtTile(map, collider, position))
            {
                return true;
            }

            // Now, check against other entities.
            foreach (var other in others)
            {
                var otherCollider = other.colider;
                if (ignoreId == other.id) continue; // That's me!

                foreach (var shape in collider.Shapes)
                {
                    foreach (var otherShape in otherCollider.Shapes)
                    {
                        if (CollidesWith(shape, position.Point, otherShape, other.position.Point))
                        {
                            hitId = other.id;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool CollidesWith(Entity entityA, Entity entityB)
        {
            if (entityA.TryGetCollider() is ColliderComponent colliderA
                && entityA.TryGetTransform() is IMurderTransformComponent positionA
                && entityB.TryGetCollider() is ColliderComponent colliderB
                && entityB.TryGetTransform() is IMurderTransformComponent positionB)
            {
                foreach (var shapeA in colliderA.Shapes)
                {
                    foreach (var shapeB in colliderB.Shapes)
                    {
                        if (CollidesWith(shapeA, positionA.GetGlobal().Point, shapeB, positionB.GetGlobal().Point))
                            return true;
                    }
                }
            }

            return false;
        }
        public static bool CollidesWith(IShape shape1, Point position1, IShape shape2, Point position2)
        {

            { // Lazy vs. Box
                if ((shape1 is BoxShape && shape2 is LazyShape) || (shape2 is BoxShape && shape1 is LazyShape))
                {
                    // Code is very ugly, but it's this way for maximum performance
                    LazyShape lazy;
                    BoxShape box;

                    int boxLeft, boxRight, boxTop, boxBottom;
                    int lazyLeft, lazyRight, lazyTop, lazyBottom;

                    if (shape1 is BoxShape)
                    {
                        box = ((BoxShape)shape1);
                        lazy = ((LazyShape)shape2);

                        boxLeft = box.Offset.X + position1.X - Calculator.RoundToInt(box.Origin.X * box.Width);
                        boxRight = boxLeft + box.Width;
                        boxTop = box.Offset.Y + position1.Y - Calculator.RoundToInt(box.Origin.Y * box.Height);
                        boxBottom = boxTop + box.Height;

                        int size = Calculator.RoundToInt(LazyShape.SQUARE_ROOT_OF_TWO * lazy.Radius / 2f);
                        lazyLeft = lazy.Offset.X + position2.X - size + 1;
                        lazyRight = lazyLeft + size * 2 - 1;
                        lazyTop = lazy.Offset.Y + position2.Y - size;
                        lazyBottom = lazyTop + size * 2;
                    }
                    else
                    {
                        box = ((BoxShape)shape2);
                        lazy = ((LazyShape)shape1);

                        boxLeft = box.Offset.X + position2.X - Calculator.RoundToInt(box.Origin.X * box.Width);
                        boxRight = boxLeft + box.Width;
                        boxTop = box.Offset.Y + position2.Y - Calculator.RoundToInt(box.Origin.Y * box.Height);
                        boxBottom = boxTop + box.Height;

                        int size = Calculator.RoundToInt(LazyShape.SQUARE_ROOT_OF_TWO * lazy.Radius / 2f);
                        lazyLeft = lazy.Offset.X + position1.X - size + 1;
                        lazyRight = lazyLeft + size * 2 - 1;
                        lazyTop = lazy.Offset.Y + position1.Y - size;
                        lazyBottom = lazyTop + size * 2;
                    }

                    return  boxLeft <= lazyRight &&
                            lazyLeft <= boxRight &&
                            boxTop <= lazyBottom &&
                            lazyTop <= boxBottom;
                }
            }

            { // Lazy vs. Lazy
                if (shape1 is LazyShape lazy1 && shape2 is LazyShape lazy2)
                { 
                    return lazy1.Touches(lazy2, position1, position2);
                }
            }

            { // Lazy vs. Circle
                if ((shape1 is CircleShape && shape2 is LazyShape) || (shape2 is CircleShape && shape1 is LazyShape))
                {
                    Circle circle;
                    LazyShape lazy;

                    if (shape1 is CircleShape)
                    {
                        circle = ((CircleShape)shape1).Circle;
                        lazy = ((LazyShape)shape2);
                    }
                    else
                    {
                        circle = ((CircleShape)shape2).Circle;
                        lazy = ((LazyShape)shape1);
                    }

                    return lazy.Touches(circle, position1, position2);
                }
            }

            { // Lazy vs. Point
                if ((shape1 is PointShape && shape2 is LazyShape) || (shape2 is PointShape && shape1 is LazyShape))
                {
                    Point point;
                    LazyShape lazy;

                    if (shape1 is PointShape)
                    {
                        point = ((PointShape)shape1).Point + position1 - position2;
                        lazy = ((LazyShape)shape2);

                        return lazy.Touches(point);
                    }
                    else
                    {
                        point = ((PointShape)shape2).Point + position2 - position1;
                        lazy = ((LazyShape)shape1);

                        return lazy.Touches(point);
                    }

                }
            }

            { // Lazy vs. Line
                if ((shape1 is LineShape && shape2 is LazyShape) || (shape2 is LineShape && shape1 is LazyShape))
                {
                    Rectangle rect;
                    Line2 line;

                    if (shape1 is LineShape)
                    {
                        line = ((LineShape)shape1).LineAtPosition(position1);
                        rect = ((LazyShape)shape2).Rectangle(position2);
                    }
                    else
                    {
                        line = ((LineShape)shape1).LineAtPosition(position2);
                        rect = ((LazyShape)shape1).Rectangle(position1);
                    }

                    return line.IntersectsRect(rect);
                }
            }

            { // Lazy vs. Polygon
                if ((shape1 is PolygonShape && shape2 is LazyShape) || (shape2 is PolygonShape && shape1 is LazyShape))
                {
                    Rectangle circle;
                    Polygon polygon;

                    if (shape1 is PolygonShape)
                    {
                        polygon = ((PolygonShape)shape1).Polygon.AddPosition(position1);
                        circle = ((LazyShape)shape2).Rectangle(position2);
                    }
                    else
                    {
                        polygon = ((PolygonShape)shape2).Polygon.AddPosition(position2);
                        circle = ((LazyShape)shape1).Rectangle(position1);
                    }

                    return polygon.Intersect(circle);
                }
            }
            
            { // Point vs. Point
                if (shape1 is PointShape point1 && shape2 is PointShape point2)
                {
                    return point1.Point + position1 == point2.Point + position2;
                }
            }

            { // Point vs. Box
                if ((shape1 is BoxShape && shape2 is PointShape) || (shape2 is BoxShape && shape1 is PointShape))
                {
                    Point point;
                    Rectangle box;

                    if (shape1 is BoxShape)
                    {
                        box = ((BoxShape)shape1).Rectangle.AddPosition(position1);
                        point = ((PointShape)shape2).Point + position2;
                    }
                    else
                    {
                        box = ((BoxShape)shape2).Rectangle.AddPosition(position2);
                        point = ((PointShape)shape1).Point + position1;
                    }

                    return box.Contains(point);
                }
            }

            { // Point vs. Circle
                if ((shape1 is CircleShape && shape2 is PointShape) || (shape2 is CircleShape && shape1 is PointShape))
                {
                    Point point;
                    Circle circle;

                    if (shape1 is BoxShape)
                    {
                        circle = ((CircleShape)shape1).Circle.AddPosition(position1);
                        point = ((PointShape)shape2).Point + position2;
                    }
                    else
                    {
                        circle = ((CircleShape)shape2).Circle.AddPosition(position2);
                        point = ((PointShape)shape1).Point + position1;
                    }

                    return circle.Contains(point);
                }
            }

            { // Point vs. Line
                if ((shape1 is LineShape && shape2 is PointShape) || (shape2 is LineShape && shape1 is PointShape))
                {
                    Point point;
                    Line2 line;

                    if (shape1 is BoxShape)
                    {
                        line = ((LineShape)shape1).LineAtPosition(position1);
                        point = ((PointShape)shape2).Point + position2;
                    }
                    else
                    {
                        line = ((LineShape)shape2).LineAtPosition(position2);
                        point = ((PointShape)shape1).Point + position1;
                    }
                    return line.HasPoint(point);
                }
            }

            { // Line vs Circle
                if ((shape1 is LineShape && shape2 is CircleShape) || (shape2 is LineShape && shape1 is CircleShape))
                {
                    Circle circle;
                    Line2 line;

                    if (shape1 is LineShape)
                    {
                        line = ((LineShape)shape1).LineAtPosition(position1);
                        circle = ((CircleShape)shape2).Circle.AddPosition(position2);
                    }
                    else
                    {
                        line = ((LineShape)shape2).LineAtPosition(position2);
                        circle = ((CircleShape)shape1).Circle.AddPosition(position1);
                    }

                    return line.IntersectsCircle(circle);
                }
            }


            { // Line vs Line
                if ((shape1 is LineShape line1 && shape2 is LineShape line2))
                {
                    var lineA = line1.LineAtPosition(position1);
                    var lineB = line2.LineAtPosition(position2);
                    return lineA.Intersects(lineB);
                }
            }



            { // Line vs. Box
                if ((shape1 is BoxShape && shape2 is LineShape) || (shape2 is BoxShape && shape1 is LineShape))
                {
                    Line2 line;
                    Rectangle box;

                    if (shape1 is BoxShape)
                    {
                        box = ((BoxShape)shape1).Rectangle.AddPosition(position1);
                        line= ((LineShape)shape2).LineAtPosition(position2);
                    }
                    else
                    {
                        box = ((BoxShape)shape2).Rectangle.AddPosition(position2);
                        line = ((LineShape)shape1).LineAtPosition(position1);
                    }

                    //check to see if any lines on the box intersect the line
                    Line2 boxLine;

                    boxLine = new Line2(box.Left + 1, box.Top + 1, box.Right, box.Top);
                    if (boxLine.Intersects(line))
                        return true;

                    boxLine = new Line2(box.Right, box.Top + 1, box.Right, box.Bottom);
                    if (boxLine.Intersects(line))
                        return true;

                    boxLine = new Line2(box.Right, box.Bottom, box.Left + 1, box.Bottom);
                    if (boxLine.Intersects(line))
                        return true;

                    boxLine = new Line2(box.Left + 1, box.Bottom, box.Left + 1, box.Top + 1);
                    if (boxLine.Intersects(line))
                        return true;

                    return false;
                }
            }


            { // Box vs. Box
                if (shape1 is BoxShape box1 && shape2 is BoxShape box2)
                {
                    Rectangle rect1 = box1.Rectangle.AddPosition(position1);
                    Rectangle rect2 = box2.Rectangle.AddPosition(position2);

                    if (rect1.Touches(rect2))
                    {
                        return true;
                    }

                    return false;
                }
            }

            { // Box vs. Circle
                if ((shape1 is BoxShape && shape2 is CircleShape) || (shape2 is BoxShape && shape1 is CircleShape))
                {
                    Circle circle;
                    Rectangle box;

                    if (shape1 is BoxShape)
                    {
                        box = ((BoxShape)shape1).Rectangle.AddPosition(position1);
                        circle = ((CircleShape)shape2).Circle.AddPosition(position2);
                    }
                    else
                    {
                        box = ((BoxShape)shape2).Rectangle.AddPosition(position2);
                        circle = ((CircleShape)shape1).Circle.AddPosition(position1);
                    }

                    //check is c center point is in the rect
                    if (box.Contains(circle.X, circle.Y))
                    {
                        return true;
                    }

                    //check to see if any corners are in the circle
                    if (Calculator.DistanceRectPoint(circle.X, circle.Y+1, box.Left, box.Top+1, box.Width, box.Height) < circle.Radius)
                    {
                        return true;
                    }

                    //check to see if any lines on the box intersect the circle
                    Line2 boxLine;

                    boxLine = new Line2(box.Left, box.Top + 1, box.Right, box.Top);
                    if (boxLine.IntersectsCircle(circle))
                        return true;

                    boxLine = new Line2(box.Right, box.Top + 1, box.Right, box.Bottom);
                    if (boxLine.IntersectsCircle(circle))
                        return true;

                    boxLine = new Line2(box.Right, box.Bottom, box.Left, box.Bottom);
                    if (boxLine.IntersectsCircle(circle))
                        return true;

                    boxLine = new Line2(box.Left, box.Bottom, box.Left, box.Top + 1);
                    if (boxLine.IntersectsCircle(circle))
                        return true;

                    return false;
                }
            }

            { // Circle vs. Circle
                if ((shape1 is CircleShape circle1 && shape2 is CircleShape circle2))
                {
                    var center1 = position1 + circle1.Offset;
                    var center2 = position2 + circle2.Offset;

                    return (center1 - center2).LengthSquared() <= MathF.Pow(circle1.Radius + circle2.Radius, 2);
                }
            }

            { // Polygon vs. Point
                if ((shape1 is PolygonShape && shape2 is PointShape) || (shape2 is PolygonShape && shape1 is PointShape))
                {
                    Point point;
                    Polygon polygon;

                    if (shape1 is PolygonShape)
                    {
                        polygon = ((PolygonShape)shape1).Polygon.AddPosition(position1);
                        point = ((PointShape)shape2).Point + position2;
                    }
                    else
                    {
                        polygon = ((PolygonShape)shape2).Polygon.AddPosition(position2);
                        point = ((PointShape)shape1).Point + position1;
                    }
                    return polygon.HasVector2(point);
                }
            }

            { // Polygon vs. Circle
                if ((shape1 is PolygonShape && shape2 is CircleShape) || (shape2 is PolygonShape && shape1 is CircleShape))
                {
                    Circle circle;
                    Polygon polygon;

                    if (shape1 is PolygonShape)
                    {
                        polygon = ((PolygonShape)shape1).Polygon.AddPosition(position1);
                        circle = ((CircleShape)shape2).Circle.AddPosition(position2);
                    }
                    else
                    {
                        polygon = ((PolygonShape)shape2).Polygon.AddPosition(position2);
                        circle = ((CircleShape)shape1).Circle.AddPosition(position1);
                    }

                    return polygon.Intersect(circle);

                }
            }


            { // Polygon vs. Line
                if ((shape1 is PolygonShape && shape2 is LineShape) || (shape2 is PolygonShape && shape1 is LineShape))
                {
                    Line2 line;
                    Polygon polygon;

                    if (shape1 is PolygonShape)
                    {
                        polygon = ((PolygonShape)shape1).Polygon.AddPosition(position1);
                        line = ((LineShape)shape2).LineAtPosition(position2);
                    }
                    else
                    {
                        polygon = ((PolygonShape)shape2).Polygon.AddPosition(position1);
                        line = ((LineShape)shape1).LineAtPosition(position1);
                    }

                    return polygon.Intersects(line);
                }
            }

            { // Polygon vs. Rectangle
                if ((shape1 is PolygonShape && shape2 is BoxShape) || (shape2 is PolygonShape && shape1 is BoxShape))
                {
                    Rectangle rectangle;
                    Polygon polygon;

                    if (shape1 is PolygonShape)
                    {
                        polygon = ((PolygonShape)shape1).Polygon.AddPosition(position1);
                        rectangle = ((BoxShape)shape2).Rectangle.AddPosition(position2);
                    }
                    else
                    {
                        polygon = ((PolygonShape)shape2).Polygon.AddPosition(position2);
                        rectangle = ((BoxShape)shape1).Rectangle.AddPosition(position1);
                    }

                    return polygon.Intersect(rectangle);
                }
            }

            { // Polygon vs. Polygon
                if (shape1 is PolygonShape poly1 && shape2 is PolygonShape poly2)
                { 
                    return poly1.Polygon.AddPosition(position1)
                        .Intersect(poly2.Polygon.AddPosition(position2));
                }
            }

            GameLogger.Fail($"Invalid collision check {shape1.GetType()} & {shape2.GetType()}");

            return false;
        }

        public static bool ContainsPoint(Entity entity, Point point)
        {
            if (entity.TryGetComponent<ColliderComponent>() is ColliderComponent collider)
            {
                Point position = Point.Zero;
                if (entity.TryGetComponent<PositionComponent>() is PositionComponent positionComponent)
                {
                    position = positionComponent.GetGlobal().Point;
                }

                if (!collider.Shapes.IsDefaultOrEmpty)
                {
                    foreach (var shape in collider.Shapes)
                    {
                        switch (shape)
                        {
                            case PointShape pointShape:
                                return pointShape.Point == point;

                            case LineShape lineShape:
                                return lineShape.Line.HasPoint(point);

                            case BoxShape box:
                                var rect = new Rectangle(position + box.Offset, new Point(box.Width, box.Height));
                                return rect.Contains(point);

                            case CircleShape circle:
                                var delta = position + circle.Offset - point;
                                return delta.LengthSquared() <= MathF.Pow(circle.Radius, 2);

                            case PolygonShape polygon:
                                return polygon.Polygon.HasVector2(point);

                            default:
                                return false;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Apply collision with tile objects within the map.
        /// </summary>
        public static bool CollidesAtTile(in Map map, ColliderComponent collider, Vector2 position)
        {
            foreach (var shape in collider.Shapes)
            {
                if (CollidesAtTile(map, shape, position))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Apply collision with tile objects within the map.
        /// </summary>
        private static bool CollidesAtTile(in Map map, IShape shape, Vector2 position)
        {
            switch (shape)
            {
                case LazyShape lazy:
                    {
                        var rect = lazy.Rectangle(position);

                        var topLeft = new Point(
                            Calculator.FloorToInt((float)(rect.X) / Grid.CellSize),
                            Calculator.FloorToInt((float)(rect.Y - 1) / Grid.CellSize));
                         
                        var botRight = new Point(
                            Calculator.CeilToInt((float)(rect.X + rect.Width) / Grid.CellSize),
                            Calculator.CeilToInt((float)(rect.Y + rect.Height + 1) / Grid.CellSize));

                        if (map?.HasStaticCollisionAt(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y) is Point point)
                            return true;

                        return false;
                    }

                case PointShape point:
                    {
                        Point gridPos = (point.Point.ToVector2() + position).ToGridPoint();
                        return map.HasStaticCollisionAt(gridPos.X, gridPos.Y, 1, 1) is Point;
                    }

                case LineShape lineShape:
                    {
                        Line2 line = lineShape.LineAtPosition(position.Point);
                        //make a rectangle out of the line segment, check for any tiles in that rectangle

                        //if there are tiles in there, loop through and check each one as a rectangle against the line
                        if (map.HasStaticCollisionAt(Grid.RoundToGrid(line.Left)-1, Grid.RoundToGrid(line.Top)-1, Grid.CeilToGrid(line.Width)+1, Grid.CeilToGrid(line.Height)+1) is Point)
                        {
                            int rectX, rectY;
                            int
                                gridx = Grid.FloorToGrid(line.Left),
                                gridy = Grid.FloorToGrid(line.Top),
                                gridx2 = Grid.CeilToGrid(line.Right),
                                gridy2 = Grid.CeilToGrid(line.Bottom);

                            for (int i = gridx; i <= gridx2; i++)
                            {
                                for (int j = gridy; j <= gridy2; j++)
                                {
                                    if (map.HasStaticCollision(i, j))
                                    {
                                        rectX = i * Grid.CellSize;
                                        rectY = j * Grid.CellSize;
                                        var rect = new Rectangle(rectX, rectY, Grid.CellSize, Grid.CellSize);
                                        
                                        if (rect.Contains(line.PointA.X, line.PointA.Y))
                                        {
                                            return true;
                                        }
                                        
                                        if (rect.Contains(line.PointB.X, line.PointB.Y))
                                        {
                                            return true;
                                        }
                                        
                                        if (line.IntersectsRect(rectX, rectY, Grid.CellSize, Grid.CellSize))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }

                        return false;
                    }

                case BoxShape box:
                    {
                        var rect = box.Rectangle;

                        var topLeft = new Point(
                            Calculator.FloorToInt((position.X + rect.X) / Grid.CellSize),
                            Calculator.FloorToInt((position.Y + rect.Y) / Grid.CellSize));

                        var botRight = new Point(
                            Calculator.CeilToInt((position.X + rect.X + box.Width) / Grid.CellSize),
                            Calculator.CeilToInt((position.Y + rect.Y + box.Height) / Grid.CellSize));

                        if (map.HasStaticCollisionAt(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y) is Point point)
                            return true;

                        return false;
                    }

                case CircleShape circle:
                    {
                        // TODO: Make this actually take in consideration the circular shape?
                        var topLeft = new Point(
                            Calculator.FloorToInt((position.X + circle.Offset.X - circle.Radius) / Grid.CellSize),
                            Calculator.FloorToInt((position.Y + circle.Offset.Y - circle.Radius) / Grid.CellSize));

                        var botRight = new Point(
                            Calculator.CeilToInt((position.X + circle.Offset.X + circle.Radius) / Grid.CellSize),
                            Calculator.CeilToInt((position.Y + circle.Offset.X + circle.Radius) / Grid.CellSize));

                        if (map.HasStaticCollisionAt(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y) is Point point)
                            return true;

                        return false;
                    }

                case PolygonShape polygon:
                    {
                        var rect = new IntRectangle(Grid.FloorToGrid(polygon.Rect.X), Grid.FloorToGrid(polygon.Rect.Y),
                            Grid.CeilToGrid(polygon.Rect.Width)+2, Grid.CeilToGrid(polygon.Rect.Height)+2)
                            .AddPosition(position.ToGridPoint());
                        foreach(var tile in map.GetStaticCollisions(rect))
                        {
                            var tileRect = new Rectangle(tile.X * Grid.CellSize, tile.Y * Grid.CellSize, Grid.CellSize, Grid.CellSize).AddPosition(-position);
                            if (polygon.Polygon.Intersect(tileRect))
                                return true;
                        }

                        return false;
                    }

                default:
                    return false;
            }
        }
    }
}
