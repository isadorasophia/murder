using Bang.Components;
using Murder.Attributes;

namespace Murder.Components.Effects
{
    /// <summary>
    /// Component that temporarily excludes this entity to exist within the world.
    /// </summary>
    [DoNotPersistOnSave]
    public readonly struct DisableEntityComponent : IComponent { }
}