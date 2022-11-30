using Bang.Entities;
using Bang.StateMachines;
using Bang;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Bang.Interactions;
using Murder.StateMachines;
using Murder.Components;

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

        public void Interact(World world, Entity interactor, Entity __)
        {
            Entity dialogEntity = world.AddEntity();

            if (interactor?.TryGetSpeaker() is SpeakerComponent speaker)
            {
                dialogEntity.SetSpeaker(speaker);
            }

            dialogEntity.SetSituation(Character, Situation);
            dialogEntity.SetStateMachine(new StateMachineComponent<DialogStateMachine>());
        }
    }
}
