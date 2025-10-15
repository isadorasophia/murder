using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// Set alpha of a component being displayed in the screen.
    /// </summary>
    
    public readonly struct AlphaComponent : IComponent
    {
        public readonly float Alpha = 1f;

        public AlphaComponent() { }

        public AlphaComponent(float amount)
        {
            Alpha = amount;
        }
    }
}