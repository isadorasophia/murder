using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// Will only be rendered if the player has vision on this
    /// </summary>
    public readonly struct RequiresVisionComponent : IComponent
    {
        public readonly bool OnlyOnce;
    }
}
