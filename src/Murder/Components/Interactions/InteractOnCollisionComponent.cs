using Bang.Components;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    public readonly struct InteractOnCollisionComponent : IComponent
    {
        [Tooltip("Whether this should be activated again.")]
        public readonly bool OnlyOnce = false;

        [Tooltip("Whether this will send a message once the object stop colliding.")]
        public readonly bool SendMessageOnExit = false;

        [Tooltip("Whether this will send a message every frame while colliding.")]
        public readonly bool SendMessageOnStay = false;

        [Tooltip("Interactions that will be triggered in addition to interactions in this entity.")]
        public readonly ImmutableArray<IInteractiveComponent> CustomEnterMessages = ImmutableArray<IInteractiveComponent>.Empty;

        [Tooltip("Interactions that will be triggered in addition to interactions in this entity.")]
        public readonly ImmutableArray<IInteractiveComponent> CustomExitMessages = ImmutableArray<IInteractiveComponent>.Empty;

        [Tooltip("Whether only a player is able to activate this.")]
        public readonly bool PlayerOnly = false;

        public InteractOnCollisionComponent() { }

        public InteractOnCollisionComponent(bool playerOnly)
        {
            PlayerOnly = playerOnly;
        }

        public InteractOnCollisionComponent(bool playerOnly, bool sendMessageOnExit)
        {
            PlayerOnly = playerOnly;
            SendMessageOnExit = sendMessageOnExit;
        }
    }
}