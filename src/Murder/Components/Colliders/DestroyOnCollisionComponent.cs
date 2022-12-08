using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct DestroyOnCollisionComponent : IComponent
    {
        [Tooltip("Send a FatalDamageMessage instead of destroying the entity")]
        public readonly bool KillInstead;
    }
}
