using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder;
using Murder.Assets.Graphics;
using Murder.Components;
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

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is AsepriteAsset ase)
                {
                    bool complete;
                    if (e.HasHighlightSprite())
                    {
                        complete = RenderServices.RenderSpriteWithOutline(
                            render.GetSpriteBatch(s.TargetSpriteBatch),
                            render.Camera,
                            transform.ToVector2(),
                            s.AnimationId,
                            ase,
                            s.AnimationStartedTime,
                            -1,
                            s.Offset,
                            false,
                            rotation,
                            color,
                            RenderServices.BLEND_NORMAL,
                            ySort);
                    }
                    else
                    {
                        complete = RenderServices.RenderSprite(
                            render.GetSpriteBatch(s.TargetSpriteBatch),
                            render.Camera,
                            transform.ToVector2(),
                            s.AnimationId,
                            ase,
                            s.AnimationStartedTime,
                            -1,
                            s.Offset,
                            false,
                            rotation,
                            color,
                            RenderServices.BLEND_NORMAL,
                            ySort);
                    }
                    RenderServices.MessageCompleteAnimations(e, s, complete);
                }
            }

            return default;
        }
    }
}
