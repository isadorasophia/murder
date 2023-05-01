using Bang.Components;
using Murder.Assets;
using Murder.Attributes;

namespace Murder.Components
{
    public readonly struct SituationComponent : IComponent
    {
        [GameAssetId(typeof(CharacterAsset)), ShowInEditor]
        public readonly Guid Character = Guid.Empty;

        /// <summary>
        /// This is the starter situation for the interaction.
        /// </summary>
        public readonly int Situation = 0;

        [Tooltip("Optional name that sent this situation."), ShowInEditor]
        public readonly string Sender = string.Empty;

        public SituationComponent() { }

        public SituationComponent(Guid character, int situation)
        {
            Character = character;
            Situation = situation;
        }

        public SituationComponent WithSituation(int situation) => new(Character, situation);
    }
}
