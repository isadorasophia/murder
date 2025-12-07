using Bang;
using Bang.Contexts;
using Bang.Entities;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using System.Numerics;

namespace Murder.Services
{
    public static class EffectsServices
    {
        private static int _quickSpriteCount = 0;
        public static Entity CreateQuickSprite(World world, QuickSpriteInfo info, Entity? parent = null, bool destroyAfter = true)
        {
            Entity e = world.AddEntity(
                new SpriteComponent(info.Sprite, Vector2.Zero, info.Animations, info.YSortOffset, false, OutlineStyle.None, info.TargetSpriteBatch),
                new PositionComponent(info.Offset),
                new DoNotPersistEntityOnSaveComponent(),
                new FlipSpriteComponent(info.Flip)
            );

            if (destroyAfter)
            {
                e.SetDestroyOnAnimationComplete();
            }

            if (parent != null)
            {
                parent.AddChild(e.EntityId, $"quick_sprite_{_quickSpriteCount++}");
            }

            return e;
        }

        /// <summary>
        /// Add an entity which will apply a "fade-in" effect. Darkening the screen to black.
        /// </summary>
        public static Entity? FadeIn(World world, float time, Color color, float sorting = 0, int? targetBatch = null)
        {
            if (Game.Instance.IsSkippingDeltaTimeOnUpdate)
            {
                return null;
            }

            Entity e = world.AddEntity();
            string? customTexture = world.TryGetUniqueCustomFadeScreenStyle()?.CustomFadeImage;

            e.SetFadeScreen(new FadeScreenComponent(FadeType.In, Game.NowUnscaled, time, color, customTexture != null ? Path.Join("images", customTexture) : string.Empty, sorting: sorting)
                with
            { TargetBatch = targetBatch });

            return e;
        }

        /// <summary>
        /// Add an entity which will apply a "fade-out" effect. Clearing the screen.
        /// </summary>
        public static void FadeOut(World world, float duration, Color color, float delay = 0, int bufferDrawFrames = 0)
        {
            if (Game.Instance.IsSkippingDeltaTimeOnUpdate)
            {
                return;
            }

            foreach (var old in world.GetEntitiesWith(typeof(FadeScreenComponent)))
            {
                old.Destroy();
            }

            var e = world.AddEntity();
            string? customTexture = world.TryGetUniqueCustomFadeScreenStyle()?.CustomFadeImage;

            if (bufferDrawFrames > 0)
            {
                // With buffer frames we must wait until we get Game.Now otherwise we will get an value
                // specially at lower frame rates
                e.SetFadeScreen(new(FadeType.Out, delay, duration, color, customTexture != null ? Path.Join("images", customTexture) : string.Empty, 0, bufferDrawFrames));
            }
            else
            {
                e.SetFadeScreen(new(FadeType.Out, Game.NowUnscaled + delay, duration, color, customTexture != null ? Path.Join("images", customTexture) : string.Empty, 0, bufferDrawFrames));
            }
        }

        public static void ApplyHighlight(World world, Entity e, HighlightSpriteComponent highlight)
        {
            if (e.TryGetHighlightOnChildren() is HighlightOnChildrenComponent childrenHighlight)
            {
                Entity root = EntityServices.FindRootEntity(e);
                if (childrenHighlight.Child is string child)
                {
                    root.TryFetchChild(child)?.SetHighlightSprite(highlight);
                }
                else
                {
                    foreach (int childId in root.Children)
                    {
                        world.TryGetEntity(childId)?.SetHighlightSprite(highlight);
                    }
                }
            }
            else
            {
                if (!e.HasSprite())
                {
                    Entity root = EntityServices.FindRootEntity(e);
                    root.SetHighlightSprite(highlight);
                }
                else
                {
                    e.SetHighlightSprite(highlight);
                }
            }
        }

        public static void RemoveHighlight(Entity e)
        {
            if (e.TryGetHighlightOnChildren() is HighlightOnChildrenComponent childrenHighlight)
            {
                Entity root = EntityServices.FindRootEntity(e);
                if (childrenHighlight.Child is string child)
                {
                    root.TryFetchChild(child)?.RemoveHighlightSprite();
                }
                else
                {
                    foreach (int childId in root.Children)
                    {
                        root.TryFetchChild(childId)?.RemoveHighlightSprite();
                    }
                }
            }
            else
            {
                if (!e.HasSprite())
                {
                    Entity root = EntityServices.FindRootEntity(e);
                    root.RemoveHighlightSprite();
                }
                else
                {
                    e.RemoveHighlightSprite();
                }
            }
        }

        public static void PlayAnimationAt(World world, Portrait blastAnimation, Vector2 position)
        {
            world.AddEntity(
                new PositionComponent(position),
                new SpriteComponent(blastAnimation),
                new DestroyOnAnimationCompleteComponent()
            );
        }

        public static void RemoveSolid(Entity e)
        {
            Entity? target = e.HasCollider() ? e : e.TryFetchChild("solid");
            if (target is null)
            {
                return;
            }

            if (target.TryGetCollider() is ColliderComponent collider && (collider.Layer & CollisionLayersBase.SOLID) != 0)
            {
                target.SetCollider(collider.WithoutLayerFlag(CollisionLayersBase.SOLID));
            }
        }
    }
}