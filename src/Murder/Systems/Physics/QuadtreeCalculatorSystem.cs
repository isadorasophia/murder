using Bang.Components;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Diagnostics;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.AnyOf, typeof(ColliderComponent), typeof(PushAwayComponent))]
    [Filter(typeof(ITransformComponent))]
    public class QuadtreeCalculatorSystem : IFixedUpdateSystem, IMonoRenderSystem, IStartupSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;

            // TODO: Move this to a debug system.
            //EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            //if (hook.DrawQuadTree == EditorHook.ShowQuadTree.Collision)
            //{
            //    qt.Collision?.DrawDebug(render.DebugSpriteBatch);
            //}

            //if (hook.DrawQuadTree == EditorHook.ShowQuadTree.PushAway)
            //{
            //    qt.PushAway?.DrawDebug(render.DebugSpriteBatch);
            //}

            return default;
        }

        public ValueTask FixedUpdate(Context context)
        {
            Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;
            qt.UpdateQuadTree(context.Entities);
            
            return default;
        }

        public ValueTask Start(Context context)
        {
            Rectangle quadTreeSize;
            if (context.World.TryGetUnique<MapComponent>() is MapComponent map)
            {
                quadTreeSize = new Rectangle(0, 0, map.Width * Grid.CellSize, map.Height * Grid.CellSize);
            }
            else
            {
                GameLogger.Warning("No size for the map was found!");

                // TODO: We need to have the city size too!
                quadTreeSize = new Rectangle(0, 0, 2000, 2000);
            }

            context.World.AddEntity(new QuadtreeComponent(quadTreeSize));
            return default;
        }
    }
}
