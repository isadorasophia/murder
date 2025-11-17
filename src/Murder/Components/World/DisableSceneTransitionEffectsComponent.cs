using Bang.Components;
using Murder.Helpers;
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
    public readonly Vector2? ForceCameraPosition { get; init; } = null;

    public readonly Vector2? PlayerOffsetFromCamera { get; init; } = null;

    public readonly Vector2? PlayerOffsetFromTarget { get; init; } = null;

    public readonly bool CleanUpAnyPendingOnes = false;

    public DisableSceneTransitionEffectsComponent() { }

    public DisableSceneTransitionEffectsComponent(Vector2 bounds) => ForceCameraPosition = bounds;

    public DisableSceneTransitionEffectsComponent(bool cleanUpAnyPendingOnes)
    {
        CleanUpAnyPendingOnes = cleanUpAnyPendingOnes;
    }
}