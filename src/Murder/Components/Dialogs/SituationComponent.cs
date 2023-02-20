using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;

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

        public SituationComponent() { }

        public SituationComponent(Guid character, int situation)
        {
            Character = character;
            Situation = situation;
        }

        public SituationComponent WithSituation(int situation) => new(Character, situation);
    }
}
