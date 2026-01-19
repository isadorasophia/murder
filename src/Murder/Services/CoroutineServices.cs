using Bang;
using Bang.Entities;
using Bang.StateMachines;
using Murder.Core;
using Murder.Diagnostics;
using Murder.StateMachines;

namespace Murder.Services;

public enum CoroutineFlags
{
    None = 0,
    DoNotPause = 1
}

public static class CoroutineServices
{
    public static Coroutine RunCoroutine(this World world, IEnumerator<Wait> routine, CoroutineFlags flags = CoroutineFlags.None)
    {
        if (world is not MonoWorld murderWorld)
        {
            GameLogger.Warning("Unable to run coroutine on a world that is not MonoWorld.");
            return new();
        }

        return murderWorld.RunCoroutine(routine, flags);
    }

    public static Entity RunCoroutine(this Entity e, IEnumerator<Wait> routine)
    {
        e.SetStateMachine(new StateMachineComponent<CoroutineStateMachine>(new CoroutineStateMachine(routine)));

        // Immediately run the first tick!
        e.GetStateMachine().Tick(Game.DeltaTime);
        return e;
    }

    public static Coroutine FireAfter(this World world, float seconds, Action action, CoroutineFlags flags = CoroutineFlags.None)
    {
        if (seconds == 0)
        {
            // immediately trigger
            action.Invoke();
            return new();
        }

        if (world is not MonoWorld murderWorld)
        {
            GameLogger.Warning("Unable to run coroutine on a world that is not MonoWorld.");
            return new();
        }

        return murderWorld.RunCoroutine(WaitAndRun(seconds, action), flags);
    }

    private static IEnumerator<Wait> WaitAndRun(float seconds, Action action)
    {
        yield return Wait.ForSeconds(seconds);
        action.Invoke();
    }
}