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
    [Filter(ContextAccessorFilter.AllOf, typeof(AsepriteComponent), typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(ThreeSliceComponent))]
    public class AsepriteRenderSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                bool flip = false;

                IMurderTransformComponent transform = e.GetGlobalTransform();
                AsepriteComponent s = e.GetAseprite();

                if (s.AnimationStartedTime == 0)
                    continue;

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is not AsepriteAsset ase)
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
                if (s.TargetSpriteBatch!=TargetSpriteBatches.Ui && !render.Camera.Bounds.Touches(new Rectangle(renderPosition - ase.Size * s.Offset - ase.Origin, ase.Size)))
                    continue;

                // Handle rotation
                float rotation = transform.Angle;
                if (s.RotateWithFacing)
                {
                    if (e.TryGetFacing() is FacingComponent facing)
                        rotation += DirectionHelper.Angle(facing.Direction);
                }

                if (s.FlipWithFacing)
                {
                    if (e.TryGetFacing() is FacingComponent facing && facing.Direction.Flipped())
                        flip = true;
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

                Microsoft.Xna.Framework.Vector3 blend;
                // Handle flashing
                if (e.HasFlashSprite())
                {
                    blend = RenderServices.BLEND_WASH;
                }
                else
                {
                    blend = RenderServices.BLEND_NORMAL;
                }

                var ySort = RenderServices.YSort(transform.Y + s.YSortOffset);

                if (e.TryGetVerticalPosition() is VerticalPositionComponent verticalPosition)
                {
                    renderPosition = new Vector2(renderPosition.X, renderPosition.Y - verticalPosition.Z);
                }

                bool complete;
                if (e.HasHighlightSprite())
                {
                    complete = RenderServices.RenderSpriteWithOutline(
                        render.GetSpriteBatch(s.TargetSpriteBatch),
                        renderPosition,
                        s.CurrentAnimation,
                        ase,
                        s.AnimationStartedTime,
                        -1,
                        s.Offset,
                        flip,
                        rotation,
                        color,
                        blend,
                        ySort,
                        useScaledTime: !e.HasPauseAnimation());
                }
                else
                {
                    complete = RenderServices.RenderSprite(
                        render.GetSpriteBatch(s.TargetSpriteBatch),
                        renderPosition,
                        s.CurrentAnimation,
                        ase,
                        s.AnimationStartedTime,
                        -1,
                        s.Offset,
                        flip,
                        rotation,
                        Vector2.One,
                        color,
                        blend,
                        ySort,
                        useScaledTime: !e.HasPauseAnimation());
                }

                if (complete)
                    RenderServices.MessageCompleteAnimations(e, s);
            }
        }
    }
}
