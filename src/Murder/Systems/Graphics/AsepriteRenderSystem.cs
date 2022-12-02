using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(AsepriteComponent), typeof(ITransformComponent))]
    public class AsepriteRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                bool flip = false;
                AsepriteComponent s = e.GetAseprite();
                if (s.AnimationStartedTime == 0)
                    continue;
                
                // Handle rotation
                IMurderTransformComponent transform = e.GetGlobalTransform();
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
                    color = Color.White.WithAlpha(alphaComponent.Alpha);
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

                Vector2 renderPosition;
                if (e.TryGetParallax() is ParallaxComponent parallax)
                {
                    renderPosition = transform.Vector2 + render.Camera.Position * (1 - parallax.Factor);
                }
                else
                {
                    renderPosition = transform.Vector2;
                }

                if (e.TryGetVerticalPosition() is VerticalPositionComponent verticalPosition)
                {
                    renderPosition = new Vector2(renderPosition.X, renderPosition.Y - verticalPosition.Z);
                }

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is AsepriteAsset ase)
                {
                    bool complete;
                    if (e.HasHighlightSprite())
                    {
                        complete = RenderServices.RenderSpriteWithOutline(
                            render.GetSpriteBatch(s.TargetSpriteBatch),
                            render.Camera,
                            renderPosition,
                            s.AnimationId,
                            ase,
                            s.AnimationStartedTime,
                            -1,
                            s.Offset,
                            flip,
                            rotation,
                            color,
                            blend,
                            ySort);
                    }
                    else
                    {
                        complete = RenderServices.RenderSprite(
                            render.GetSpriteBatch(s.TargetSpriteBatch),
                            render.Camera,
                            renderPosition,
                            s.AnimationId,
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
                            useScaledTime: e.HasPauseAnimation());
                    }

                    if (complete)
                        RenderServices.MessageCompleteAnimations(e, s);
                }
            }

            return default;
        }
    }
}
