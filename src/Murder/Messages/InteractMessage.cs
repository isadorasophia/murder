using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Messages
{
    /// <summary>
    /// Generic struct for interacting with an entity.
    /// </summary>
    [RuntimeOnly]
    public readonly struct InteractMessage : IMessage
    {
        public readonly Entity? Interactor = null;

        public InteractMessage(Entity interactor)
        {
            Interactor = interactor;
        }
    }
}