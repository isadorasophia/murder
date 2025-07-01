using Bang;
using Bang.Entities;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Murder.Core.Physics
{
    public class Quadtree
    {
        [JsonIgnore]
        public readonly QTNode<Entity> Collision;

        [JsonIgnore]
        public readonly QTNode<(
            Entity entity,
            IMurderTransformComponent position,
            PushAwayComponent pushAway,
            Vector2 velocity
            )> PushAway;

        [JsonIgnore]
        public readonly QTNode<(Entity entity, SpriteComponent sprite, Vector2 renderPosition)> StaticRender;

        public Quadtree(Rectangle mapBounds)
        {
            Collision = new(0, mapBounds);
            PushAway = new(0, mapBounds);
            StaticRender = new(0, mapBounds);
        }

        public void AddToCollisionQuadTree(Entity entity)
        {
            IMurderTransformComponent pos = entity.GetGlobalTransform();
            if (entity.TryGetCollider() is ColliderComponent collider)
            {
                Collision.Insert(entity.EntityId, entity, collider.GetBoundingBox(pos.Vector2, entity.FetchScale()));
            }

            if (entity.TryGetPushAway() is PushAwayComponent pushAway)
            {
                PushAway.Insert(entity.EntityId, (entity, pos, pushAway, entity.TryGetVelocity()?.Velocity ?? Vector2.Zero), entity.GetColliderBoundingBox());
            }
        }

        public void AddToCollisionQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (!e.IsActive)
                    continue;
                AddToCollisionQuadTree(e);
            }
        }

        public void RemoveFromCollisionQuadTree(int entityId)
        {
            Collision.Remove(entityId);
            PushAway.Remove(entityId);
        }

        public void RemoveFromCollisionQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                Collision.Remove(e.EntityId);
                PushAway.Remove(e.EntityId);
            }
        }

        /// <summary>
        /// Completelly clears and rebuilds the quad tree and pushAway quad tree using a given list of entities
        /// We should avoid this if possible
        /// </summary>
        /// <param name="entities"></param>
        public void UpdateQuadTree(IEnumerable<Entity> entities)
        {
            Collision.Clear();
            PushAway.Clear();

            foreach (var e in entities)
            {
                IMurderTransformComponent pos = e.GetGlobalTransform();
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    Collision.Insert(e.EntityId, e, collider.GetBoundingBox(pos.Point, e.FetchScale()));
                }

                if (e.TryGetPushAway() is PushAwayComponent pushAway)
                {
                    PushAway.Insert(e.EntityId,
                        (
                            e,
                            e.GetGlobalTransform(),
                            e.GetPushAway(),
                            e.TryGetVelocity()?.Velocity ?? Vector2.Zero
                        ), new Rectangle(pos.X, pos.Y, pushAway.Size, pushAway.Size));
                }
            }
        }

        public void RemoveFromStaticRenderQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                StaticRender.Remove(e.EntityId);
            }
        }

        public void AddToStaticRenderQuadTree(IEnumerable<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (!e.IsActive)
                    continue;

                Point position = e.GetGlobalTransform().Point;
                SpriteComponent spriteComponent = e.GetSprite();

                if (Game.Data.TryGetAsset<SpriteAsset>(spriteComponent.AnimationGuid) is not SpriteAsset spriteAsset)
                    continue;

                // This is a greedy size, it will include the object even if it's rotated
                float diameter = (float)Math.Sqrt(spriteAsset.Size.X * spriteAsset.Size.X + spriteAsset.Size.Y * spriteAsset.Size.Y);


                StaticRender.Insert(
                    e.EntityId,
                    (e, spriteComponent, position), new Rectangle(
                        position.X - spriteAsset.Origin.X - spriteComponent.Offset.X * spriteAsset.Size.X - (diameter - spriteAsset.Size.X) / 2f,
                        position.Y - spriteAsset.Origin.Y - spriteComponent.Offset.Y * spriteAsset.Size.Y - (diameter - spriteAsset.Size.Y) / 2f,
                        diameter, diameter));
            }
        }

        public void GetCollisionEntitiesAt(Rectangle boundingBox, List<NodeInfo<Entity>> list)
        {
            Collision.Retrieve(boundingBox, list);
        }

        public static Quadtree GetOrCreateUnique(World world)
        {
            if (world.TryGetUniqueQuadtree() is not QuadtreeComponent qt)
            {
                Rectangle quadTreeSize;
                if (world.TryGetUniqueMap() is MapComponent map)
                {
                    quadTreeSize = new Rectangle(0, 0, map.Width * Grid.CellSize, map.Height * Grid.CellSize);
                }
                else
                {
                    GameLogger.Warning("No size for the map was found!");

                    // TODO: We need to have the city size too!
                    quadTreeSize = new Rectangle(0, 0, 2000, 2000);
                }

                qt = new QuadtreeComponent(quadTreeSize);
                world.AddEntity(qt);
            }

            return qt.Quadtree;
        }
    }
}