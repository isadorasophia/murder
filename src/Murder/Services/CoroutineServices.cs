using Bang;
using Bang.StateMachines;
using Murder.StateMachines;

namespace Murder.Services
{
    public static class CoroutineServices
    {
        public static void RunCoroutine(this World world, IEnumerator<Wait> routine)
        {
            // TODO: Figure out object pulling of entities here.
            world.AddEntity(
                new StateMachineComponent<Coroutine>(new Coroutine(routine)));
        }
    }
}
