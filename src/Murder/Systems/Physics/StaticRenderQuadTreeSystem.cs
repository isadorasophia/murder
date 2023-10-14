using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Effects;
using Murder.Core.Physics;
using Murder.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Physics
{
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableEntityComponent))]
    [Filter(ContextAccessorFilter.AllOf, typeof(ITransformComponent), typeof(SpriteComponent), typeof(StaticComponent))]
    [Watch(typeof(StaticComponent))]
    internal class StaticRenderQuadTreeSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            Quadtree qt = Quadtree.GetOrCreateUnique(world);
            qt.AddToStaticRenderQuadTree(entities);
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            GameLogger.Log("Quad tree modified. Careful, this could be slow.");
            Quadtree qt = Quadtree.GetOrCreateUnique(world);
            qt.RemoveFromStaticRenderQuadTree(entities);
            qt.AddToStaticRenderQuadTree(entities);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            Quadtree qt = Quadtree.GetOrCreateUnique(world);
            qt.RemoveFromStaticRenderQuadTree(entities);
        }
    }
}