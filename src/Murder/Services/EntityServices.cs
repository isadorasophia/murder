using Bang;
using Bang.Entities;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace Murder.Services
{
    public static class EntityServices
    {
        public static void RotateChildPositions(World world, Entity entity, float angle)
        {
            foreach (var id in entity.Children)
            {
                if (world.TryGetEntity(id) is Entity child)
                {
                    Vector2 localPosition = child.GetTransform().Vector2;
                    child.SetTransform(new PositionComponent(localPosition.Rotate(angle)));
                }
            }
        }
        
        public static void RotatePositionAround(Entity entity, Vector2 center, float angle)
        {
            Vector2 localPosition = entity.GetTransform().Vector2 - center;
            entity.SetTransform(new PositionComponent(center + localPosition.Rotate(angle)));
        }
        
        public static void RotatePosition(Entity entity, float angle)
        {
            Vector2 localPosition = entity.GetTransform().Vector2;
            entity.SetTransform(new PositionComponent(localPosition.Rotate(angle)));
        }

        public static bool IsChildOf(World world, Entity parent, Entity child)
        {
            if (child.Parent.HasValue)
            {
                if (child.Parent.Value == parent.EntityId)
                {
                    return true;
                }
                else
                {
                    if (world.TryGetEntity(child.Parent.Value) is Entity parentEntity)
                    {
                        return IsChildOf(world, parent, parentEntity);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Recursivelly get all children of this entity, including itself.
        /// </summary>
        /// <param name="world">World that this entity belongs.</param>
        /// <param name="entity">Entity target.</param>
        internal static IEnumerable<int> GetAllTreeOfEntities(World world, Entity entity)
        {
            // First, return ourselves.
            yield return entity.EntityId;

            foreach (int child in GetAllChildren(world, entity))
            {
                yield return child;
            }
        }
        
        internal static IEnumerable<int> GetAllChildren(World world, Entity entity)
        {
            foreach (int childId in entity.Children)
            {
                if (world.TryGetEntity(childId) is not Entity child)
                {
                    // Child died...?
                    continue;
                }
                
                // Return the child and its children
                foreach (int entityIdInTree in GetAllTreeOfEntities(world, child))
                {
                    yield return entityIdInTree;
                }
            }
        }

        public static Entity? FindRootEntity(Entity e)
        {
            if (e.Parent is null || e.TryFetchParent() is not Entity parent)
            {
                return e;
            }

            return FindRootEntity(parent);
        }

        public static AsepriteComponent? TryPlayAsepriteAnimationNext(this Entity entity, string animationName)
        {

            if (entity.TryGetAseprite() is AsepriteComponent aseprite)
            {
                AsepriteComponent result = aseprite.PlayAfter(animationName);
                entity.SetAseprite(result);
                entity.RemoveAnimationComplete();

                return result;
            }

            return null;
        }
        public static AsepriteComponent? PlayAsepriteAnimationNext(this Entity entity, string animationName)
        {
            if (TryPlayAsepriteAnimationNext(entity, animationName) is AsepriteComponent result)
            {
                return result;
            }

            GameLogger.Error("Entity doesn's have an Aseprite component");
            return null;
        }
        public static AsepriteComponent? TryPlayAsepriteAnimation(this Entity entity, params string[] nextAnimations)
        {
            if (entity.TryGetAseprite() is AsepriteComponent aseprite)
            {
                if (aseprite.IsPlaying(nextAnimations))
                    return aseprite;

                AsepriteComponent result = aseprite.Play(!entity.HasPauseAnimation(), nextAnimations);
                entity.SetAseprite(result);
                entity.RemoveAnimationComplete();

                return result;
            }

            return null;
        }
        public static AsepriteComponent? PlayAsepriteAnimation(this Entity entity, params string[] nextAnimations)
        {
            if (TryPlayAsepriteAnimation(entity, nextAnimations) is AsepriteComponent aseprite)
            {
                return aseprite;
            }

            GameLogger.Error("Entity doesn's have an Aseprite component");
            return null;
        }
        public static AsepriteComponent? PlayAsepriteAnimation(this Entity entity, ImmutableArray<string> animations)
        {
            if (entity.TryGetAseprite() is AsepriteComponent aseprite)
            {
                AsepriteComponent result = aseprite.Play(!entity.HasPauseAnimation(), animations);
                entity.SetAseprite(result);
                entity.RemoveAnimationComplete();
                
                return result;
            }

            GameLogger.Error("Entity doesn's have an Aseprite component");
            return null;
        }
    }
}
