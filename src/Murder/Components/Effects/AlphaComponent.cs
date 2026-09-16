using Bang.Components;
using Murder.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// Set alpha of a component being displayed in the screen.
    /// </summary>
    
    public readonly struct AlphaComponent : IComponent
    {
        [Slider(0,1)]
        public readonly float Alpha = 1f;

        public AlphaComponent() { }

        public AlphaComponent(float amount)
        {
            Alpha = amount;
        }
    }
}