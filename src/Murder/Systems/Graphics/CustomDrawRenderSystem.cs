using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;

namespace Murder.Systems.Graphics
{
    [Filter(typeof(CustomDrawComponent))]
    public class CustomDrawRenderSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                e.GetCustomDraw().Draw(render);
            }
            return default;
        }
    }
}
