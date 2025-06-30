using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Murder.Services;

public static class PhysicsServices
{
    public readonly struct PhysicEntityCachedInfo
    {
        public readonly int EntityId;
        public readonly Vector2 Scale;
        public readonly Point GlobalPosition;
        public readonly ColliderComponent Collider;

        public PhysicEntityCachedInfo(int entityId, Vector2 scale, Point globalPosition, ColliderComponent collider)
        {
            EntityId = entityId;
            Scale = scale;
            GlobalPosition = globalPosition;
            Collider = collider;
        }
    }
    private static readonly ImmutableArray<PhysicEntityCachedInfo>.Builder _physicsInfoCacheBuilder = ImmutableArray.CreateBuilder<PhysicEntityCachedInfo>();

    public static Map? TryGetMap(World world)
    {
        return world.TryGetUniqueMap()?.Map;
    }

    public static IntRectangle GetCarveBoundingBox(this ColliderComponent collider, Vector2 position)
    {
        Rectangle rect = collider.GetBoundingBox(position);
        return rect.GetCarveBoundingBox(occupiedThreshold: .75f);
    }

    public static IntRectangle[] GetCollidersBoundingBox(this ColliderComponent collider, Point position, bool gridCoordinates)
    {
        IntRectangle[] result = new IntRectangle[collider.Shapes.Length];
        for (int i = 0; i < collider.Shapes.Length; ++i)
        {
            IShape shape = collider.Shapes[i];

            if (gridCoordinates)
            {
                var rect = shape.GetBoundingBox();
                int left = Grid.FloorToGrid(rect.X + position.X);
                int top = Grid.FloorToGrid(rect.Y + position.Y);
                int right = Grid.CeilToGrid(rect.X + rect.Width + position.X);
                int bottom = Grid.CeilToGrid(rect.Y + rect.Height + position.Y);

                result[i] = new IntRectangle(left, top, right - left, bottom - top);
            }
            else
            {
                result[i] = shape.GetBoundingBox().AddPosition(position);
            }
        }

        return result;
    }

    public static Rectangle GetBoundingBox(this ColliderComponent collider, Vector2 position)
    {
        if (collider.Shapes.Length == 0)
        {
            return Rectangle.Empty;
        }

        float left = int.MaxValue;
        float right = int.MinValue;
        float top = int.MaxValue;
        float bottom = int.MinValue;
        foreach (var shape in collider.Shapes)
        {
            var rect = shape.GetBoundingBox();
            left = Math.Min(left, rect.Left);
            top = Math.Min(top, rect.Top);
            right = Math.Max(right, rect.Right);
            bottom = Math.Max(bottom, rect.Bottom);
        }

        return new(left + position.X, top + position.Y, right - left, bottom - top);
    }

