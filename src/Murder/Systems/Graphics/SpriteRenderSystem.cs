using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Helpers;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(SpriteComponent), typeof(ITransformComponent), typeof(InCameraComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(InvisibleComponent), typeof(ThreeSliceComponent))]
    public class SpriteRenderSystem : IMurderRenderSystem, IFixedUpdateSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                ImageFlip flip = ImageFlip.None;

                IMurderTransformComponent transform = e.GetGlobalTransform();
                SpriteComponent s = e.GetSprite();
                
                if (Game.Data.TryGetAsset<SpriteAsset>(s.AnimationGuid) is not SpriteAsset asset)
                    continue;

                Vector2 renderPosition;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                {
                    renderPosition = (transform.Vector2 + render.Camera.Position * (1 - parallax.Factor)).Round();
                }
                else
                {
                    renderPosition = transform.Vector2;
                }

                // Handle rotation
                FacingComponent? facing = s.RotateWithFacing || s.FlipWithFacing ? e.TryGetFacing() : null;
                float rotation = transform.Angle;

                if (e.TryGetRotation() is RotationComponent RotationComponent)
                {
                    rotation += RotationComponent.Rotation;
                }

                if (facing is not null)
                {
                    if (s.RotateWithFacing) rotation += facing.Value.Angle;
                    // Currently we never flip sprites vertically with facing, so just assign the horizontal flip.
                    if (s.FlipWithFacing && facing.Value.Direction.Flipped()) flip = ImageFlip.Horizontal;
                }

                // Handle color
                var tintColor = e.TryGetTint()?.TintColor ?? Color.White;
                var color = tintColor * GetInheritedAlpha(e);

                BlendStyle blend;
                // Handle flashing
                if (e.HasFlashSprite())
                {
                    blend = BlendStyle.Wash;
                }
                else
                {
                    blend = BlendStyle.Normal;
                }

                float ySortOffsetRaw = transform.Y + s.YSortOffset;

                string animation = s.CurrentAnimation;
                
                
                float startTime = e.TryGetAnimationStarted()?.StartTime ?? (s.UseUnscaledTime ? Game.Now : Game.NowUnscaled);

                AnimationOverloadComponent? overload = null;
                if (e.TryGetAnimationOverload() is AnimationOverloadComponent o)
                {
                    startTime = o.Start;
                    if (o.CustomSprite is SpriteAsset customSprite)
                    {
                        asset = customSprite;
                    }

                    if (asset.Animations.ContainsKey(o.CurrentAnimation))
                    {
                        animation = o.CurrentAnimation;
                    }

                    ySortOffsetRaw += o.SortOffset;
                }

                float ySort = e.HasUiDisplay() ? e.GetUiDisplay().YSort : RenderServices.YSort(ySortOffsetRaw);

                VerticalPositionComponent? verticalPosition = e.TryGetVerticalPosition();
                if (verticalPosition is not null)
                {
                    renderPosition = new Vector2(renderPosition.X, renderPosition.Y - verticalPosition.Value.Z);
                }

                var animationInfo = new AnimationInfo()
                {
                    Name = animation,
                    Start = startTime,
                    UseScaledTime = !e.HasPauseAnimation() && !s.UseUnscaledTime,
                    Loop = 
                        s.NextAnimations.Length <= 1 &&  // if this is a sequence, don't loop
                        !e.HasDestroyOnAnimationComplete() &&     // if you want to destroy this, don't loop
                        (overload == null || (overload.Value.AnimationCount == 1 && overload.Value.Loop)) // if this is 
                };

                var scale = e.TryGetScale()?.Scale ?? Vector2.One;

                Rectangle clip = Rectangle.Empty;
                if (e.TryGetSpriteClippingRect() is SpriteClippingRectComponent spriteClippingRect)
                {
                    clip = new Rectangle(
                        (int)(spriteClippingRect.BorderLeft),
                        (int)(spriteClippingRect.BorderUp),
                        (int)(asset.Size.X - spriteClippingRect.BorderRight),
                        (int)(asset.Size.Y - spriteClippingRect.BorderDown));

                    renderPosition += new Vector2(spriteClippingRect.BorderLeft, spriteClippingRect.BorderUp);
                }

                if (e.TryGetSpriteOffset() is SpriteOffsetComponent offset)
                {
                    renderPosition += offset.Offset;
                }

                var frameInfo = RenderServices.DrawSprite(
                    render.GetBatch(s.TargetSpriteBatch),
                    asset.Guid,
                    renderPosition,
                    new DrawInfo(ySort)
                    {
                        Clip = clip,
                        Origin = s.Offset,
                        ImageFlip = flip,
                        Rotation = rotation,
                        Scale = scale,
                        Color = color,
                        BlendMode = blend,
                        Sort = ySort,
                        OutlineStyle = s.HighlightStyle,
                        Outline = e.TryGetHighlightSprite()?.Color ?? null,
                    }, animationInfo);

                e.SetRenderedSpriteCache(new RenderedSpriteCacheComponent() with
                {
                    RenderedSprite = asset.Guid,
                    CurrentAnimation = frameInfo.Animation,
                    RenderPosition = renderPosition,
                    Offset = s.Offset,
                    ImageFlip = flip,
                    Rotation = rotation,
                    Scale = scale,
                    Color = color,
                    Blend = blend,
                    Outline = s.HighlightStyle,
                    AnimInfo = animationInfo,
                    Sorting = ySort,
                });

                if (frameInfo.Failed)
                    continue;

                // Animations do not send complete messages until the current sequence is done
                if (frameInfo.Complete)
                {
                    if (frameInfo.Animation.NextAnimation is AnimationSequence sequence)
                    {
                        if (Game.Random.TryWithChanceOf(sequence.Chance))
                        {
                            if (!string.IsNullOrWhiteSpace(sequence.Next))
                                e.PlaySpriteAnimation(sequence.Next);

                            e.SendMessage(new AnimationCompleteMessage());
                            e.RemoveAnimationComplete();
                        }
                        else
                        {
                            e.PlaySpriteAnimation(s.NextAnimations);

                            e.SendMessage(new AnimationCompleteMessage());
                            e.RemoveAnimationComplete();
                        }
                    }
                    else
                    {
                        RenderServices.MessageCompleteAnimations(e, s);
                    }
                }

            }
        }

        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                if (e.TryGetRenderedSpriteCache() is not RenderedSpriteCacheComponent cache)
                    continue;

                SpriteComponent sprite = e.GetSprite();
                RenderServices.TriggerEventsIfNeeded(e, cache, sprite.UseUnscaledTime);
            }
        }

        private float GetInheritedAlpha(Entity entity)
        {
            float alpha = 1;
            if (entity.TryFetchParent() is Entity parent)
            {
                alpha = GetInheritedAlpha(parent);
            }

            return alpha * (entity.TryGetAlpha()?.Alpha ?? 1.0f);
        }
    }
}