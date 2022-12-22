using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Ai;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Systems
{
    [Filter(typeof(CarveComponent))]
    [Watch(typeof(ITransformComponent), typeof(ColliderComponent))]
    internal class MapCarveCollisionSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUnique<MapComponent>().Map;
            foreach (Entity e in entities)
            {
                UntrackEntityOnGrid(map, e);
                TrackEntityOnGrid(map, e);
            }

            PathfindServices.UpdatePathfind(world);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            // We currently do not support moving carve entities.
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            Map map = world.GetUnique<MapComponent>().Map;
            foreach (Entity e in entities)
            {
                UntrackEntityOnGrid(map, e);
                TrackEntityOnGrid(map, e);
            }

            PathfindServices.UpdatePathfind(world);
        }

        private void TrackEntityOnGrid(Map map, Entity e)
        {
            IMurderTransformComponent transform = e.GetGlobalTransform();
            ColliderComponent collider = e.GetCollider();

            if (IsValidCarve(e, collider))
            {
                CarveComponent carve = e.GetCarve();

                IntRectangle rect = collider.GetCarveBoundingBox(transform.Point);
                map.SetOccupiedAsCarve(rect, carve.BlockVision, carve.Obstacle, carve.Weight);
            }
        }

        private void UntrackEntityOnGrid(Map map, Entity e)
        {
            IMurderTransformComponent transform = e.GetGlobalTransform();
            ColliderComponent collider = e.GetCollider();
            CarveComponent carve = e.GetCarve();

            if (!IsValidCarve(e, collider))
            {
                IntRectangle rect = collider.GetCarveBoundingBox(transform.Point);
                map.SetUnoccupiedCarve(rect, carve.BlockVision, carve.Obstacle, carve.Weight);
            }
        }

        private bool IsValidCarve(Entity e, ColliderComponent collider) =>
            collider.Layer == CollisionLayersBase.SOLID && !e.IsDestroyed;
    }
}
