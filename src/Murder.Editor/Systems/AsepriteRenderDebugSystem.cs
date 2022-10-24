using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Services;

namespace Murder.Editor.Systems
{
    [Filter(typeof(AsepriteComponent), typeof(ShowYSortComponent))]
    internal class AsepriteRenderDebugSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                // TODO: Generate extensions
                //var ase = e.GetComponent<AsepriteComponent>();
                //RenderServices.DrawHorizontalLine(
                //    render.DebugSpriteBatch,
                //    (int)render.Camera.Bounds.Left,
                //    (int)(e.GetGlobalPosition().Y + ase.YSortOffset),
                //    (int)render.Camera.Bounds.Width,
                //    Color.BrightGray,
                //    0.2f);
            }

            return default;
        }
    }
}
