using InstallWizard.Components.Editor;
using InstallWizard.Components.Graphics;
using InstallWizard.Core;
using InstallWizard.Core.Engine;
using InstallWizard.Core.Graphics;
using InstallWizard.Util;
using Bang.Contexts;
using Bang.Systems;

namespace Editor.Systems
{
    [Filter(typeof(AsepriteComponent), typeof(ShowYSortComponent))]
    internal class AsepriteRenderDebugSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                var ase = e.GetComponent<AsepriteComponent>();
                RenderServices.DrawHorizontalLine(
                    render.DebugSpriteBatch,
                    (int)render.Camera.Bounds.Left,
                    (int)(e.GetGlobalPosition().Y + ase.YSortOffset),
                    (int)render.Camera.Bounds.Width,
                    Color.BrightGray,
                    0.2f);
            }

            return default;
        }
    }
}
