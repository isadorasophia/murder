using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using System;
using System.Collections.Immutable;
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
                new FlipSpriteComponent(info.Flip),
                new TintComponent(info.Tint)
            );

            if (destroyAfter)
            {
                e.SetDestroyOnAnimationComplete(DestroyOnAnimationCompleteFlags.Destroy);
            }

            if (parent != null)
            {
                parent.AddChild(e.EntityId, $"quick_sprite_{_quickSpriteCount++}");
            }

            return e;
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