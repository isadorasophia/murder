using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Bang.StateMachines;
using Murder.Components;
using Murder.Diagnostics;
using Murder.StateMachines;

namespace Murder.Interactions
{
    [Requires(typeof(SituationComponent))]
    public readonly struct TalkToInteraction : IInteraction
    {
        public static readonly string DIALOGUE_CHILD = "DialogueChild";

        public TalkToInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (interacted is null)
            {
                GameLogger.Error("Error while interacting with empty entity.");
                return;
            }

            SituationComponent? situation = interacted.TryGetOverrideSituation()?.Peek ??
                interacted.TryGetSituation();

            if (situation is null)
            {
                GameLogger.Error("Interacted without a situation.");
                return;
            }

            Entity? dialogEntity = CreateDialogueChild(world, interacted);
            if (dialogEntity is null)
            {
                return;
            }

            if (interactor.TryGetSpeaker() is SpeakerComponent speaker)
            {
                dialogEntity.SetSpeaker(speaker);
            }

            if (interacted.HasAutomaticNextDialogue())
            {
                dialogEntity.SetAutomaticNextDialogue();
            }

            dialogEntity.SetSituation(situation.Value);
            dialogEntity.SetStateMachine(new StateMachineComponent<DialogStateMachine>());

            if (interacted is not null)
            {
                // Propagate target entity that has been interacted.
                dialogEntity.SetIdTarget(interacted.EntityId);
            }
        }

        public static Entity? CreateDialogueChild(World world, Entity? interacted)
        {
            if (interacted is null)
            {
                GameLogger.Error("Error while interacting with empty entity.");
                return null;
            }

            if (!interacted.HasSituation() && !interacted.HasOverrideSituation())
            {
                GameLogger.Error("Interacted without a situation.");
                return null;
            }

            Entity? childDialogue = interacted?.TryFetchChild(DIALOGUE_CHILD);
            if (childDialogue is not null && childDialogue.HasStateMachine())
            {
                // Already in progress.
                return null;
            }

            childDialogue ??= world.AddEntity();
            interacted?.AddChild(childDialogue.EntityId, DIALOGUE_CHILD);

            return childDialogue;
        }
    }
}