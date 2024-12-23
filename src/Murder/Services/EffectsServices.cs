using Bang;
using Bang.Contexts;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using System.Numerics;

namespace Murder.Services
{
    public static class EffectsServices
    {
        private static int _quickSpriteCount = 0;
        public static void CreateQuickSprite(World world, QuickSpriteInfo info, Entity? parent)
        {
            var e = world.AddEntity(
                new SpriteComponent(info.Sprite, Vector2.Zero, info.Animations, info.YSortOffset, false, OutlineStyle.None, info.TargetSpriteBatch),
                new DestroyOnAnimationCompleteComponent(),
                new PositionComponent(info.Offset),
                new DoNotPersistEntityOnSaveComponent()
            );

            if (parent != null)
            {
                parent.AddChild(e.EntityId, $"quick_sprite_{_quickSpriteCount++}");
            }
        }

        /// <summary>
        /// Add an entity which will apply a "fade-in" effect. Darkening the screen to black.
        /// </summary>
        public static void FadeIn(World world, float time, Color color, float sorting = 0, int? targetBatch = null)
        {
            if (Game.Instance.IsSkippingDeltaTimeOnUpdate)
            {
                return;
            }

            Entity e = world.AddEntity();
            e.SetFadeScreen(new FadeScreenComponent(FadeType.In, Game.NowUnscaled, time, color, sorting: sorting)
                with { TargetBatch = targetBatch });
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

            if (bufferDrawFrames > 0)
            {
                // With buffer frames we must wait until we get Game.Now otherwise we will get an value
                // specially at lower frame rates
                e.SetFadeScreen(new(FadeType.Out, delay, duration, color, string.Empty, 0, bufferDrawFrames));
            }
            else
            {
                e.SetFadeScreen(new(FadeType.Out, Game.NowUnscaled + delay, duration, color, string.Empty, 0, bufferDrawFrames));
            }
        }

        public static void ApplyHighlight(World world, Entity e, HighlightSpriteComponent highlight)
        {
            if (e.HasHighlightOnChildren())
            {
                foreach (int childId in e.Children)
                {
                    world.TryGetEntity(childId)?.SetHighlightSprite(highlight);
                }
            }
            else
            {
                if (!e.HasSprite())
                {
                    e.TryFetchParent()?.SetHighlightSprite(highlight);
                }
                else
                {
                    e.SetHighlightSprite(highlight);
                }
            }
        }

        public static void RemoveHighlight(Entity e)
        {
            if (e.HasHighlightOnChildren())
            {
                foreach (int childId in e.Children)
                {
                    e.TryFetchChild(childId)?.RemoveHighlightSprite();
                }
            }
            else
            {
                e.RemoveHighlightSprite();
                e.TryFetchParent()?.RemoveHighlightSprite();
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