using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;

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

                // This is as early as we can to check for out of bounds
                if (s.TargetSpriteBatch != TargetSpriteBatches.Ui && 
                    !render.Camera.Bounds.TouchesWithMaxRotationCheck(renderPosition - ase.Origin, ase.Size, s.Offset))
                {
                    continue;
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

                float ySort = e.HasUiDisplay() ? e.GetUiDisplay().YSort : RenderServices.YSort(transform.Y + s.YSortOffset);

                if (e.TryGetVerticalPosition() is VerticalPositionComponent verticalPosition)
                {
                    renderPosition = new Vector2(renderPosition.X, renderPosition.Y - verticalPosition.Z);
                }

                bool complete;

                complete = RenderServices.DrawSprite(
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
                    },
                    new AnimationInfo()
                    {
                        Name = s.CurrentAnimation,
                        Start = s.AnimationStartedTime,
                        UseScaledTime = !e.HasPauseAnimation()
                    });
               
                if (complete)
                    RenderServices.MessageCompleteAnimations(e, s);
            }
        }
    }
}
