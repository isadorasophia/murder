using Bang.Entities;
using Bang;
using Bang.Interactions;
using System.Collections.Immutable;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using Murder.Messages;

namespace Murder.Interactions
{
    public class InteractChildOnInteraction : Interaction
    {
        [ChildId]
        [Tooltip("Children which will be displayed.")]
        public readonly ImmutableArray<string> Children = ImmutableArray<string>.Empty;

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
