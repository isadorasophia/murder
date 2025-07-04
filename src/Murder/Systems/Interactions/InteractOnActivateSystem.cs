using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using System.Collections.Immutable;

namespace Murder.Systems;

[Filter(typeof(InteractOnActivateComponent))]
[Watch(typeof(InteractOnActivateComponent))]
internal class InteractOnActivateSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities) { }

    public void OnModified(World world, ImmutableArray<Entity> entities) { }

    public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

    public void OnActivated(World world, ImmutableArray<Entity> entities) 
    {
        foreach (Entity e in entities)
        {
            e.SendInteractMessage(e);
        }
    }
}
