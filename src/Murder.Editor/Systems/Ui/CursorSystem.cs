using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Editor;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;

namespace Murder.Systems
{
    [EditorSystem]
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    public class CursorSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            Architect.Instance.Cursor = hook.Cursor;
        }
    }
}