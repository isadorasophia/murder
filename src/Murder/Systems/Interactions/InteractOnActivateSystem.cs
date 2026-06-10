using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Services;
using System.Collections.Immutable;

namespace Murder.Systems;

[Filter(typeof(InteractOnActivateComponent))]
[Watch(typeof(InteractOnActivateComponent))]
internal class InteractOnActivateSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            if (e.TryGetInteractOnActivate() is InteractOnActivateComponent interactOnActivateComponent &&
                interactOnActivateComponent.Flags.HasFlag(InteractOnActivateComponent.InteractOnActivateFlags.InteractOnAdded))
            {
                e.SendInteractMessage(e);
            }
        }
    }

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

[Filter(typeof(FireOnActivatedComponent))]
[Watch(typeof(FireOnActivatedComponent))]
internal class FireOnActivateSystem : IReactiveSystem
{
    public void OnAdded(World world, ImmutableArray<Entity> entities) { }

    public void OnModified(World world, ImmutableArray<Entity> entities) { }

    public void OnRemoved(World world, ImmutableArray<Entity> entities) { }

    public void OnActivated(World world, ImmutableArray<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            FireOnActivatedComponent fireOnActivated = e.GetFireOnActivated();
            DialogueServices.Fire(world, e, e, fireOnActivated.Actions);
        }
    }
}
