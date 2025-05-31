using Bang;
using Bang.Entities;
using Bang.Interactions;
using Bang.StateMachines;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Services;

namespace Murder.Interaction
{
    public readonly struct InteractWithDelayInteraction : IInteraction
    {
        public readonly float Time = 0;

        [Default("Add interaction...")]
        public readonly IInteractiveComponent? Interactive = null;

        public InteractWithDelayInteraction() { }

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            if (Interactive is null)
            {
                GameLogger.Error($"Why is InteractWithDelayInteraction with a null interaction? Skipping interactor {interactor}.");
                return;
            }

            world.RunCoroutine(WaitBeforeInteract(world, interactor, interacted, Time, Interactive));
        }

        private static IEnumerator<Wait> WaitBeforeInteract(
            World world, Entity interactor, Entity? interacted, float time, IInteractiveComponent interactive)
        {
            yield return Wait.ForSeconds(time);
            interactive.Interact(world, interactor, interacted);
        }
    }
}
