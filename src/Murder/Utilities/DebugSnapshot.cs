using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Utilities;

public class DebugSnapshot
{
    protected static DebugSnapshot? _instance;

    /// <summary>
    /// This is a singleton.
    /// </summary>
    protected DebugSnapshot() { }

    [MemberNotNull(nameof(_instance))]
    public static DebugSnapshot GetOrCreateInstance()
    {
        _instance ??= new DebugSnapshot();
        return _instance;
    }

    public static void TakeSnapShot(int samples, float divider = 1000f)
    {
        if (!Game.DIAGNOSTICS_MODE)
        {
            return;
        }

        GetOrCreateInstance().TakeSnapShotImpl(samples, divider);
    }

    public static void StartStopwatch(string id)
    {
        if (!Game.DIAGNOSTICS_MODE)
        {
            return;
        }

        GetOrCreateInstance().StartStopwatchImpl(id);
    }
    public static void PauseStopwatch()
    {
        if (!Game.DIAGNOSTICS_MODE)
        {
            return;
        }

        GetOrCreateInstance().PauseStopWatchImpl();
    }

    public static void EndSnapshot()
    {
        if (!Game.DIAGNOSTICS_MODE)
        {
            return;
        }

        GetOrCreateInstance().EndSnapshotImpl();
    }

    public static float GetTotalTime()
    {
        if (!Game.DIAGNOSTICS_MODE)
        {
            return 0;
        }
        
        return GetOrCreateInstance().GetTotalTimeImpl();
    }

    public static ImmutableArray<(string id, float time)> GetAllEntries()
    {
        if (!Game.DIAGNOSTICS_MODE)
        {
            return [];
        }
        
        return GetOrCreateInstance().GetAllEntriesImpl();
    }

    protected virtual void TakeSnapShotImpl(int samples, float divider) { }

    protected virtual void StartStopwatchImpl(string id) { }

    protected virtual void EndSnapshotImpl() { }

    protected virtual void PauseStopWatchImpl() { }

    protected virtual float GetTotalTimeImpl() => 0;

    protected virtual ImmutableArray<(string id, float time)> GetAllEntriesImpl() => [];
}