    /// <summary>
    /// Get bounding box of an entity that contains both <see cref="ColliderComponent"/>
    /// and <see cref="PositionComponent"/>.
    /// </summary>
    public static IntRectangle GetColliderBoundingBox(this Entity target)
    {
        if (!target.HasCollider() || !target.HasTransform())
        {
            return IntRectangle.Empty;
        }

        ColliderComponent collider = target.GetCollider();
        Vector2 position = target.GetGlobalTransform().Vector2;

        return collider.GetBoundingBox(position.Point());
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

    /// <summary>
    /// Tryies to raycast looking for tile collisions.
    /// </summary>
    public static bool RaycastTiles(
        World world,
        Vector2 startPos, Vector2 endPos,
        int flags, out RaycastHit hit)
    {
        Map map = world.GetUniqueMap().Map;

        // Convert to *grid* space once
        Vector2 startGrid = startPos / Grid.CellSize;
        Vector2 endGrid = endPos / Grid.CellSize;

        int x = Calculator.FloorToInt(startGrid.X);
        int y = Calculator.FloorToInt(startGrid.Y);
        int endX = Calculator.FloorToInt(endGrid.X);
        int endY = Calculator.FloorToInt(endGrid.Y);

        int stepX = Math.Sign(endX - x); // −1, 0, or +1
        int stepY = Math.Sign(endY - y);

        // How far we must move along the ray to cross the first X / Y cell boundary
        float tMaxX = (stepX != 0)
            ? ((stepX > 0 ? (x + 1) - startGrid.X : startGrid.X - x) / MathF.Abs(endGrid.X - startGrid.X))
            : float.PositiveInfinity;

        float tMaxY = (stepY != 0)
            ? ((stepY > 0 ? (y + 1) - startGrid.Y : startGrid.Y - y) / MathF.Abs(endGrid.Y - startGrid.Y))
            : float.PositiveInfinity;

        // Distance to cross one whole cell in X / Y
        float tDeltaX = (stepX != 0) ? 1f / MathF.Abs(endGrid.X - startGrid.X) : float.PositiveInfinity;
        float tDeltaY = (stepY != 0) ? 1f / MathF.Abs(endGrid.Y - startGrid.Y) : float.PositiveInfinity;

        // Traverse
        while (true)
        {
            if (!map.IsInsideGrid(x, y)) break;

            int tile = map.At(x, y);

            // Uncomment this to debug the raycast tiles
            //DebugServices.DrawText(world, tile.ToString(), (new Vector2(x, y) + Vector2.One * 0.45f) * Grid.CellSize , 0.2f, Color.Red);

            if (tile.HasFlag(flags))
            {
                Vector2 hitPoint = startPos + (endPos - startPos) *
                                   MathF.Min(tMaxX, tMaxY);
                hit = new RaycastHit(new Point(x, y), hitPoint);
                return true;
            }

            // step to next cell
            if (tMaxX < tMaxY)
            {
                x += stepX;
                tMaxX += tDeltaX;
            }
            else
            {
                y += stepY;
                tMaxY += tDeltaY;
            }

            if (x == endX && y == endY) break;
        }

        hit = default;
        return false;
    }


    public static bool HasLineOfSight(World world, Vector2 from, Vector2 to, int mask)
    {
        if (Raycast(world, from, to, mask, [], out RaycastHit hit))
        {
            return false;
        }

        return true;
    }

    public static bool HasLineOfSight(World world, Vector2 from, Vector2 to)
    {
        return HasLineOfSight(world, from, to, CollisionLayersBase.BLOCK_VISION);
    }

    /// <summary>
    /// Returns whether <paramref name="from"/> an see an entity <paramref name="targetEntityId"/> before
    /// it gets to <paramref name="to"/>.
    /// </summary>
    public static bool CanSeeEntity(World world, Vector2 from, Vector2 to, int selfEntityId, int targetEntityId)
    {
        if (!Raycast(world, from, to, CollisionLayersBase.ACTOR, [selfEntityId], out RaycastHit hit))
        {
            return false;
        }

        if (hit.Entity?.EntityId == targetEntityId)
        {
            return true;
        }

        return false;
    }

    public static bool HasLineOfSight(World world, Entity from, Entity to)
    {
        if (from.TryGetMurderTransform()?.Vector2 is not Vector2 origin)
        {
            return false;
        }

        if (to.TryGetMurderTransform()?.Vector2 is not Vector2 target)
        {
            return false;
        }

        if (Raycast(world, origin, target, CollisionLayersBase.BLOCK_VISION,
            Enumerable.Empty<int>(), out RaycastHit hit))
        {
            if (hit.Entity?.EntityId == to.EntityId)
            {
                return true;
            }
        }
        else
        {
            // No obstacles! This means it has line of sight.
            return true;
        }

        return false;
    }
    public static bool Raycast(World world, Vector2 startPosition, Vector2 endPosition, float minHitDistance, int layerMask, IEnumerable<int> ignoreEntities, out RaycastHit hit)
    {
        Vector2 newStartPosition = startPosition + (endPosition - startPosition).Normalized() * minHitDistance;
        return Raycast(world, newStartPosition, endPosition, layerMask, ignoreEntities, out hit);
    }
    public static bool Raycast(World world, Vector2 startPosition, Vector2 endPosition, int layerMask, IEnumerable<int> ignoreEntities, out RaycastHit hit)
    {
        hit = default;
        Map map = world.GetUniqueMap().Map;
        Line2 line = new(startPosition, endPosition);
        bool hitSomething = false;
        float closest = float.MaxValue;

        List<int> ignoreEntitiesWithChildren = new List<int>();
        List<NodeInfo<Entity>> possibleEntities = new();

        foreach (var id in ignoreEntities)
        {
            if (world.TryGetEntity(id) is Entity entity)
            {
                ignoreEntitiesWithChildren.Add(id);
                ignoreEntitiesWithChildren.AddRange(EntityServices.GetAllChildren(world, entity));
            }
        }


        if (RaycastTiles(world, startPosition, endPosition, layerMask, out var hitTile))
        {
            line = new(startPosition, hitTile.Point);
            hit = hitTile;
            hitSomething = true;

            closest = (startPosition - hitTile.Point).LengthSquared();
        }

        var qt = world.GetUniqueQuadtree().Quadtree;

        float minX = Math.Clamp(MathF.Min(startPosition.X, endPosition.X), 0, map.Width * Grid.CellSize);
        float maxX = Math.Clamp(MathF.Max(startPosition.X, endPosition.X), 0, map.Width * Grid.CellSize);
        float minY = Math.Clamp(MathF.Min(startPosition.Y, endPosition.Y), 0, map.Height * Grid.CellSize);
        float maxY = Math.Clamp(MathF.Max(startPosition.Y, endPosition.Y), 0, map.Height * Grid.CellSize);

        possibleEntities.Clear();
        qt.GetCollisionEntitiesAt(new Rectangle(minX, minY, maxX - minX, maxY - minY), possibleEntities);

        foreach (var e in possibleEntities)
        {
            if (ignoreEntitiesWithChildren.Contains(e.EntityInfo.EntityId))
                continue;

            if (e.EntityInfo.IsDestroyed)
                continue;
            var position = e.EntityInfo.GetGlobalTransform();
            if (e.EntityInfo.TryGetCollider() is ColliderComponent collider)
            {
                if ((collider.Layer & CollisionLayersBase.RAYIGNORE) != 0)
                    continue;

                // Check if the entity is on the same layer as the raycast
                if ((collider.Layer & layerMask) == 0)
                    continue;

                foreach (var shape in collider.Shapes)
                {
                    switch (shape)
                    {
                        // TODO: Get intersecting point with circle
                        case CircleShape circle:
                            {
                                if (line.TryGetIntersectingPoint(circle.Circle.AddPosition(position.Point), out Vector2 hitPoint))
                                {
                                    CompareShapeHits(startPosition, ref hit, ref hitSomething, ref closest, e, hitPoint.Point());

                                    continue;
                                }
                            }
                            break;
                        case BoxShape rect:
                            {
                                if (line.TryGetIntersectingPoint(rect.Rectangle + position.Point, out Vector2 hitPoint))
                                {
                                    CompareShapeHits(startPosition, ref hit, ref hitSomething, ref closest, e, hitPoint.Point());

                                    continue;
                                }
                            }
                            break;
                        case LazyShape lazy:
                            {
                                var otherRect = lazy.Rectangle(position.Point);
                                if (line.TryGetIntersectingPoint(otherRect, out Vector2 hitPoint))
                                {
                                    CompareShapeHits(startPosition, ref hit, ref hitSomething, ref closest, e, hitPoint.Point());

                                    continue;
                                }
                            }
                            break;
                        case PolygonShape polygon:
                            {
                                if (polygon.Polygon.Intersects(line.AddPosition(-position.Point), out Vector2 hitPoint))
                                {
                                    CompareShapeHits(startPosition, ref hit, ref hitSomething, ref closest, e, hitPoint.Point());

                                    continue;
                                }
                            }
                            break;
                    }
                }
            }
        }

        return hitSomething;
    }

    private static void CompareShapeHits(Vector2 startPosition, ref RaycastHit hit, ref bool hitSomething, ref float closest, NodeInfo<Entity> e, Point hitPosition)
    {
        var hitDistanceSq = (startPosition - hitPosition).LengthSquared();
        if (hitDistanceSq < closest)
        {
            closest = hitDistanceSq;
            hit = new RaycastHit(e.EntityInfo, hitPosition);
            hitSomething = true;
        }
    }

    [Flags]
    public enum NextAvailablePositionFlags
    {
        CheckLineOfSight = 0b01,
        CheckNeighbours = 0b10,
        /// <summary>
        /// Only used if <see cref="CheckNeighbours"/> is set.
        /// </summary>
        CheckRecursiveNeighbours = 0b100,
        CheckTarget = 0b1000
    }

    private readonly static HashSet<Vector2> _checkedPositionsCache = new();
    /// <summary>
    /// Find an eligible position to place an entity <paramref name="e"/> in the world that does not collide
    /// with other entities and targets <paramref name="target"/>.
    /// This will return immediate neighbours if <paramref name="target"/> is already occupied.
    /// </summary>
    public static Vector2? FindNextAvailablePosition(
        World world,
        Entity e,
        Vector2 center,
        Vector2 target,
        int layerMask,
        NextAvailablePositionFlags flags = NextAvailablePositionFlags.CheckTarget | NextAvailablePositionFlags.CheckNeighbours | NextAvailablePositionFlags.CheckRecursiveNeighbours)
    {
        Map map = world.GetUniqueMap().Map;
        var collisionEntities = FilterPositionAndColliderEntities(
            world,
            layerMask);
        _checkedPositionsCache.Clear();

        return FindNextAvailablePosition(world, e, center, target, map, collisionEntities, _checkedPositionsCache, flags, layerMask);
    }

    public static bool CollidesAt(
        World world,
        Entity e,
        Vector2 position,
        int layerMask)
    {
        Map map = world.GetUniqueMap().Map;
        var collisionEntities = FilterPositionAndColliderEntities(
            world,
            layerMask);

        if (e.TryGetCollider() is ColliderComponent collider)
        {
            Vector2 scale = e.FetchScale();
            return CollidesAt(map, e.EntityId, collider, position, scale, collisionEntities, layerMask, out int _);
        }
        else
        {
            GameLogger.Warning("Creating a collider on the fly, is this really what you want?");
            // Check using a single point
            return CollidesAt(map, e.EntityId, new ColliderComponent(new PointShape(), CollisionLayersBase.NONE, Color.Gray), position, Vector2.One, collisionEntities, layerMask, out int _);
        }
    }



    /// <summary>
    /// Position to check for collision used by <c>FindNextAvailablePosition</c>.
    /// </summary>
    private readonly static Queue<Vector2> _positionsToCheck = new Queue<Vector2>();

    private static Vector2? FindNextAvailablePosition(
        World world,
        Entity e,
        Vector2 center,
        Vector2 target,
        Map map,
        ImmutableArray<PhysicEntityCachedInfo> collisionEntities,
        HashSet<Vector2> checkedPositions,
        NextAvailablePositionFlags flags,
        int layerMask)
    {
        _positionsToCheck.Clear();
        _positionsToCheck.Enqueue(target);

        checkedPositions.Add(target);

        int depth = 0;

        bool checkNeighbours = flags.HasFlag(NextAvailablePositionFlags.CheckNeighbours);

        ColliderComponent collider = e.GetCollider();
        Vector2 scale = e.FetchScale();

        while (_positionsToCheck.Count > 0)
        {
            if (depth++ > 5)
            {
                // Let's not freeze our framerate by adding a limit here.
                GameLogger.Warning("FindNextAvailablePosition: Depth limit reached, returning null.");
                return null;
            }

            Vector2 currentPosition = _positionsToCheck.Dequeue();
            Point startGridPoint = center.ToGrid();

            bool CheckPosition(Vector2 position)
            {
                if (!CollidesAt(map, ignoreId: e.EntityId, collider, position, scale, collisionEntities, layerMask, out int _))
                {
                    if (!flags.HasFlag(NextAvailablePositionFlags.CheckLineOfSight) ||
                        map.HasLineOfSight(startGridPoint, position.ToGrid(), excludeEdges: true, layerMask))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (CheckPosition(currentPosition))
            {
                return currentPosition;
            }

            if (checkNeighbours)
            {
                foreach (Vector2 neighbour in currentPosition.Neighbours(world, 0.5f))
                {
                    if (!checkedPositions.Contains(neighbour))
                    {
                        _positionsToCheck.Enqueue(neighbour);
                        checkedPositions.Add(neighbour); // Mark this neighbour as checked to avoid re-checking
                    }
                }
            }
        }

        // No available position was found
        return null;
    }

    /// <summary>
    /// Get all the neighbours of a position within the world.
    /// This does not check for collision (yet)!
    /// </summary>
    public static IEnumerable<Vector2> Neighbours(this Vector2 position, World world, float unit)
    {
        int width = 256;
        int height = 256;

        if (world.TryGetUniqueMap() is MapComponent map)
        {
            width = map.Width;
            height = map.Height;
        }

        return position.Neighbours(width * Grid.CellSize, height * Grid.CellSize, unit);
    }

    public static ImmutableArray<PhysicEntityCachedInfo> FilterPositionAndColliderEntities(List<NodeInfo<Entity>> entities, HashSet<int> ignore, int layerMask)
    {
        _physicsInfoCacheBuilder.Clear();
        for (int i = 0; i < entities.Count; i++)
        {
            var e = entities[i];

            if (ignore.Contains(e.EntityInfo.EntityId))
                continue;

            if (e.EntityInfo.IsDestroyed || !e.EntityInfo.HasCollider())
                continue;

            var collider = e.EntityInfo.GetCollider();
            if ((collider.Layer & layerMask) == 0)
                continue;

            _physicsInfoCacheBuilder.Add(
                new PhysicEntityCachedInfo(
                    e.EntityInfo.EntityId,
                    e.EntityInfo.FetchScale(),
                    e.EntityInfo.GetGlobalTransform().Point,
                    collider
                ));
        }
        var collisionEntities = _physicsInfoCacheBuilder.ToImmutable();
        return collisionEntities;
    }


    public static ImmutableArray<PhysicEntityCachedInfo> FilterPositionAndColliderEntities(IEnumerable<Entity> entities, int layerMask)
    {
        _physicsInfoCacheBuilder.Clear();
        foreach (var e in entities)
        {
            var collider = e.GetCollider();
            if ((collider.Layer & layerMask) == 0)
                continue;

            _physicsInfoCacheBuilder.Add(
                new PhysicEntityCachedInfo(
                    e.EntityId,
                    e.FetchScale(),
                    e.GetGlobalTransform().Point,
                    collider
                ));
        }
        var collisionEntities = _physicsInfoCacheBuilder.ToImmutable();
        return collisionEntities;
    }

    public static ImmutableArray<PhysicEntityCachedInfo> FilterPositionAndColliderEntities(World world, Func<Entity, bool> filter)
    {
        _physicsInfoCacheBuilder.Clear();
        foreach (var e in world.GetEntitiesWith(ContextAccessorFilter.AllOf, typeof(ColliderComponent), typeof(ITransformComponent)))
        {
            var collider = e.GetCollider();
            if (filter(e))
            {
                _physicsInfoCacheBuilder.Add(new PhysicEntityCachedInfo(
                    e.EntityId,
                    e.FetchScale(),
                    e.GetGlobalTransform().Point,
                    collider
                    ));
            }
        }
        var collisionEntities = _physicsInfoCacheBuilder.ToImmutable();
        return collisionEntities;
    }

    public static ImmutableArray<PhysicEntityCachedInfo> FilterPositionAndColliderEntities(World world, int layerMask)
    {
        _physicsInfoCacheBuilder.Clear();
        foreach (var e in world.GetEntitiesWith(ContextAccessorFilter.AllOf, typeof(ColliderComponent), typeof(ITransformComponent)))
        {
            var collider = e.GetCollider();
            if ((collider.Layer & layerMask) == 0)
                continue;

            _physicsInfoCacheBuilder.Add(
                new PhysicEntityCachedInfo
                (
                e.EntityId,
                e.TryGetScale()?.Scale ?? Vector2.One,
                e.GetGlobalTransform().Point,
                collider
                ));
        }
        var collisionEntities = _physicsInfoCacheBuilder.ToImmutable();
        return collisionEntities;
    }

    public static ImmutableArray<Entity> FilterEntities(World world, int layerMask)
    {
        var builder = ImmutableArray.CreateBuilder<Entity>();
        foreach (var e in world.GetEntitiesWith(ContextAccessorFilter.AllOf, typeof(ColliderComponent), typeof(ITransformComponent)))
        {
            var collider = e.GetCollider();
            if ((collider.Layer & layerMask) == 0)
                continue;

            builder.Add(e);
        }
        var collisionEntities = builder.ToImmutable();
        return collisionEntities;
    }

    public static ImmutableArray<(int id, ColliderComponent collider, IMurderTransformComponent position)> FilterPositionAndColliderEntities(World world, int layerMask, params Type[] requireComponents)
    {
        var builder = ImmutableArray.CreateBuilder<(int id, ColliderComponent collider, IMurderTransformComponent position)>();
        Type[] filter = new Type[requireComponents.Length + 2];
        filter[0] = typeof(ColliderComponent);
        filter[1] = typeof(ITransformComponent);
        for (int i = 0; i < requireComponents.Length; i++)
        {
            filter[i + 2] = requireComponents[i];
        }

        foreach (var e in world.GetEntitiesWith(ContextAccessorFilter.AllOf, filter))
        {
            var collider = e.GetCollider();
            if (collider.Layer != layerMask)
            {
                builder.Add((
                    e.EntityId,
                    collider,
                    e.GetGlobalTransform()
                    ));
            }
        }
        var collisionEntities = builder.ToImmutable();
        return collisionEntities;
    }

    private static readonly List<NodeInfo<Entity>> _cachedCollisions = new();

    public static IEnumerable<Entity> GetAllCollisionsAt(World world, Point position, Vector2 scale, ColliderComponent collider, int ignoreId, int mask)
    {
        _cachedCollisions.Clear();

        // Now, check against other entities.
        Quadtree qt = world.GetUniqueQuadtree().Quadtree;
        qt.GetCollisionEntitiesAt(GetBoundingBox(collider, position), _cachedCollisions);

        foreach (var other in _cachedCollisions)
        {
            if (other.EntityInfo.IsDestroyed || other.EntityInfo.TryGetCollider() is not ColliderComponent otherCollider)
                continue;

            if (ignoreId == other.EntityInfo.EntityId)
                continue; // That's me!

            if (!otherCollider.Layer.HasFlag(mask))
                continue;

            var otherPosition = other.EntityInfo.GetGlobalTransform().Point;
            Vector2 otherScale = other.EntityInfo.FetchScale();

            foreach (var shape in collider.Shapes)
            {
                foreach (var otherShape in otherCollider.Shapes)
                {
                    if (CollidesWith(shape, position, scale, otherShape, otherPosition, otherScale))
                    {
                        yield return other.EntityInfo;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks for collision at a position, returns the minimum translation vector (MTV) to resolve the collision.
    /// </summary>
    public static bool GetFirstMtvAt(
        in Map map,
        ColliderComponent collider,
        Vector2 fromPosition,
        Vector2 toPosition,
        ImmutableArray<PhysicEntityCachedInfo> others,
        int mask,
        out int hitId,
        out int layer,
        out Vector2 mtv)
    {
        hitId = -1;
        layer = CollisionLayersBase.NONE;
        mtv = Vector2.Zero;

        Vector2 totalMtv = Vector2.Zero;
        bool hasCollision = false;
        float mtvLength = float.MaxValue;

        // Check for tile collision
        if (PhysicsServices.GetAccumulatedMtvAtTile(map, collider, fromPosition, toPosition, mask, out int tileLayer) is Vector2 tileMtv && tileMtv != Vector2.Zero)
        {
            layer = tileLayer;
            totalMtv += tileMtv;
            hasCollision = true;
            mtvLength = tileMtv.Length();
        }

        // Check against other entities
        foreach (var shape in collider.Shapes)
        {
            var polyA = shape.GetPolygon();

            foreach (var other in others)
            {
                foreach (var otherShape in other.Collider.Shapes)
                {
                    var polyB = otherShape.GetPolygon();
                    if (polyA.Polygon.Intersects(polyB.Polygon, toPosition, other.GlobalPosition) is Vector2 colliderMtv && colliderMtv.HasValue())
                    {
                        // The hit ID is the ID of the closest entity
                        float currentMtvLengthSquared = colliderMtv.LengthSquared();
                        if (currentMtvLengthSquared < mtvLength)
                        {
                            hitId = other.EntityId;
                            layer = other.Collider.Layer;
                            mtvLength = currentMtvLengthSquared;
                        }

                        float similarity = Calculator.Vector2Similarity(totalMtv, colliderMtv);

                        totalMtv += colliderMtv * (1 - similarity);
                        hasCollision = true;
                    }
                }
            }
        }

        if (hasCollision)
        {
            mtv = totalMtv;
        }

        return hasCollision;
    }



    /// <summary>
    /// Checks for collision at a position, returns the minimum translation vector (MTV) to resolve the collision.
    /// </summary>
    public static List<Vector2> GetMtvAt(in Map map, int ignoreId, ColliderComponent collider, Vector2 position, IEnumerable<PhysicEntityCachedInfo> others, int mask, out int hitId)
    {
        hitId = -1;
        List<Vector2> mtvs =
        [
            // First, check if there is a collision against a tile.
            .. PhysicsServices.GetMtvAtTile(map, collider, position, mask),
        ];

        // Now, check against other entities.

        foreach (var shape in collider.Shapes)
        {
            var polyA = shape.GetPolygon();

            foreach (var other in others)
            {
                var otherCollider = other.Collider;
                if (ignoreId == other.EntityId) continue; // That's me!

                foreach (var otherShape in otherCollider.Shapes)
                {
                    var polyB = otherShape.GetPolygon();
                    // TODO: This doesn't take into account the scale of the other entity.
                    if (polyA.Polygon.Intersects(polyB.Polygon, position.Point(), other.GlobalPosition) is Vector2 mtv && mtv.HasValue())
                    {
                        hitId = other.EntityId;
                        mtvs.Add(mtv);
                    }
                }
            }
        }

        return mtvs;
    }



    /// <summary>
    /// Checks for collision at a position, returns the minimum translation vector (MTV) to resolve the collision.
    /// </summary>
    public static Vector2 GetFirstMtv(int entityId, ColliderComponent collider, Vector2 position, IEnumerable<PhysicEntityCachedInfo> others, out int hitId)
    {
        hitId = -1;

        // Now, check against other entities.
        foreach (var shape in collider.Shapes)
        {
            var polyA = shape.GetPolygon();

            foreach (var other in others)
            {
                var otherCollider = other.Collider;
                if (entityId == other.EntityId) continue; // That's me!

                foreach (var otherShape in otherCollider.Shapes)
                {
                    var polyB = otherShape.GetPolygon();
                    if (polyA.Polygon.Intersects(polyB.Polygon, position.Point(), other.GlobalPosition) is Vector2 mtv && mtv.HasValue())
                    {
                        hitId = other.EntityId;
                        return mtv;
                    }
                }
            }
        }

        return Vector2.Zero;
    }

    private static Vector2 GetAccumulatedMtvAtTile(Map map, ColliderComponent collider, Vector2 fromPosition, Vector2 toPosition, int mask, out int layer)
    {
        Vector2 accumulatedMtv = Vector2.Zero;
        layer = CollisionLayersBase.NONE;
        float distance = (toPosition - fromPosition).Length();
        int collisionCount = 0;

        for (int i = 0; i < collider.Shapes.Length; i++)
        {
            var shape = collider.Shapes[i];
            var polygon = shape.GetPolygon();
            var boundingBox = new IntRectangle(Grid.FloorToGrid(polygon.Rect.X), Grid.FloorToGrid(polygon.Rect.Y),
                                Grid.CeilToGrid(polygon.Rect.Width) + 2, Grid.CeilToGrid(polygon.Rect.Height) + 2)
                                .AddPosition(toPosition.ToGridPoint());

            foreach (var tileCollisionInfo in map.GetCollisionInfosWith(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height, mask))
            {
                IntRectangle adjustedRect = (tileCollisionInfo.Rectangle * Grid.CellSize);
                adjustedRect.Height += 1;
                var tilePolygon = Polygon.FromRectangle(adjustedRect);

                if (polygon.Polygon.Intersects(tilePolygon, toPosition, Vector2.Zero) is Vector2 mtv && mtv.HasValue())
                {
                    collisionCount++;
                    accumulatedMtv += mtv;
                    layer |= tileCollisionInfo.Layer;
                }
            }
        }

        return (accumulatedMtv / Math.Max(1, collisionCount)).ClampLength(distance);
    }

    private static Vector2 GetFirstMtvAtTile(Map map, ColliderComponent collider, Vector2 position, int mask, out int layer)
    {
        Vector2 weightedMtv = Vector2.Zero;
        float totalWeight = 0f;
        layer = CollisionLayersBase.NONE;

        foreach (var shape in collider.Shapes)
        {
            var polygon = shape.GetPolygon();
            var boundingBox = new IntRectangle(Grid.FloorToGrid(polygon.Rect.X), Grid.FloorToGrid(polygon.Rect.Y),
                                Grid.CeilToGrid(polygon.Rect.Width) + 2, Grid.CeilToGrid(polygon.Rect.Height) + 2)
                                .AddPosition(position.ToGridPoint());

            foreach (var tileCollisionInfo in map.GetCollisionInfosWith(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height, mask))
            {
                var tilePolygon = Polygon.FromRectangle(tileCollisionInfo.Rectangle * Grid.CellSize);
                if (polygon.Polygon.Intersects(tilePolygon, position, Vector2.Zero) is Vector2 mtv && mtv.HasValue())
                {
                    float weight = mtv.Length();
                    weightedMtv += mtv * weight;
                    totalWeight += weight;
                    layer = tileCollisionInfo.Layer;
                }
            }
        }

        if (totalWeight > 0)
        {
            // Return the weighted average of the MTVs
            return weightedMtv / totalWeight;
        }

        return Vector2.Zero;
    }


    private static List<Vector2> GetMtvAtTile(Map map, ColliderComponent collider, Vector2 position, int mask)
    {
        List<Vector2> mtvs = new();
        foreach (var shape in collider.Shapes)
        {
            var polygon = shape.GetPolygon();
            var boundingBox = new IntRectangle(Grid.FloorToGrid(polygon.Rect.X), Grid.FloorToGrid(polygon.Rect.Y),
                            Grid.CeilToGrid(polygon.Rect.Width) + 2, Grid.CeilToGrid(polygon.Rect.Height) + 2)
                            .AddPosition(position.ToGridPoint());
            foreach (var tile in map.GetCollisionsWith(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height, mask))
            {
                var tilePolygon = Polygon.FromRectangle(tile.X * Grid.CellSize, tile.Y * Grid.CellSize, Grid.CellSize, Grid.CellSize);
                if (polygon.Polygon.Intersects(tilePolygon, position, Vector2.Zero) is Vector2 mtv && mtv.HasValue())
                {
                    mtvs.Add(mtv);
                }
            }
        }

        return mtvs;
    }

    public static bool CollidesAt(in Map map, int ignoreId, ColliderComponent collider, Vector2 position, Vector2 scale, ImmutableArray<PhysicEntityCachedInfo> others, int mask, out int hitId)
    {
        hitId = -1;

        // First, check if there is a collision against a tile.
        if (PhysicsServices.CollidesAtTile(map, collider, position, mask))
        {
            return true;
        }

        // Now, check against other entities.
        foreach (var other in others)
        {
            var otherCollider = other.Collider;
            if (ignoreId == other.EntityId) continue; // That's me!

            foreach (var shape in collider.Shapes)
            {
                foreach (var otherShape in otherCollider.Shapes)
                {
                    if (CollidesWith(shape, position.Point(), scale, otherShape, other.GlobalPosition, other.Scale))
                    {
                        hitId = other.EntityId;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool CollidesWith(Entity entityA, Entity entityB, Vector2 positionA)
    {
        if (entityA.TryGetCollider() is ColliderComponent colliderA
            && entityB.TryGetCollider() is ColliderComponent colliderB
            && entityB.TryGetTransform() is IMurderTransformComponent positionB)
        {
            Point posA = positionA.Point();
            Vector2 scaleA = entityA.FetchScale();
            Vector2 scaleB = entityB.FetchScale();
            foreach (var shapeA in colliderA.Shapes)
            {
                foreach (var shapeB in colliderB.Shapes)
                {
                    if (CollidesWith(shapeA, posA, scaleA, shapeB, positionB.GetGlobal().Point, scaleB))
                        return true;
                }
            }
        }

        return false;
    }
    public static bool CollidesWith(Entity entityA, Entity entityB)
    {
        if (entityA.TryGetCollider() is ColliderComponent colliderA
            && entityA.GetGlobalPositionIfValid() is Vector2 positionA
            && entityB.TryGetCollider() is ColliderComponent colliderB
            && entityB.GetGlobalPositionIfValid() is Vector2 positionB)
        {
            Vector2 scaleA = entityA.TryGetScale()?.Scale ?? Vector2.One;
            Vector2 scaleB = entityB.TryGetScale()?.Scale ?? Vector2.One;

            foreach (var shapeA in colliderA.Shapes)
            {
                foreach (var shapeB in colliderB.Shapes)
                {
                    if (CollidesWith(shapeA, positionA.Point(), scaleA, shapeB, positionB.Point(), scaleB))
                        return true;
                }
            }
        }

        return false;
    }
    public static bool CollidesWith(IShape shape1, Point position1, Vector2 scale1, IShape shape2, Point position2, Vector2 scale2)
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

                return boxLeft <= lazyRight &&
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
                Vector2 polygonScale;
                if (shape1 is PolygonShape)
                {
                    polygon = ((PolygonShape)shape1).Polygon;
                    polygonScale = scale1;

                    circle = ((LazyShape)shape2).Rectangle(position2 - position1);
                }
                else
                {
                    polygon = ((PolygonShape)shape2).Polygon;
                    polygonScale = scale2;

                    circle = ((LazyShape)shape1).Rectangle(position1 - position2);
                }

                return polygon.Intersect(circle, polygonScale);
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

                if (shape1 is CircleShape)
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
                    line = ((LineShape)shape2).LineAtPosition(position2);
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
                if (GeometryServices.DistanceRectPoint(circle.X, circle.Y + 1, box.Left, box.Top + 1, box.Width, box.Height) < circle.Radius)
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
                Vector2 polygonScale;

                if (shape1 is PolygonShape)
                {
                    polygon = ((PolygonShape)shape1).Polygon;
                    polygonScale = scale1;
                    point = ((PointShape)shape2).Point + position2 - position1;
                }
                else
                {
                    polygon = ((PolygonShape)shape2).Polygon;
                    polygonScale = scale2;
                    point = ((PointShape)shape1).Point + position1 - position2;
                }
                return polygon.Contains(point, polygonScale);
            }
        }

        { // Polygon vs. Circle
            if ((shape1 is PolygonShape && shape2 is CircleShape) || (shape2 is PolygonShape && shape1 is CircleShape))
            {
                Circle circle;
                Polygon polygon;
                Vector2 polygonScale;

                if (shape1 is PolygonShape)
                {
                    polygon = ((PolygonShape)shape1).Polygon;
                    polygonScale = scale1;
                    circle = ((CircleShape)shape2).Circle.AddPosition(position2 - position1);
                }
                else
                {
                    polygon = ((PolygonShape)shape2).Polygon;
                    polygonScale = scale2;
                    circle = ((CircleShape)shape1).Circle.AddPosition(position1 - position2);
                }

                return polygon.Intersect(circle, polygonScale);

            }
        }


        { // Polygon vs. Line
            if ((shape1 is PolygonShape && shape2 is LineShape) || (shape2 is PolygonShape && shape1 is LineShape))
            {
                Line2 line;
                Polygon polygon;
                Vector2 polygonScale;

                if (shape1 is PolygonShape)
                {
                    polygon = ((PolygonShape)shape1).Polygon;
                    polygonScale = scale1;
                    line = ((LineShape)shape2).LineAtPosition(position2 - position1);
                }
                else
                {
                    polygon = ((PolygonShape)shape2).Polygon;
                    polygonScale = scale2;
                    line = ((LineShape)shape1).LineAtPosition(position1 - position2);
                }

                return polygon.Intersects(line, polygonScale, out _);
            }
        }

        { // Polygon vs. Rectangle
            if ((shape1 is PolygonShape && shape2 is BoxShape) || (shape2 is PolygonShape && shape1 is BoxShape))
            {
                Rectangle rectangle;
                Polygon polygon;
                Vector2 polygonScale;

                if (shape1 is PolygonShape)
                {
                    polygon = ((PolygonShape)shape1).Polygon;
                    polygonScale = scale1;
                    rectangle = ((BoxShape)shape2).Rectangle.AddPosition(position2 - position1);
                }
                else
                {
                    polygon = ((PolygonShape)shape2).Polygon;
                    polygonScale = scale2;
                    rectangle = ((BoxShape)shape1).Rectangle.AddPosition(position1 - position2);
                }

                return polygon.Intersect(rectangle, polygonScale);
            }
        }

        { // Polygon vs. Polygon
            if (shape1 is PolygonShape poly1 && shape2 is PolygonShape poly2)
            {
                return poly1.Polygon.CheckOverlapAt(poly2.Polygon, position1 - position2);
            }
        }

        GameLogger.Fail($"Invalid collision check {shape1.GetType()} & {shape2.GetType()}");

        return false;
    }

    public static bool ContainsPoint(Entity entity, Point point)
    {
        if (entity.TryGetComponent<ColliderComponent>() is not ColliderComponent collider)
        {
            return false;
        }

        Point position = Point.Zero;
        if (entity.TryGetComponent<PositionComponent>() is PositionComponent positionComponent)
        {
            position = positionComponent.GetGlobal().Point;
        }

        if (collider.Shapes.IsDefaultOrEmpty)
        {
            return false;
        }

        bool contains = false;
        foreach (var shape in collider.Shapes)
        {
            switch (shape)
            {
                case LazyShape lazyShape:
                    Point delta = position + lazyShape.Offset - point;
                    contains = delta.LengthSquared() <= MathF.Pow(lazyShape.Radius, 2);
                    break;

                case PointShape pointShape:
                    contains = pointShape.Point == point;
                    break;

                case LineShape lineShape:
                    contains = lineShape.Line.HasPoint(point);
                    break;

                case BoxShape box:
                    var rect = new Rectangle(position + box.Offset, new Point(box.Width, box.Height));
                    contains = rect.Contains(point);
                    break;

                case CircleShape circle:
                    Point circleDelta = position + circle.Offset - point;
                    contains = circleDelta.LengthSquared() <= MathF.Pow(circle.Radius, 2);
                    break;

                case PolygonShape polygon:
                    contains = polygon.Polygon.Contains(point);
                    break;

                default:
                    return false;
            }

            if (contains)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Apply collision with tile objects within the map.
    /// </summary>
    public static bool CollidesAtTile(in Map map, ColliderComponent collider, Vector2 position, int mask)
    {
        foreach (var shape in collider.Shapes)
        {
            if (CollidesAtTile(map, shape, position, mask))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Apply collision with tile objects within the map.
    /// </summary>
    private static bool CollidesAtTile(in Map map, IShape shape, Vector2 position, int mask)
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

                    if (map?.HasCollisionAt(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y, mask) is Point point)
                        return true;

                    return false;
                }

            case PointShape point:
                {
                    Point gridPos = (point.Point.ToVector2() + position).ToGridPoint();
                    return map.HasCollisionAt(gridPos.X, gridPos.Y, 1, 1, mask) is Point;
                }

            case LineShape lineShape:
                {
                    Line2 line = lineShape.LineAtPosition(position.Point());
                    //make a rectangle out of the line segment, check for any tiles in that rectangle

                    //if there are tiles in there, loop through and check each one as a rectangle against the line
                    if (map.HasCollisionAt(Grid.RoundToGrid(line.Left) - 1, Grid.RoundToGrid(line.Top) - 1, Grid.CeilToGrid(line.Width) + 1, Grid.CeilToGrid(line.Height) + 1, mask) is Point)
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
                                if (map.HasCollision(i, j, mask))
                                {
                                    rectX = i * Grid.CellSize;
                                    rectY = j * Grid.CellSize;
                                    var rect = new Rectangle(rectX, rectY, Grid.CellSize, Grid.CellSize);

                                    if (rect.Contains(line.Start.X, line.Start.Y))
                                    {
                                        return true;
                                    }

                                    if (rect.Contains(line.End.X, line.End.Y))
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

                    if (map.HasCollisionAt(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y, mask) is Point point)
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

                    if (map.HasCollisionAt(topLeft.X, topLeft.Y, botRight.X - topLeft.X, botRight.Y - topLeft.Y, mask) is Point point)
                        return true;

                    return false;
                }

            case PolygonShape polygon:
                {
                    var rect = new IntRectangle(Grid.FloorToGrid(polygon.Rect.X), Grid.FloorToGrid(polygon.Rect.Y),
                        Grid.CeilToGrid(polygon.Rect.Width) + 2, Grid.CeilToGrid(polygon.Rect.Height) + 2)
                        .AddPosition(position.ToGridPoint());
                    foreach (var tile in map.GetStaticCollisions(rect))
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


    private readonly static List<NodeInfo<Entity>> _coneCheckCache = new();

    /// <summary>
    /// Checks for collisions in a cone.
    /// </summary>
    public static IEnumerable<Entity> ConeCheck(World world, Vector2 coneStart, float range, float angle, float angleRange, int collisionLayer)
    {
        var coneStart1 = coneStart + new Vector2(-4, -2).Rotate(angle);
        var coneStart2 = coneStart + new Vector2(-4, 2).Rotate(angle);

        var coneEnd = coneStart + new Vector2(range + 4, 0).Rotate(angle);
        var coneEndMin = coneStart + new Vector2(range, 0).Rotate(angle - angleRange / 2f);
        var coneEndMax = coneStart + new Vector2(range, 0).Rotate(angle + angleRange / 2f);

        var polygon = new Polygon([
            coneStart1.Point(),
            coneEndMin.Point(),
            coneEnd.Point(),
            coneEndMax.Point(),
            coneStart2.Point()
        ]);

        Rectangle boundingBox = polygon.GetBoundingBox();

        _coneCheckCache.Clear();
        world.GetUniqueQuadtree().Quadtree.Collision.Retrieve(boundingBox, _coneCheckCache);

        foreach (var other in _coneCheckCache)
        {
            // Should we be cleaning the cache to make sure we get valid entities?
            if (other.EntityInfo.TryGetCollider() is not ColliderComponent collider)
            {
                continue;
            }

            // TODO [PERF] This should be filtered before anything
            if (collider.Layer != collisionLayer)
            {
                continue;
            }

            var otherPosition = other.EntityInfo.GetGlobalTransform().Point;
            foreach (var otherShape in collider.Shapes)
            {
                if (otherShape is LazyShape lazy)
                {
                    if (polygon.Intersect(lazy.Rectangle(otherPosition)))
                        yield return other.EntityInfo;
                }
                else if (otherShape is BoxShape box)
                {
                    if (polygon.Intersect(box.Rectangle.AddPosition(otherPosition)))
                        yield return other.EntityInfo;
                }
                else if (otherShape is PolygonShape poly)
                {
                    if (polygon.CheckOverlapAt(poly.Polygon, otherPosition - coneStart))
                        yield return other.EntityInfo;
                }
                else if (otherShape is CircleShape circle)
                {
                    if (polygon.Intersect(circle.Circle.AddPosition(otherPosition)))
                        yield return other.EntityInfo;
                }
                else
                {
                    GameLogger.Error($"Unknown shape collision attempted {otherShape.GetType().Name}");
                }
            }
        }
    }

    /// <summary>
    /// Find the closest entity on a <paramref name="range"/> according to a <paramref name="collisionLayer"/>.
    /// </summary>
    public static bool FindClosestEntityOnRange(
        World world,
        Vector2 fromPosition,
        float range,
        int collisionLayer,
        HashSet<int> excludeEntities,
        [NotNullWhen(true)] out Entity? target,
        [NotNullWhen(true)] out Vector2? location)
    {
        Rectangle rangeArea = new(fromPosition.X - range / 2f, fromPosition.Y - range / 2f, range, range);

        List<NodeInfo<Entity>> entities = new();
        world.GetUniqueQuadtree().Quadtree.Collision.Retrieve(rangeArea, entities);

        float shortestDistance = float.MaxValue;
        float maximumDistance = range * range;

        target = null;
        location = null;

        foreach (var e in entities)
        {
            if (e.EntityInfo.IsDestroyed)
            {
                continue;
            }

            var collider = e.EntityInfo.GetCollider();

            if (excludeEntities.Contains(e.EntityInfo.EntityId))
            {
                continue;
            }

            // TODO [PERF] This should be filtered before anything
            if (collider.Layer != collisionLayer)
            {
                continue;
            }

            Vector2 otherPosition = e.EntityInfo.GetGlobalTransform().Vector2;
            float distance = (otherPosition - fromPosition).LengthSquared();

            if (distance > maximumDistance)
            {
                continue;
            }

            if (distance < shortestDistance)
            {
                shortestDistance = distance;

                target = e.EntityInfo;
                location = otherPosition;
            }
        }

        return target != null;
    }

    /// <summary>
    /// Removes an ID from the IsColliding component. This is usually handled by TriggerPhysics system, since a message must be sent when exiting a collision.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public static bool RemoveFromCollisionCache(Entity entity, int entityId)
    {
        if (entity.TryGetCollisionCache() is CollisionCacheComponent collisionCache)
        {
            if (collisionCache.HasId(entityId))
            {
                entity.SetCollisionCache(collisionCache.Remove(entityId));
                return true;
            }
        }

        return false;
    }
    public static void AddToCollisionCache(Entity entity, int entityId)
    {
        if (entity.TryGetCollisionCache() is CollisionCacheComponent collisionCache)
        {
            entity.SetCollisionCache(collisionCache.Add(entityId));
        }
        else
        {
            entity.SetCollisionCache(new CollisionCacheComponent(entityId));
        }
    }
    /// <summary>
    /// Check if a trigger is colliding with an actor via the TriggerCollisionSystem.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityId"></param>
    public static bool HasCachedCollisionWith(Entity entity, int entityId)
    {
        if (entity.TryGetCollisionCache() is CollisionCacheComponent collisionCache)
        {
            return collisionCache.HasId(entityId);
        }

        return false;
    }

    /// <summary>
    /// Get all entities that touch this tile.
    /// [PERF] This is not very fast or cached, this can be optimized
    /// </summary>
    public static void GetAllCollisionsAtGrid(World world, Point grid, ref List<NodeInfo<Entity>> output)
    {
        var qt = world.GetUniqueQuadtree().Quadtree;
        Rectangle rectangle = GridHelper.ToRectangle(grid);
        qt.GetCollisionEntitiesAt(rectangle, output);

        // Now, check against other entities.
        for (int i = output.Count - 1; i >= 0; i--)
        {
            var other = output[i];
            if (!other.BoundingBox.Touches(rectangle))
            {
                output.RemoveAt(i);
            }
        }
    }

    public static void AddVelocity(this Entity entity, Vector2 addVelocity)
    {
        Vector2 velocity = entity.TryGetVelocity()?.Velocity ?? Vector2.Zero;
        entity.SetVelocity(velocity + addVelocity);
    }
}