using Bang.Components;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components;

/// <summary>
/// Component that will disable any transition effects between scenes.
/// </summary>
[RuntimeOnly]
[Unique]
public readonly struct DisableSceneTransitionEffectsComponent : IComponent
{
    public readonly Vector2? OverrideCameraPosition = null;

    public DisableSceneTransitionEffectsComponent() { }

    public DisableSceneTransitionEffectsComponent(Vector2 bounds) => OverrideCameraPosition = bounds;
}