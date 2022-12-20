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
    public class QuadtreeCalculatorSystem : IFixedUpdateSystem, IStartupSystem
    {
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
