using Bang.Components;

namespace Murder.Components
{
    public readonly struct InteractOnCollisionComponent : IComponent 
    {
        public readonly bool OnlyOnce = false;
        
        public InteractOnCollisionComponent() { }
    }
}
