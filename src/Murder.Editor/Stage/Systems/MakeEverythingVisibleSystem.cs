using InstallWizard.Components;
using InstallWizard.Core.Engine;
using InstallWizard.Core.Graphics;
using Bang.Contexts;
using Bang.Systems;

namespace Editor.Systems
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
