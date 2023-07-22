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
using Murder.Services;
using Murder.Utilities;
using System.Diagnostics;

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

                if (Game.Data.TryGetAsset<SpriteAsset>(s.AnimationGuid) is not SpriteAsset ase)
                    continue;

                Vector2 renderPosition;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                {
                    renderPosition = transform.Vector2 + render.Camera.Position * (1 - parallax.Factor);
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

                DrawInfo.BlendStyle blend;
                // Handle flashing
                if (e.HasFlashSprite())
                {
                    blend = DrawInfo.BlendStyle.Wash;
                }
                else
                {
                    blend = DrawInfo.BlendStyle.Normal;
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
                        ase = customSprite;
                    }

                    if (ase.Animations.ContainsKey(o.CurrentAnimation))
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

                FrameInfo frameInfo;
                var animInfo = new AnimationInfo()
                {
                    Name = animation,
                    Start = startTime,
                    UseScaledTime = !e.HasPauseAnimation() && !s.UseUnscaledTime,
                    Loop = overload == null || (overload.Value.AnimationCount == 1 && overload.Value.Loop)
                };

                frameInfo = RenderServices.DrawSprite(
                    render.GetSpriteBatch(s.TargetSpriteBatch),
                    ase.Guid,
                    renderPosition,
                    new DrawInfo(ySort)
                    {
                        Origin = s.Offset,
                        FlippedHorizontal = flip,
                        Rotation = rotation,
                        Scale = Vector2.One,
                        Color = color,
                        BlendMode = blend,
                        Sort = ySort,
                        Outline = s.CanBeHighlighted ? e.TryGetHighlightSprite()?.Color : null,
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
                            ase.Guid,
                            renderPosition + reflection.Offset + verticalOffset,
                            new DrawInfo(ySort)
                            {
                                Origin = s.Offset,
                                FlippedHorizontal = flip,
                                Rotation = rotation,
                                Scale = Vector2.One,
                                Color = Color.Black,
                                BlendMode = blend,
                                Sort = 0,
                                Outline = s.CanBeHighlighted ? Color.Black : null,
                            }, animInfo);
                    }
                    else
                    {
                        RenderServices.DrawSprite(
                            render.ReflectedBatch,
                            ase.Guid,
                            renderPosition + reflection.Offset + verticalOffset,
                            new DrawInfo(ySort)
                            {
                                Origin = s.Offset,
                                FlippedHorizontal = flip,
                                Rotation = rotation,
                                Scale = new(1, -1),
                                Color = color * reflection.Alpha,
                                BlendMode = blend,
                                Sort = ySort,
                            }, animInfo);
                    }
                }
               
                if (!frameInfo.Event.IsEmpty)
                {
                    foreach (var ev in frameInfo.Event)
                    {
                        e.SendMessage(new AnimationEventMessage(ev));
                    }
                }

                if (frameInfo.Complete)
                    RenderServices.MessageCompleteAnimations(e, s);
            }
        }
    }
}
