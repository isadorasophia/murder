using Bang.Components;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

[HideInEditor]
[PersistOnSave]
public readonly struct PreviouslyPlayedCutscenesComponent : IComponent
{
    public readonly ImmutableHashSet<string> Played { get; init; } = [];

    public PreviouslyPlayedCutscenesComponent() { }
}
