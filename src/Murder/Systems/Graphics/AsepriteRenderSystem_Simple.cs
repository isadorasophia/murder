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

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(AsepriteComponent), typeof(ITransformComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(AlphaComponent), typeof(ItemHighlightedComponent))]
    public class AsepriteRenderSystem_Simple : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                AsepriteComponent s = e.GetAseprite();
                if (s.AnimationStartedTime == 0)
                    continue;
                
                IMurderTransformComponent transform = e.GetGlobalTransform();
                float rotation = transform.Angle;
                if (s.RotateWithFacing)
                {
                    if (e.TryGetFacing() is FacingComponent facing)
                        rotation += DirectionHelper.Angle(facing.Direction);

                    if (e.TryFetchParent()?.TryGetFacing() is FacingComponent parentFacing)
                        rotation += DirectionHelper.Angle(parentFacing.Direction);
                }
                var ySort = RenderServices.YSort(transform.Y + s.YSortOffset);

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is AsepriteAsset ase)
                {
                    bool complete = RenderServices.RenderSprite(
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
                        Color.White,
                        RenderServices.BLEND_NORMAL,
                        ySort);

                    RenderServices.MessageCompleteAnimations(e, s, complete);
                }
            }

            return default;
        }
    }
}
