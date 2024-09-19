using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(SpriteComponent), typeof(ITransformComponent), typeof(InCameraComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(InvisibleComponent), typeof(ThreeSliceComponent))]
    public class SpriteRenderSystem : IStartupSystem, IExitSystem, IMurderRenderSystem
    {
        private readonly Dictionary<Guid, SpriteAsset> _spriteAssetCache = [];
        
        public void Draw(RenderContext render, Context context)
        {
            bool issueSlowdownWarning = false;

            foreach (Entity e in context.Entities)
            {
                ImageFlip flip = ImageFlip.None;

                IMurderTransformComponent transform = e.GetGlobalTransform();
                SpriteComponent s = e.GetSprite();

                if (!_spriteAssetCache.TryGetValue(s.AnimationGuid, out SpriteAsset? asset))
                {
                    if (Game.Data.TryGetAsset<SpriteAsset>(s.AnimationGuid) is not SpriteAsset loadedAsset)
                        continue;

                    _spriteAssetCache[s.AnimationGuid] = loadedAsset;
                    asset = loadedAsset;
                }


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

                float ySort = e.HasUiDisplay() ? e.GetUiDisplay().YSort : RenderServices.YSort(ySortOffsetRaw + 0.01f * (e.EntityId % 20));

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
                        s.NextAnimations.Length <= 1 &&             // if this is a sequence, don't loop
                        !e.HasDoNotLoop() &&                       // if this has the DoNotLoop component, don't loop
                        !e.HasDestroyOnAnimationComplete() &&     // if you want to destroy this, don't loop
                        (overload == null || (overload.Value.AnimationCount == 1 && overload.Value.Loop))
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

                var frameInfo = RenderServices.DrawSprite(
                    render.GetBatch(s.TargetSpriteBatch),
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
                        Sort = ySort,
                        OutlineStyle = s.HighlightStyle,
                        Outline = e.TryGetHighlightSprite()?.Color ?? null,
                    }, animationInfo);

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
                    LastFrameIndex = frameInfo.InternalFrame
                });

                if (frameInfo.Failed)
                    continue;

                // Animations do not send complete messages until the current sequence is done
                if (frameInfo.Complete)
                {
                    // Handle animation sequences imported by Aseprite and baked into the asset
                    if (frameInfo.Animation.NextAnimation is AnimationSequence sequence)
                    {
                        if (Game.Random.TryWithChanceOf(sequence.Chance))
                        {
                            if (!string.IsNullOrWhiteSpace(sequence.Next))
                                e.PlaySpriteAnimation(sequence.Next);
                        }
                        else
                        {
                            e.PlaySpriteAnimation(animation);
                        }
                    }
                    else
                    {
                        RenderServices.DealWithCompleteAnimations(e, s);
                    }
                }

            }

            if (issueSlowdownWarning)
            {
                GameLogger.Warning("Animation event loop reached. Breaking out of loop. This was probably caused by a major slowdown.");
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

        public void ClearCache()
        {
            _spriteAssetCache.Clear();
        }

        public void Start(Context context)
        {
#if DEBUG
            Game.Data.TrackOnHotReloadSprite(ClearCache);
#endif
        }

        public void Exit(Context context)
        {
#if DEBUG
            Game.Data.UntrackOnHotReloadSprite(ClearCache);
#endif
        }
    }
}