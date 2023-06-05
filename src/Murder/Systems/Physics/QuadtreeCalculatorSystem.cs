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

[Filter(ContextAccessorFilter.AnyOf, typeof(ColliderComponent), typeof(PushAwayComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(DisableEntityComponent), typeof(StaticComponent))]
[Filter(typeof(ITransformComponent))]
public class QuadtreeCalculatorSystem : IFixedUpdateSystem, IStartupSystem
{
    public void Start(Context context)
    {
        Quadtree.GetOrCreateUnique(context.World);
    }

    public void FixedUpdate(Context context)
    {
        Quadtree qt = context.World.GetUnique<QuadtreeComponent>().Quadtree;
        qt.UpdateQuadTree(context.Entities);
    }
}

[Filter(ContextAccessorFilter.AllOf, typeof(ITransformComponent), typeof(ColliderComponent), typeof(StaticComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(DisableEntityComponent))]
[Watch(typeof(StaticComponent))]
public class StaticQuadtreeCalculatorSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.UpdateStaticQuadTree(entities);
    }

public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.UpdateStaticQuadTree(entities);
    }

    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        var qt = Quadtree.GetOrCreateUnique(world);
        qt.UpdateStaticQuadTree(entities);
    }
}
