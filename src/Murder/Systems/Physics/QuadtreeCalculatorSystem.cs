using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Effects;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Physics;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace Murder.Systems;

[Filter(typeof(ColliderComponent), typeof(ITransformComponent))]
[Watch(typeof(ITransformComponent), typeof(ColliderComponent))]
public class QuadtreeCalculatorSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.AddToCollisionQuadTree(entities);
    }

    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.RemoveFromCollisionQuadTree(entities);
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.RemoveFromCollisionQuadTree(entities);
        qt.AddToCollisionQuadTree(entities);
    }
    
    public void OnActivated(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.AddToCollisionQuadTree(entities);
    }
    public void OnDeactivated(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.RemoveFromCollisionQuadTree(entities);

    }
}
