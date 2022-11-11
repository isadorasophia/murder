using Bang.Components;
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
    [Filter(ContextAccessorFilter.AllOf, typeof(AsepriteComponent), typeof(ITransformComponent), typeof(ItemHighlightedComponent))]
    public class AsepriteRenderSystem_Highlighted : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (Entity e in context.Entities)
            {
                IMurderTransformComponent transform = e.GetGlobalTransform();
                AsepriteComponent s = e.GetAseprite();
                bool flipped = false;//e.TryGetFacing()?.Flipped ?? false;
                float rotation = e.TryGetRotate()?.Rotation ?? 0;

                var ySort = RenderServices.YSort(transform.Y + s.YSortOffset);

                if (!render.Camera.SafeBounds.Contains(transform.Vector2))
                {
                    continue;
                }

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is AsepriteAsset ase)
                {
                    bool complete = RenderServices.RenderSpriteWithOutline(
                        render.GetSpriteBatch(s.TargetSpriteBatch),
                        transform.ToVector2(),
                        s.AnimationId,
                        ase,
                        s.AnimationStartedTime,
                        -1,
                        s.Offset,
                        flipped,
                        rotation,
                        Color.White,
                        ySort);

                    RenderServices.MessageCompleteAnimations(e, s, complete);
                }
            }

            return default;
        }
    }

}
