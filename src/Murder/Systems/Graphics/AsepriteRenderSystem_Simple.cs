using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Systems.Graphics
{
    [Filter(ContextAccessorFilter.AllOf, typeof(AsepriteComponent), typeof(PositionComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(AlphaComponent), typeof(ItemHighlightedComponent))]
    public class AsepriteRenderSystem_Simple : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                PositionComponent pos = e.GetGlobalPosition();
                AsepriteComponent s = e.GetAseprite();
                bool flipped = false;// e.TryGetFacing()?.Flipped ?? false;
                float rotation = e.TryGetRotate()?.Rotation ?? 0;
                var ySort = RenderServices.YSort(pos.Y + s.YSortOffset);

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is AsepriteAsset ase)
                {
                    bool complete = RenderServices.RenderSprite(
                        render.GetSpriteBatch(s.TargetSpriteBatch),
                        render.Camera,
                        pos,
                        s.AnimationId,
                        ase,
                        s.AnimationStartedTime,
                        -1,
                        s.Offset,
                        flipped,
                        rotation,
                        Color.White,
                        RenderServices.BlendNormal,
                        ySort);

                    RenderServices.MessageCompleteAnimations(e, s, complete);
                }
            }

            return default;
        }
    }
}
