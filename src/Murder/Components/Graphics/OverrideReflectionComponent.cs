using Bang.Components;
using Murder.Attributes;
using System.Numerics;

namespace Murder.Components;

/// <summary>
/// Override (usually temporarily) the value for ReflectionComponent.
/// </summary>
[HideInEditor]
public readonly struct OverrideReflectionComponent : IComponent
{
    public readonly Vector2 Offset = Vector2.Zero;

    public OverrideReflectionComponent()
    {
    }

    public OverrideReflectionComponent(Vector2 offset)
    {
        Offset = offset;
    }
}
