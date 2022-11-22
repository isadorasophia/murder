using Bang.Components;
using Bang.Entities;

namespace Murder.Messages
{
    /// <summary>
    /// Generic struct for interacting with an entity.
    /// </summary>
    public readonly struct InteractMessage : IMessage
    {
        public readonly Entity Interactor;

        public InteractMessage(Entity interactor)
        {
            Interactor = interactor;
        }
    }
}
