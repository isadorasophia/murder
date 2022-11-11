using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Helpers;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    [Filter(typeof(AsepriteComponent), typeof(ITransformComponent))]
    internal class AsepriteRenderDebugSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                AsepriteComponent s = e.GetAseprite();
                
                if (e.HasComponent<ShowYSortComponent>())
                {
                    RenderServices.DrawHorizontalLine(
                    render.DebugSpriteBatch,
                    (int)render.Camera.Bounds.Left,
                    (int)(e.GetGlobalTransform().Y + s.YSortOffset),
                    (int)render.Camera.Bounds.Width,
                    Color.BrightGray,
                    0.2f);
                }
                
                IMurderTransformComponent pos = e.GetGlobalTransform();
                float rotation = e.TryGetRotate()?.Rotation ?? 0;
                if (s.RotateWithFacing && e.TryGetFacing() is FacingComponent facing)
                {
                    rotation += DirectionHelper.Angle(facing.Direction);
                }

                var ySort = RenderServices.YSort(pos.Y + s.YSortOffset);

                if (Game.Data.TryGetAsset<AsepriteAsset>(s.AnimationGuid) is AsepriteAsset ase)
                {
                    bool complete = RenderServices.RenderSprite(
                        render.GetSpriteBatch(s.TargetSpriteBatch),
                        render.Camera,
                        pos.Vector2,
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
                }
            }

            return default;
        }
    }
}
