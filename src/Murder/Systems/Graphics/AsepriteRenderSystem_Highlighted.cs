using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(AsepriteComponent), typeof(PositionComponent), typeof(ItemHighlightedComponent))]
    public class AsepriteRenderSystem_Highlighted : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                // TODO: Generate extensions
                //PositionComponent pos = e.GetGlobalPosition();
                //AsepriteComponent s = e.GetAseprite();
                //bool flipped = e.TryGetFacing()?.Flipped ?? false;
                //float rotation = e.TryGetRotate()?.Rotation ?? 0;

                //var ySort = RenderServices.YSort(pos.Y + s.YSortOffset);

                //if (!render.Camera.SafeBounds.Contains(pos))
                //    continue;

                //if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is AsepriteAsset ase)
                //{
                //    bool complete = RenderServices.RenderSpriteWithOutline(
                //        render.GetSpriteBatch(s.TargetSpriteBatch),
                //        pos,
                //        s.AnimationId,
                //        ase,
                //        s.AnimationStartedTime,
                //        -1,
                //        s.Offset,
                //        flipped,
                //        rotation,
                //        Color.White,
                //        ySort);

                //    RenderServices.MessageCompleteAnimations(e, s, complete);
                //}
            }

            return default;
        }
    }

}
