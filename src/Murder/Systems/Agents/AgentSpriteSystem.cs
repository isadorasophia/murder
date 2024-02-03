using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.AllOf, typeof(ITransformComponent), typeof(AgentSpriteComponent), typeof(FacingComponent))]
    [ShowInEditor]
    public class AgentSpriteSystem : IMurderRenderSystem, IFixedUpdateSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                IMurderTransformComponent transform = e.GetGlobalTransform();
                AgentSpriteComponent sprite = e.GetAgentSprite();

                if (Game.Data.GetAsset<SpriteAsset>(sprite.AnimationGuid) is not SpriteAsset spriteAsset)
                    continue;

                VerticalPositionComponent? verticalPosition = e.TryGetVerticalPosition();

                Vector2 renderPosition;
                if (verticalPosition is not null)
                {
                    renderPosition = transform.Vector2 + new Vector2(0, -verticalPosition.Value.Z);
                }
                else
                {
                    renderPosition = transform.Vector2;
                }

                // This is as early as we can to check for out of bounds
                if (!render.Camera.Bounds.Touches(new Rectangle(renderPosition - spriteAsset.Origin, spriteAsset.Size)))
                    continue;

                FacingComponent facing = e.GetFacing();

                Vector2 impulse = Vector2.Zero;

                if (e.TryGetAgentImpulse() is AgentImpulseComponent imp) impulse = imp.Impulse;

                float start = NoiseHelper.Simple01(e.EntityId * 10) * 5f;
                var prefix = sprite.IdlePrefix;

                if (impulse.HasValue() && !e.HasDisableAgent())
                {
                    prefix = sprite.WalkPrefix;
                    SetParticleWalk(context.World, e, isWalking: true);
                }
                else
                {
                    SetParticleWalk(context.World, e, isWalking: false);
                }

                // Pause animation if this is a one-shot animation, like a spell cast.
                // For looping animations we don't need to pause
                // TODO: Check if this works for all animations
                float ySortOffsetRaw = transform.Y + sprite.YSortOffset;

                AnimationOverloadComponent? overload = null;
                if (e.TryGetAnimationOverload() is AnimationOverloadComponent o)
                {
                    overload = o;
                    prefix = $"{o.CurrentAnimation}_";

                    start = o.Start;
                    if (o.CustomSprite is SpriteAsset customSprite)
                        spriteAsset = customSprite;

                    ySortOffsetRaw += o.SortOffset;
                }

                float ySort = RenderServices.YSort(ySortOffsetRaw);

                (string suffix, bool flip) = DirectionHelper.GetSuffixFromAngle(e, sprite, facing.Angle);

                if (overload is not null && overload.Value.IgnoreFacing)
                    suffix = string.Empty;

                if (string.IsNullOrEmpty(suffix))
                    prefix = prefix.Trim('_');

                float speed = overload?.Duration ?? -1;
                AnimationSpeedOverload? speedOverload = e.TryGetAnimationSpeedOverload();

                if (speedOverload is not null)
                {
                    if (speed > 0)
                        speed *= speedOverload.Value.Rate;
                    else
                    {
                        if (spriteAsset.Animations.TryGetValue(prefix + suffix, out var animation))
                        {
                            speed = animation.AnimationDuration / speedOverload.Value.Rate;
                        }
                    }
                }

                // Handle flashing
                BlendStyle blend;
                if (e.HasFlashSprite())
                {
                    blend = BlendStyle.Wash;
                }
                else
                {
                    blend = BlendStyle.Normal;
                }

                // Handle alpha
                Color color;
                if (e.TryGetAlpha() is AlphaComponent alphaComponent)
                {
                    color = Color.White * alphaComponent.Alpha;
                }
                else
                {
                    color = Color.White;
                }

                // Handle scaling
                Vector2 scale = Vector2.One;
                if (e.TryGetScale() is ScaleComponent scaleComponent)
                {
                    scale = scaleComponent.Scale;
                }

                int target = sprite.TargetSpriteBatch;
                if (e.TryGetCustomTargetSpriteBatch() is CustomTargetSpriteBatchComponent renderTarget)
                    target = renderTarget.TargetBatch;


                if (impulse.HasValue() && spriteAsset.Animations.TryGetValue(prefix + sprite.WalkPrefix + suffix, out _) && !e.HasDisableAgent())
                {
                    prefix += sprite.WalkPrefix;
                }

                AnimationInfo animationInfo = new AnimationInfo()
                {
                    Name = prefix + suffix,
                    Start = start,
                    Duration = speed,
                    Loop = overload == null || (overload.Value.AtLast && overload.Value.Loop),
                    UseScaledTime = true
                };

                // Draw to the sprite batch
                FrameInfo frameInfo = RenderServices.DrawSprite(
                    render.GetBatch((int)target),
                    assetGuid: spriteAsset.Guid,
                    position: renderPosition,
                    new DrawInfo(ySort)
                    {
                        ImageFlip = flip ? ImageFlip.Horizontal : ImageFlip.None,
                        Color = color,
                        Scale = scale,
                        BlendMode = blend,
                        Sort = ySort,
                        Outline = e.TryGetHighlightSprite()?.Color,
                    }, animationInfo);

                e.SetRenderedSpriteCache(new RenderedSpriteCacheComponent() with
                {
                    RenderedSprite = spriteAsset.Guid,
                    CurrentAnimation = frameInfo.Animation,
                    RenderPosition = renderPosition,
                    ImageFlip = flip ? ImageFlip.Horizontal : ImageFlip.None,
                    Rotation = 0,
                    Scale = scale,
                    Color = color,
                    Blend = blend,
                    Outline = OutlineStyle.None,
                    AnimInfo = animationInfo,
                    Sorting = ySort,
                });

                // The animation overload is now done
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
                            e.SendAnimationCompleteMessage();
                            e.SetAnimationComplete();
                        }
                    }
                    else if (!overload.Value.Loop)
                    {
                        e.RemoveAnimationOverload();
                        e.SendAnimationCompleteMessage();
                        e.SetAnimationComplete();
                    }
                    else
                    {
                        e.SendAnimationCompleteMessage();
                        e.SetAnimationComplete();
                    }

                    if (speedOverload is not null && speedOverload.Value.Persist)
                    {
                        e.RemoveAnimationSpeedOverload();
                    }
                }
                else
                {
                    e.RemoveAnimationComplete();
                }
            }
        }
        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                if (e.TryGetRenderedSpriteCache() is not RenderedSpriteCacheComponent cache)
                    continue;

                RenderServices.TriggerEventsIfNeeded(e, cache, false);
            }
        }

        protected virtual void SetParticleWalk(World world, Entity e, bool isWalking)
        {
            Entity? walk = e.TryFetchChild("Particle");

            if (isWalking)
            {
                walk?.RemoveDisableParticleSystem();
            }
            else
            {
                walk?.SetParticleSystem();
            }
        }
    }
}