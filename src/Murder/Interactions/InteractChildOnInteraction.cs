using Bang;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Messages;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Interactions
{
    public readonly struct InteractChildOnInteraction : IInteraction
    {
        [ChildId]
        [Tooltip("Children which will be displayed.")]
        public readonly ImmutableArray<string> Children = ImmutableArray<string>.Empty;

        public InteractChildOnInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            foreach (string name in Children)
            {
                Entity? e2 = interacted?.TryFetchChild(name);
                if (e2 is null)
                {
                    continue;
                }

                e2.SendMessage<InteractMessage>(new(interactor));
            }
        }
    }
}