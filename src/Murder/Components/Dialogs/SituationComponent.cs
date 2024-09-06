using Bang.Components;
using Murder.Assets;
using Murder.Attributes;

namespace Murder.Components;

public readonly struct SituationComponent : IComponent
{
    [GameAssetId(typeof(CharacterAsset)), ShowInEditor]
    public readonly Guid Character = Guid.Empty;

    /// <summary>
    /// This is the starter situation for the interaction.
    /// </summary>
    public readonly string? Situation = string.Empty;

    public bool Empty => Character == Guid.Empty;

    public SituationComponent() { }

    public SituationComponent(Guid character, string situation)
    {
        Character = character;
        Situation = situation;
    }

    public SituationComponent WithSituation(string situation) => new(Character, situation);
}