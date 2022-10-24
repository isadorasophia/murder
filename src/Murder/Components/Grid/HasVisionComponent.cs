using Bang.Components;

namespace Murder.Components
{
    public readonly struct HasVisionComponent : IComponent
    {
        public readonly int Range = 10;
        
        public HasVisionComponent() { }
    }
}
