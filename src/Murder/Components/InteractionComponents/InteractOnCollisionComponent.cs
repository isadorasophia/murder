using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct InteractOnCollisionComponent : IComponent 
    {
        [Tooltip("Whether this should be activated again.")]
        public readonly bool OnlyOnce = false;

        [Tooltip("Whether this will send a message once the object stop colliding.")]
        public readonly bool SendMessageOnExit = false;

        [Tooltip("Whether only a player is able to activate this.")]
        public readonly bool PlayerOnly = false;

        public InteractOnCollisionComponent() { }
    }
}
