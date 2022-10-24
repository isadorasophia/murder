using Bang.Components;

namespace Murder.Components
{
    public struct OwnerComponent : IComponent
    {
        public int Owner;
        public OwnerComponent(int owner)
        {
            Owner = owner;
        }
    }
}
