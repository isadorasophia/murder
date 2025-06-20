using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Editor.Diagnostics;

public class EditorDebugSnapshot : DebugSnapshot
{
    private bool _snapShotStarted;

    private readonly Stopwatch _stopwatch = new();

    private readonly Dictionary<string, float> _recodedTimes = new();
    private ImmutableArray<(string id, float time)>? _cachedEntries;
    private float _totalTime;

    private string? _currentlyRecording = null;
    private float _divider = 1000f;

    /// <summary>
    /// This is a singleton.
    /// </summary>
    protected EditorDebugSnapshot() : base() { }

    public static EditorDebugSnapshot OverrideInstanceWithEditor()
    {
        EditorDebugSnapshot snapshot = new();
        _instance = snapshot;

        return snapshot;
    }

    protected override void TakeSnapShotImpl(float divider)
    {
        _snapShotStarted = true;
        _recodedTimes.Clear();
        _currentlyRecording = null;
        _cachedEntries = null;
        _divider = divider;
    }

    protected override void StartStopwatchImpl(string id)
    {
        if (!_snapShotStarted)
        {
            return;
        }

        if (id != null)
        {
            float time = (float)_stopwatch.Elapsed.TotalSeconds;
            _recodedTimes[_currentlyRecording ?? string.Empty] = time * _divider;
            _totalTime += time * _divider;
        }

        _currentlyRecording = id;
        _stopwatch.Restart();
    }

    protected override void EndSnapshotImpl()
    {
        if (!_snapShotStarted)
        {
            return;
        }
        _snapShotStarted = false;
        if (_currentlyRecording != null)
        {
            float time = (float)_stopwatch.Elapsed.TotalSeconds;
            _recodedTimes[_currentlyRecording] = time;
            _totalTime += time;
        }
        _cachedEntries = null;
        _stopwatch.Stop();
    }

    protected override float GetTotalTimeImpl()
    {
        return _totalTime;
    }

    protected override ImmutableArray<(string id, float time)> GetAllEntriesImpl()
    {
        if (_cachedEntries is not null)
        {
            return _cachedEntries.Value;
        }

        var builder = ImmutableArray.CreateBuilder<(string id, float time)>();
        foreach (var entry in _recodedTimes)
        {
            builder.Add((entry.Key, entry.Value));
        }

        _cachedEntries = builder.ToImmutable();
        return _cachedEntries.Value;
    }
}
