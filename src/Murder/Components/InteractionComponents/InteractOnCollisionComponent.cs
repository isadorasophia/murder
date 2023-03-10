using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct InteractOnCollisionComponent : IComponent 
    {
        public readonly bool OnlyOnce = false;
        public readonly bool SendMsgOnExit = false;

        [Tooltip("Whether only a player is able to activate this.")]
        public readonly bool PlayerOnly = false;

        public InteractOnCollisionComponent() { }
    }
}
