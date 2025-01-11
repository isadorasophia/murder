using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Ai;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Systems
{
    [Filter(typeof(CarveComponent), typeof(ColliderComponent))]
    [Watch(typeof(ITransformComponent), typeof(ColliderComponent), typeof(CarveComponent))]
    public class MapCarveCollisionSystem : IReactiveSystem
    {
        private readonly Dictionary<int, IntRectangle> _trackEntitiesPreviousPosition = [];

        public virtual void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUniqueMap().Map;
            foreach (Entity e in entities)
            {
                IntRectangle rect = GetCarveBoundingBox(e);

                UntrackEntityOnGrid(map, e, rect, force: false);
                TrackEntityOnGrid(map, e, rect);

                _trackEntitiesPreviousPosition[e.EntityId] = rect;
            }

            PathfindServices.UpdatePathfind(world);
        }

        public virtual void OnModified(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUniqueMap().Map;
            foreach (Entity e in entities)
            {
                if (!e.IsActive)
                {
                    continue;
                }

                IntRectangle updatedRectangle = GetCarveBoundingBox(e);

                if (!_trackEntitiesPreviousPosition.TryGetValue(e.EntityId, out IntRectangle previousRectangle))
                {
                    GameLogger.Error($"How did entity {e.EntityId} was not tracked by the map carve system?");
                    previousRectangle = updatedRectangle;
                }

                UntrackEntityOnGrid(map, e, previousRectangle, force: true);

                UntrackEntityOnGrid(map, e, updatedRectangle, force: false);
                TrackEntityOnGrid(map, e, updatedRectangle);

                _trackEntitiesPreviousPosition[e.EntityId] = updatedRectangle;
            }

            // We currently do not support updating pathfinding for moving carve entities.
        }

        public virtual void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUniqueMap().Map;
            foreach (Entity e in entities)
            {
                IntRectangle updatedRectangle = GetCarveBoundingBox(e);

                if (!_trackEntitiesPreviousPosition.TryGetValue(e.EntityId, out IntRectangle previousRectangle))
                {
                    GameLogger.Error($"How did entity {e.EntityId} was not tracked by the map carve system?");
                    previousRectangle = updatedRectangle;
                }

                UntrackEntityOnGrid(map, e, previousRectangle, force: true);
                TrackEntityOnGrid(map, e, previousRectangle);

                _trackEntitiesPreviousPosition.Remove(e.EntityId);
            }

            PathfindServices.UpdatePathfind(world);
        }

        public virtual void OnActivated(World world, ImmutableArray<Entity> entities) 
        {
            OnAdded(world, entities);
        }

        public virtual void OnDeactivated(World world, ImmutableArray<Entity> entities) 
        {
            OnRemoved(world, entities);
        }

        protected void TrackEntityOnGrid(Map map, Entity e, IntRectangle rect)
        {
            ColliderComponent collider = e.GetCollider();
            if (e.TryGetCarve() is not CarveComponent carve)
            {
                return;
            }
            
            if (IsValidCarve(e, collider, carve))
            {
                map.SetOccupiedAsCarve(rect, carve.BlockVision, carve.Obstacle, carve.ClearPath, carve.Weight);
            }
        }

        protected void UntrackEntityOnGrid(Map map, Entity e, IntRectangle rect, bool force)
        {
            ColliderComponent collider = e.GetCollider();
            if (e.TryGetCarve() is not CarveComponent carve)
            {
                return;
            }

            if (force || !IsValidCarve(e, collider, carve))
            {
                map.SetUnoccupiedCarve(rect, carve.BlockVision, carve.Obstacle, carve.Weight);
            }
        }

        protected bool IsValidCarve(Entity e, ColliderComponent collider, CarveComponent carve) =>
            !e.IsDestroyed && (collider.Layer == CollisionLayersBase.SOLID ||
                               collider.Layer == CollisionLayersBase.PATHFIND || carve.ClearPath);

        private IntRectangle GetCarveBoundingBox(Entity e)
        {
            Vector2 position = e.GetGlobalTransform().Vector2;

            ColliderComponent collider = e.GetCollider();
            IntRectangle updatedRectangle = collider.GetCarveBoundingBox(position);

            return updatedRectangle;
        }
    }
}