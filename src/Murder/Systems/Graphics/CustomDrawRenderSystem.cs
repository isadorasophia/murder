using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(CustomDrawComponent))]
    public class CustomDrawRenderSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                e.GetCustomDraw().Draw(render);
            }
        }
    }
}
