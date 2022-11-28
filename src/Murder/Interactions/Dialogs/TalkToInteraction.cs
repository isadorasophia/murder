using Bang.Entities;
using Bang.StateMachines;
using Bang;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Bang.Interactions;
using Murder.StateMachines;

namespace Murder.Interactions
{
    public readonly struct TalkToInteraction : Interaction
    {
        [GameAssetId(typeof(CharacterAsset)), ShowInEditor]
        public readonly Guid Character = Guid.Empty;

        /// <summary>
        /// This is the starter situation for the interaction.
        /// </summary>
        public readonly int Situation = 0;

        public TalkToInteraction() { }

        public void Interact(World world, Entity _, Entity __)
        {
            Entity dialogEntity = world.AddEntity();

            dialogEntity.SetSituation(Character, Situation);
            dialogEntity.SetStateMachine(new StateMachineComponent<DialogStateMachine>());
        }
    }
}
