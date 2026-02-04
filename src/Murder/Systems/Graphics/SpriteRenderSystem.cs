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
using Murder.Diagnostics;
using Murder.Prefabs;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;
using System.Security.AccessControl;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(SpriteComponent), typeof(ITransformComponent), typeof(InCameraComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(InvisibleComponent), typeof(ThreeSliceComponent))]
    public class SpriteRenderSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            DebugSnapshot.StartStopwatch("Sprite Render System");
            bool issueSlowdownWarning = false;

            foreach (Entity e in context.Entities)
            {
                Vector2 position = e.GetGlobalPosition();
                SpriteComponent s = e.GetSprite();

                if (Game.Data.TryGetAsset<SpriteAsset>(s.AnimationGuid) is not SpriteAsset asset)
                {
                    GameLogger.Error($"Sprite GUID not found {s.AnimationGuid}");
                    continue;
                }

                ImageFlip flip = ImageFlip.None;

                if (e.TryGetFlipSprite() is FlipSpriteComponent flipSprite)
                {
                    flip = flipSprite.Orientation;
                }

                Vector2 renderPosition;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                {
                    renderPosition = (position + render.Camera.Position * (1 - parallax.Factor)).Round();
                }
                else
                {
                    renderPosition = position;
                }

                // Handle rotation
                float rotation = e.GetPosition().Angle;

                if (e.TryGetRotation() is RotationComponent RotationComponent)
                {
                    rotation += RotationComponent.Rotation;
                }

                if (s.RotateWithFacing && e.TryGetFacing() is FacingComponent facing)
                {
                    rotation += facing.Angle;
                }

                Entity root = EntityServices.FindRootEntity(e);

                // Handle color
                var tintColor = e.TryGetTint()?.TintColor ?? Color.White;
                var color = tintColor * GetInheritedAlpha(root, e);

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

                MurderBlendState blendState;
                if (e.TryGetSpriteBlend() is SpriteBlendComponent spriteBlend)
                {
                    blendState = spriteBlend.BlendState;
                }
                else
                {
                    blendState = MurderBlendState.AlphaBlend;
                }

                float ySortOffsetRaw = 0;

                string animationId = s.CurrentAnimation;
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
                        animationId = o.CurrentAnimation;
                    }

                    ySortOffsetRaw += o.SortOffset;
                }

                float yPositionForYSort = position.Y;

                int? forceFrame = null;
                if (e.TryGetSpriteFrame() is SpriteFrameComponent spriteFrame && spriteFrame.Animation.Equals(animationId))
                {
                    forceFrame = spriteFrame.Frame;
                    yPositionForYSort = spriteFrame.Y;
                }

                ySortOffsetRaw += yPositionForYSort + s.YSortOffset;
                float ySort = e.HasUiDisplay() ? e.GetUiDisplay().YSort : RenderServices.YSort(ySortOffsetRaw + 0.01f * (e.EntityId % 20));

                VerticalPositionComponent? verticalPosition = e.TryGetVerticalPosition() ?? root.TryGetVerticalPosition();
                if (verticalPosition is not null)
                {
                    renderPosition = new Vector2(renderPosition.X, renderPosition.Y - verticalPosition.Value.Z);
                }

                if (!asset.Animations.TryGetValue(animationId, out var animation))
                {
                    GameLogger.Log($"Couldn't find animation {animationId} for {asset.Guid}.");
                    continue;
                }

                var animationInfo = new AnimationInfo()
                {
                    Name = animationId,
                    Start = startTime,
                    UseScaledTime = !e.HasPauseAnimation() && !s.UseUnscaledTime,
                    Loop =
                        s.NextAnimations.Length <= 1 &&              // if this is a sequence, don't loop
                        animation.NextAnimation.Length == 0 &&      //
                        !e.HasDoNotLoop() &&                       // if this has the DoNotLoop component, don't loop
                        !e.HasDestroyOnAnimationComplete() &&     // if you want to destroy this, don't loop
                        (overload == null || (overload.Value.AnimationCount == 1 && overload.Value.Loop)),
                    ForceFrame = forceFrame
                };

                var scale = e.TryGetScale()?.Scale ?? Vector2.One;

                Rectangle clip = Rectangle.Empty;
                if (e.TryGetSpriteClippingRect() is SpriteClippingRectComponent spriteClippingRect)
                {
                    clip = spriteClippingRect.GetClippingRect(asset.Size);
                    renderPosition += new Vector2(clip.Left, clip.Top);
                }

                if (e.TryGetSpriteOffset() is SpriteOffsetComponent offset)
                {
                    renderPosition += offset.Offset;
                }

                Color? outlineColor = e.HasDeactivateHighlightSprite() ? null : e.TryGetHighlightSprite()?.Color;

                FrameInfo frameInfo;
                Batch2D batch2D;
                if (e.TryGetCustomTargetSpriteBatch() is CustomTargetSpriteBatchComponent customTargetSpriteBatch)
                {
                    batch2D = render.GetBatch(customTargetSpriteBatch.TargetBatch);
                }
                else
                {
                    batch2D = render.GetBatch(s.TargetSpriteBatch);
                }

                if (animationInfo.ForceFrame.HasValue)
                {
                    frameInfo = RenderServices.DrawSprite(
                        batch2D,
                        renderPosition,
                        clip,
                        animationId,
                        asset,
                        animationInfo.ForceFrame.Value,
                        s.Offset,
                        flip,
                        rotation,
                        scale,
                        color,
                        blend.ToVector3(),
                        ySort);
                }
                else
                {
                    frameInfo = RenderServices.DrawSprite(
                        batch2D,
                        asset,
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
                            BlendState = blendState,
                            Sort = ySort,
                            OutlineStyle = s.HighlightStyle,
                            Outline = outlineColor,
                        }, animationInfo);
                }

                issueSlowdownWarning = RenderServices.TriggerEventsIfNeeded(e, s.AnimationGuid, animationInfo, frameInfo);

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
                    LastFrameIndex = frameInfo.InternalFrame,
                    SpriteSize = asset.Size
                });

                if (frameInfo.Failed)
                {
                    GameLogger.Warning($"Sprite render failed!");
                    continue;
                }

                // Animations do not send complete messages until the current sequence is done
                if (frameInfo.Complete)
                {
                    if (frameInfo.Animation.NextAnimation.Length > 0)
                    {
                        // Handle animation sequences imported by Aseprite and baked into the asset
                        if (frameInfo.Animation.GetNextAnimation(Game.Random, out string next))
                        {
                            e.PlaySpriteAnimation(next);
                        }
                        else
                        // Be careful since this only runs when the Entity is in camera this can cause animation loops to get out of sync.
                        {
                            e.PlaySpriteAnimation(animationId);
                        }

                    }
                    RenderServices.DealWithCompleteAnimations(e, s);
                }
            }

            if (issueSlowdownWarning)
            {
                GameLogger.Warning("Animation event loop reached. Breaking out of loop. This was probably caused by a major slowdown.");
            }

            DebugSnapshot.PauseStopwatch();
        }

        private float GetInheritedAlpha(Entity root, Entity entity)
        {
            float alpha = entity.TryGetAlpha()?.Alpha ?? 1.0f;
            if (entity.Parent is null)
            {
                return alpha;
            }

            float parentAlpha = root.TryGetAlpha()?.Alpha ?? 1.0f;
            return parentAlpha * alpha;
        }
    }
}