using Bang;
using Bang.Entities;
using Bang.StateMachines;
using Murder.StateMachines;

namespace Murder.Services;

public enum CoroutineFlags
{
    None = 0,
    DoNotPause = 1
}

public static class CoroutineServices
{
    public static void RunCoroutine(this World world, IEnumerator<Wait> routine, CoroutineFlags flags = CoroutineFlags.None)
    {
        // TODO: Figure out object pulling of entities here.
        Entity e = world.AddEntity(
            new StateMachineComponent<Coroutine>(new Coroutine(routine)));

        if (flags.HasFlag(CoroutineFlags.DoNotPause))
        {
            e.SetDoNotPause();
        }

        // Immediately run the first tick!
        e.GetStateMachine().Tick(Game.DeltaTime);
    }

    public static void RunCoroutine(this Entity e, IEnumerator<Wait> routine)
    {
        e.SetStateMachine(new StateMachineComponent<Coroutine>(new Coroutine(routine)));

        // Immediately run the first tick!
        e.GetStateMachine().Tick(Game.DeltaTime);
    }
}