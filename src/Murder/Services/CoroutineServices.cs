using Bang;
using Bang.Entities;
using Bang.StateMachines;
using Murder.StateMachines;

namespace Murder.Services
{
    public static class CoroutineServices
    {
        public static void RunCoroutine(this World world, IEnumerator<Wait> routine)
        {
            // TODO: Figure out object pulling of entities here.
            Entity e = world.AddEntity(
                new StateMachineComponent<Coroutine>(new Coroutine(routine)));

            // Immediately run the first tick!
            e.GetStateMachine().Tick(Game.DeltaTime);
        }
    }
}
