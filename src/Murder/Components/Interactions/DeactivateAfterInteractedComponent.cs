using Bang.Components;

namespace Murder.Components;

/// <summary>
/// Currently used with <see cref="InteractOnCollisionComponent"/> that is only set once.
/// This will deactivate the entity instead of removing the component.
/// </summary>
public readonly struct DeactivateAfterInteractedComponent : IComponent
{
}