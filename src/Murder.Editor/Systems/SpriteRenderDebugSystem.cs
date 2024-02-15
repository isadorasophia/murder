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
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Helpers;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems;

[Filter(typeof(ITransformComponent))]
[Filter(filter: ContextAccessorFilter.AnyOf, typeof(SpriteComponent), typeof(AgentSpriteComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(ThreeSliceComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(InvisibleComponent))]
internal class SpriteRenderDebugSystem : IFixedUpdateSystem, IMurderRenderSystem
{
    private float _previousLastTime = 0;

    public void FixedUpdate(Context context)
    {
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

        float currentTime = Game.NowUnscaled;
        if (hook is TimelineEditorHook timeline)
        {
            currentTime = timeline.Time;
        }

        foreach (Entity e in context.Entities)
        {
            if (e.TryGetRenderedSpriteCache() is not RenderedSpriteCacheComponent cache)
            {
                continue;
            }

            RenderServices.TriggerEventsIfNeeded(e, cache, previousTime: _previousLastTime, currentTime);
        }

        _previousLastTime = currentTime;
    }

    public void Draw(RenderContext render, Context context)
    {
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

        float overrideCurrentTime = -1;
        if (hook is TimelineEditorHook timeline)
        {
            overrideCurrentTime = timeline.Time;
        }

        foreach (var e in context.Entities)
        {
            if (hook.HideStatic && e.HasStatic())
            {
                continue;
            }

            SpriteComponent? sprite = e.TryGetSprite();
            AgentSpriteComponent? agentSprite = e.TryGetAgentSprite();
            IMurderTransformComponent transform = e.GetGlobalTransform();

            string animationId;
            SpriteAsset? asset;
            float start;
            ImageFlip flip = ImageFlip.None;

            float ySortOffsetRaw;

            Vector2 boundsOffset = Vector2.Zero;
            if (sprite.HasValue)
            {
                animationId = sprite.Value.CurrentAnimation;
                asset = Game.Data.TryGetAsset<SpriteAsset>(sprite.Value.AnimationGuid);
                start = 0;
                boundsOffset = sprite.Value.Offset;

                ySortOffsetRaw = sprite.Value.YSortOffset;
            }
            else
            {
                (animationId, asset, start, flip) = GetAgentSpriteSettings(e);

                ySortOffsetRaw = agentSprite is not null ? agentSprite.Value.YSortOffset : 0;
            }

            if (asset is null)
            {
                continue;
            }

            ySortOffsetRaw += transform.Y;

            AnimationOverloadComponent? overload = null;
            if (e.TryGetAnimationOverload() is AnimationOverloadComponent o)
            {
                overload = o;
                animationId = o.CurrentAnimation;

                start = o.Start;
                if (o.CustomSprite is SpriteAsset customSprite)
                {
                    asset = customSprite;
                }

                ySortOffsetRaw += o.SortOffset;
            }

            Vector2 renderPosition;
            if (e.TryGetParallax() is ParallaxComponent parallax)
            {
                renderPosition = (transform.Vector2 + render.Camera.Position * (1 - parallax.Factor)).Point();
            }
            else
            {
                renderPosition = transform.Point;
            }

            // Handle alpha
            float alpha;
            if (e.TryGetAlpha() is AlphaComponent alphaComponent)
            {
                alpha = alphaComponent.Alpha;
            }
            else
            {
                alpha = 1f;
            }

            // This is as early as we can to check for out of bounds
            if (!render.Camera.Bounds.Touches(new Rectangle(renderPosition - asset.Size * boundsOffset - asset.Origin, asset.Size)))
                continue;

            Vector2 offset = sprite.HasValue ? sprite.Value.Offset : Vector2.Zero;
            Batch2D batch = sprite.HasValue ? render.GetBatch(sprite.Value.TargetSpriteBatch) :
                render.GameplayBatch;

            int ySortOffset = sprite.HasValue ? sprite.Value.YSortOffset : agentSprite!.Value.YSortOffset;
            if (e.HasComponent<ShowYSortComponent>())
            {
                RenderServices.DrawHorizontalLine(
                render.DebugBatch,
                (int)render.Camera.Bounds.Left,
                (int)(transform.Y + ySortOffset),
                (int)render.Camera.Bounds.Width,
                Color.BrightGray,
                0.2f);
            }

            float rotation = transform.Angle;

            if (e.TryGetRotation() is RotationComponent RotationComponent)
            {
                rotation += RotationComponent.Rotation;
            }

            if (sprite.HasValue && e.TryGetFacing() is FacingComponent facing)
            {
                if (sprite.Value.RotateWithFacing)
                    rotation += facing.Angle;

                if (sprite.Value.FlipWithFacing)
                {
                    flip = facing.Direction.GetFlipped();
                }

                if (e.TryGetSpriteFacing() is SpriteFacingComponent spriteFacing)
                {
                    (animationId, var horizontalFlip)= spriteFacing.GetSuffixFromAngle(facing.Angle);
                    flip = horizontalFlip ? ImageFlip.Horizontal : ImageFlip.None;
                }
            }

            float ySort = RenderServices.YSort(ySortOffsetRaw);

            Color baseColor = e.TryGetTint()?.TintColor ?? Color.White;
            if (e.HasComponent<IsPlacingComponent>())
            {
                baseColor *= .5f;
            }
            else
            {
                baseColor *= alpha;
            }

            Vector2 scale = e.TryGetScale()?.Scale ?? Vector2.One;

            // Cute tween effect when placing components, if any.
            float placedTime = e.TryGetComponent<PlacedInWorldComponent>()?.PlacedTime ?? 0;
            if (placedTime != 0)
            {
                float modifier = Calculator.ClampTime(Game.NowUnscaled - placedTime, .75f);
                if (modifier == 1)
                {
                    e.RemoveComponent<PlacedInWorldComponent>();
                }

                scale -= Ease.ElasticIn(.9f - modifier * .9f) * scale;
            }

            Rectangle clip = Rectangle.Empty;
            if (e.TryGetSpriteClippingRect() is SpriteClippingRectComponent spriteClippingRect)
            {
                clip = new Rectangle(
                    (int)(spriteClippingRect.BorderLeft),
                    (int)(spriteClippingRect.BorderUp),
                    (int)(asset.Size.X - spriteClippingRect.BorderRight - spriteClippingRect.BorderLeft),
                    (int)(asset.Size.Y - spriteClippingRect.BorderDown - spriteClippingRect.BorderUp));

                renderPosition += new Vector2(spriteClippingRect.BorderLeft, spriteClippingRect.BorderUp);
            }

            if (e.TryGetSpriteOffset() is SpriteOffsetComponent spriteOffset)
            {
                renderPosition += spriteOffset.Offset;
            }

            AnimationInfo info = new AnimationInfo(animationId, start) with { OverrideCurrentTime = overrideCurrentTime };
            FrameInfo frameInfo = RenderServices.DrawSprite(
                batch,
                asset.Guid,
                renderPosition,
                new DrawInfo()
                {
                    Clip = clip,
                    Origin = offset,
                    ImageFlip = flip,
                    Rotation = rotation,
                    Sort = ySort,
                    Scale = scale,
                    Color = baseColor,
                    Outline = e.HasComponent<IsSelectedComponent>() ? Color.White.FadeAlpha(0.65f) : null,
                },
                info);

            e.SetRenderedSpriteCache(new RenderedSpriteCacheComponent() with
            {
                RenderedSprite = asset.Guid,
                CurrentAnimation = frameInfo.Animation,
                RenderPosition = renderPosition,
                Offset = sprite?.Offset ?? Vector2.Zero,
                ImageFlip = flip,
                Rotation = rotation,
                Scale = scale,
                Color = baseColor,
                Outline = sprite?.HighlightStyle ?? OutlineStyle.None,
                AnimInfo = info,
                Sorting = ySort,
            });

            if (frameInfo.Complete && overload != null)
            {
                if (overload.Value.AnimationCount > 1)
                {
                    if (overload.Value.Current < overload.Value.AnimationCount - 1)
                    {
                        e.SetAnimationOverload(overload.Value.PlayNext());
                    }
                    else
                    {
                        e.SendMessage<AnimationCompleteMessage>();
                    }
                }
                else if (!overload.Value.Loop)
                {
                    e.RemoveAnimationOverload();
                    e.SendMessage<AnimationCompleteMessage>();
                }
                else
                {
                    e.SendMessage<AnimationCompleteMessage>();
                }
            }

            if (hook.ShowReflection && e.TryGetReflection() is ReflectionComponent reflection && !reflection.BlockReflection)
            {
                RenderServices.DrawSprite(
                    render.FloorBatch,
                    asset.Guid,
                    renderPosition + reflection.Offset,
                    new DrawInfo()
                    {
                        Origin = offset,
                        ImageFlip = flip,
                        Rotation = rotation,
                        Sort = 0,
                        Color = baseColor * reflection.Alpha,
                        Scale = scale * new Vector2(1, -1),
                    },
                    new AnimationInfo(animationId, start) with { OverrideCurrentTime = overrideCurrentTime });
            }
        }
    }

    private (string animationId, SpriteAsset? asset, float start, ImageFlip flip) GetAgentSpriteSettings(Entity e)
    {
        AgentSpriteComponent sprite = e.GetAgentSprite();
        FacingComponent facing = e.GetFacing();

        float start = NoiseHelper.Simple01(e.EntityId * 10) * 5f;
        var prefix = sprite.IdlePrefix;

        var angle = facing.Angle; // Gives us an angle from 0 to 1, with 0 being right and 0.5 being left
        (string suffix, bool flip) = DirectionHelper.GetSuffixFromAngle(e, sprite, angle);

        SpriteAsset? SpriteAsset = Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid);

        return (prefix + suffix, SpriteAsset, start, flip ? ImageFlip.Horizontal : ImageFlip.None);
    }
}