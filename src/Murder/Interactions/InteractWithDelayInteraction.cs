using Bang;
using Bang.Entities;
using Bang.Interactions;
using Bang.StateMachines;
using Murder.Services;

namespace Murder.Interaction
{
    internal readonly struct InteractWithDelayInteraction : IInteraction
    {
        public readonly float Time;

        public readonly IInteraction Interactive;

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            world.RunCoroutine(WaitBeforeInteract(world, interactor, interacted, Time, Interactive));
        }

        private static IEnumerator<Wait> WaitBeforeInteract(
            World world, Entity interactor, Entity? interacted, float time, IInteraction interactive)
        {
            yield return Wait.ForSeconds(time);
            interactive.Interact(world, interactor, interacted);
        }
    }
}
