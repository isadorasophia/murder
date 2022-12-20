using Bang.Contexts;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.Systems.Debug
{
    public class QuadTreeDebugSystem : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
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

            return default;
        }
    }
}
