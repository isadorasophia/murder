using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Services;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

/// <summary>
/// This is a component with a collection of entities tracked in the world.
/// </summary>
[RuntimeOnly]
[PersistOnSave]
[KeepOnReplace]
public readonly struct IdTargetCollectionComponent : IComponent
{
    public readonly ImmutableDictionary<string, int> Targets;
    public IdTargetCollectionComponent(ImmutableDictionary<string, int> targets)
    {
        Targets = targets;
    }

    public IdTargetCollectionComponent(string id, int target)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, int>(StringComparer.InvariantCultureIgnoreCase);
        builder.Add(id, target);
        Targets = builder.ToImmutable();
    }

    public IdTargetCollectionComponent(string id1, int target1, string id2, int target2)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, int>(StringComparer.InvariantCultureIgnoreCase);
        builder.Add(id1, target1);
        builder.Add(id2, target2);
        Targets = builder.ToImmutable();
    }

}