using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.EditorCore;
using Murder.Editor.Utilities;

namespace Murder.Systems
{
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    public class CursorSystem : IMonoRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            RenderCursor(hook.Cursor);
        }

        private void RenderCursor(CursorStyle style)
        {
            Architect.EditorData.CursorTextureManager?.RenderCursor(style);
        }
    }
}
