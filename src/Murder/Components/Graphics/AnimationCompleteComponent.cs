
using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components;

/// <summary>
/// The Aseprite component in this entity completed it's animation
/// </summary>
[DoNotPersistOnSave, RuntimeOnly]
public readonly struct AnimationCompleteComponent : IComponent
{
    public AnimationCompleteComponent() { }
}