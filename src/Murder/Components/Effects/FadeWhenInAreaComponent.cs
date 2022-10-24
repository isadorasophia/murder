using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// For now, this is only supported for aseprite components.
    /// </summary>
    [Requires(typeof(ColliderComponent))]
    internal readonly struct FadeWhenInAreaComponent : IComponent
    {
        public readonly float Duration = 0;

        public FadeWhenInAreaComponent() { }
    }
}
