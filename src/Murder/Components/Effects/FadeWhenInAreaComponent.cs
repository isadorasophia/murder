using Bang.Components;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    /// <summary>
    /// For now, this is only supported for aseprite components.
    /// </summary>
    [Requires(typeof(ColliderComponent))]
    public readonly struct FadeWhenInAreaComponent : IComponent
    {
        public readonly float Duration = 0;

        [Target]
        public readonly ImmutableArray<string> AppliesTo = ImmutableArray<string>.Empty;
        
        public FadeWhenInAreaComponent() { }
    }
}
