using Bang.Entities;
using Bang.StateMachines;
using Bang;
using Murder.Attributes;
using Bang.Interactions;
using Murder.StateMachines;
using Murder.Components;
using Murder.Assets;

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

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            Entity dialogEntity = world.AddEntity();

            if (interactor?.TryGetSpeaker() is SpeakerComponent speaker)
            {
                dialogEntity.SetSpeaker(speaker);
            }

            dialogEntity.SetSituation(Character, Situation);
            dialogEntity.SetStateMachine(new StateMachineComponent<DialogStateMachine>());

            if (interacted is not null)
            {
                // Propagate target entity that has been interacted.
                dialogEntity.SetIdTarget(interacted.EntityId);
            }
        }
    }
}
