using Bang.Components;
using Murder.Attributes;
using System.Collections.Immutable;

namespace Murder.Components;

[DoNotPersistOnSave]
public readonly struct ForceAnimationOnChanceComponent : IComponent
{
    public readonly ImmutableHashSet<string> Animations = [];
    public readonly float Chance = 1;
    public readonly Guid Sprite = Guid.Empty;

    public readonly bool Active { get; init; } = false;

    public ForceAnimationOnChanceComponent(ImmutableHashSet<string> animations, float chance, Guid sprite)
    {
        Animations = animations;
        Chance = chance;
        Sprite = sprite;
    }
}
