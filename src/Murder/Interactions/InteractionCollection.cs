using Bang;
using Bang.Entities;
using Bang.Interactions;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    /// <summary>
    /// This triggers a list of different interactions within this entity.
    /// </summary>
    public readonly struct InteractionCollection : Interaction
    {
        public readonly ImmutableArray<IInteractiveComponent> Interactives = ImmutableArray<IInteractiveComponent>.Empty;
        
        public InteractionCollection() { }

        public InteractionCollection(ImmutableArray<IInteractiveComponent> interactives) =>
            Interactives = interactives;

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            foreach (IInteractiveComponent interactive in Interactives)
            {
                interactive.Interact(world, interactor, interacted);
            }
        }
    }
}
