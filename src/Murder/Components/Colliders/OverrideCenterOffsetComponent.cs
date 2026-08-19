using Bang.Components;
using System.Numerics;

namespace Murder.Components;

/// <summary>
/// Overrides the interaction center.
/// </summary>
public readonly struct OverrideCenterOffsetComponent : IComponent
{
    public readonly Vector2 Offset = Vector2.Zero;

    public OverrideCenterOffsetComponent() { }

    public OverrideCenterOffsetComponent(Vector2 offset) => Offset = offset;
}
