using Bang.Entities;
using Bang.StateMachines;
using Bang;
using Murder.Attributes;
using Bang.Interactions;
using Murder.StateMachines;
using Murder.Components;
using Murder.Assets;
using Murder.Diagnostics;
using Murder.Services;
using Bang.Components;

namespace Murder.Interactions
{
    [Requires(typeof(SituationComponent))]
    public readonly struct TalkToInteraction : IInteraction
    {
        public TalkToInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted is null)
            {
                GameLogger.Error("Error while interacting with empty entity.");
                return;
            }

            if (interacted.TryGetSituation() is not SituationComponent situation)
            {
                GameLogger.Error("Interacted without a situation.");
                return;
            }

            Entity dialogEntity = world.AddEntity();

            if (interactor?.TryGetSpeaker() is SpeakerComponent speaker)
            {
                dialogEntity.SetSpeaker(speaker);
            }

            dialogEntity.SetSituation(situation);
            dialogEntity.SetStateMachine(new StateMachineComponent<DialogStateMachine>());

            if (interacted is not null)
            {
                // Propagate target entity that has been interacted.
                dialogEntity.SetIdTarget(interacted.EntityId);
            }
        }
    }
}
