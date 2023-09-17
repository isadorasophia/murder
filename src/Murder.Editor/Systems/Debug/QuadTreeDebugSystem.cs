using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;

namespace Murder.Editor.Systems.Debug
{
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    public class QuadTreeDebugSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;

            // TODO: Move this to a debug system.
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            if (hook.DrawQuadTree == EditorHook.ShowQuadTree.Collision)
            {
                qt.Collision?.DrawDebug(render.DebugSpriteBatch);
            }

            if (hook.DrawQuadTree == EditorHook.ShowQuadTree.PushAway)
            {
                qt.PushAway?.DrawDebug(render.DebugSpriteBatch);
            }

            if (hook.DrawQuadTree == EditorHook.ShowQuadTree.Render)
            {
                qt.StaticRender?.DrawDebug(render.DebugSpriteBatch);
            }
        }
    }
}
