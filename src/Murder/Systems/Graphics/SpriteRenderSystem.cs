using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core.Graphics;
using Murder.Helpers;
using Murder.Messages;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(SpriteComponent), typeof(ITransformComponent), typeof(InCameraComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(ThreeSliceComponent))]
    public class SpriteRenderSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                bool flip = false;

                IMurderTransformComponent transform = e.GetGlobalTransform();
                SpriteComponent s = e.GetSprite();

                if (s.AnimationStartedTime == 0)
                    continue;

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

                if (facing is not null)
                {
                    if (s.RotateWithFacing) rotation += DirectionHelper.Angle(facing.Value.Direction);
                    if (s.FlipWithFacing && facing.Value.Direction.Flipped()) flip = true;
                }

                // Handle color
                var tintColor = e.TryGetTint()?.TintColor ?? Color.White;
                var color = tintColor * (e.TryGetAlpha()?.Alpha ?? 1.0f);

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
                float startTime = s.AnimationStartedTime;

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

                var animInfo = new AnimationInfo()
                {
                    Name = animation,
                    Start = startTime,
                    UseScaledTime = !e.HasPauseAnimation() && !s.UseUnscaledTime,
                    Loop = overload == null || (overload.Value.AnimationCount == 1 && overload.Value.Loop)
                };

                var scale = e.TryGetScale()?.Scale ?? Vector2.One;

                var frameInfo = RenderServices.DrawSprite(
                    render.GetSpriteBatch(s.TargetSpriteBatch),
                    asset.Guid,
                    renderPosition,
                    new DrawInfo(ySort)
                    {
                        Origin = s.Offset,
                        FlippedHorizontal = flip,
                        Rotation = rotation,
                        Scale = scale,
                        Color = color,
                        BlendMode = blend,
                        Sort = ySort,
                        OutlineStyle = s.HighlightStyle,
                        Outline = e.TryGetHighlightSprite()?.Color ?? null,
                    }, animInfo);

                if (e.TryGetReflection() is ReflectionComponent reflection)
                {
                    Vector2 verticalOffset = Vector2.Zero;
                    if (verticalPosition is not null)
                    {
                        // Compensate the vertical position when drawing the reflection.
                        verticalOffset = new Vector2(0, verticalPosition.Value.Z * 2);
                    }

                    if (reflection.BlockReflection)
                    {
                        RenderServices.DrawSprite(
                            render.ReflectionAreaBatch,
                            asset.Guid,
                            renderPosition + reflection.Offset + verticalOffset,
                            new DrawInfo(ySort)
                            {
                                Origin = s.Offset,
                                FlippedHorizontal = flip,
                                Rotation = rotation,
                                Scale = scale,
                                Color = Color.Black,
                                BlendMode = blend,
                                Sort = 0,
                                OutlineStyle = s.HighlightStyle,
                                Outline = Color.Black,
                            }, animInfo);
                    }
                    else
                    {
                        RenderServices.DrawSprite(
                            render.ReflectedBatch,
                            asset.Guid,
                            renderPosition + reflection.Offset + verticalOffset,
                            new DrawInfo(ySort)
                            {
                                Origin = s.Offset,
                                FlippedHorizontal = flip,
                                Rotation = rotation,
                                Scale = scale * new Vector2(1, -1),
                                Color = color * reflection.Alpha,
                                BlendMode = blend,
                                Sort = ySort,
                            }, animInfo);
                    }
                }

                if (frameInfo.Failed)
                    continue;

                if (!frameInfo.Event.IsEmpty)
                {
                    foreach (var ev in frameInfo.Event)
                    {
                        e.SendMessage(new AnimationEventMessage(ev));
                    }
                }

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
    }
}
