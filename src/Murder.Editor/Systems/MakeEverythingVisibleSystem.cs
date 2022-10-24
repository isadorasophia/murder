using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;

namespace Murder.Editor.Systems
{
    [Filter(typeof(RequiresVisionComponent))]
    public class MakeEverythingVisibleSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                e.AddOrReplaceComponent(new LastSeenComponent());
            }

            return default;
        }
    }
}
