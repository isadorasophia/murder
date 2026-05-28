using Bang;
using Bang.Entities;
using Bang.StateMachines;
using Murder.Core;
using Murder.Core.Dialogs;
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

    public static Coroutine FireNextFrame(this World world, Action action, CoroutineFlags flags = CoroutineFlags.None) => 
        FireAfterFrames(world, 1, action, flags);

    public static Coroutine FireAfterFrames(this World world, int frames, Action action, CoroutineFlags flags = CoroutineFlags.None)
    {
        if (frames == 0)
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

        return murderWorld.RunCoroutine(WaitFramesAndRun(frames, action), flags);
    }

    public static Coroutine FireForDuration(this World world, float duration, Action<float> action, CoroutineFlags flags = CoroutineFlags.None)
    {
        if (duration == 0)
        {
            GameLogger.Warning($"{nameof(RunForDuration)} with a duration of 0?");
            return new();
        }

        if (world is not MonoWorld murderWorld)
        {
            GameLogger.Warning("Unable to run coroutine on a world that is not MonoWorld.");
            return new();
        }

        return murderWorld.RunCoroutine(RunForDuration(duration, action), flags);
    }

    private static IEnumerator<Wait> WaitAndRun(float seconds, Action action)
    {
        yield return Wait.ForSeconds(seconds);
        action.Invoke();
    }

    private static IEnumerator<Wait> WaitFramesAndRun(int frames, Action action)
    {
        yield return Wait.ForFrames(frames);
        action.Invoke();
    }

    private static IEnumerator<Wait> RunForDuration(float duration, Action<float> action)
    {
        float startedAt = Game.Now;

        while (true)
        {
            float elapsed = Math.Clamp(Game.Now - startedAt, 0, duration);
            action(elapsed);

            if (elapsed >= duration)
            {
                break;
            }

            yield return Wait.NextFrame;
        }
    }
}