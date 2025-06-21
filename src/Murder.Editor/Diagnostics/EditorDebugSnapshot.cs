using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Editor.Diagnostics;

/// <summary>
/// Editor-side snapshot that can average several consecutive snapshots
/// (specified by <paramref name="samples"/> in <see cref="TakeSnapShotImpl"/>).
///
/// • Call <see cref="TakeSnapShotImpl"/> once per snapshot, passing the desired
///   <c>samples</c> count the first time.  
/// • Each call to <see cref="EndSnapshotImpl"/> finalises **one** snapshot.  
/// • After <c>samples</c> snapshots have been collected, <see cref="GetAllEntriesImpl"/>
///   and <see cref="GetTotalTimeImpl"/> return the averaged results.  
/// • If you query before all samples are in, you’ll still get the running average
///   of whatever has been gathered so far.
/// </summary>
public class EditorDebugSnapshot : DebugSnapshot
{
    private bool _snapShotStarted;

    private readonly Stopwatch _stopwatch = new();

    // Per-snapshot data
    private readonly Dictionary<string, float> _recodedTimes = new();

    // Aggregated data across the current batch of snapshots
    private readonly Dictionary<string, float> _aggregatedTimes = new();
    private float _aggregatedTotalTime;

    private ImmutableArray<(string id, float time)>? _cachedEntries;

    private float _totalTime;                 // Total for the *current* snapshot
    private string? _currentlyRecording;
    private float _divider = 1000f;

    private int _samplesTarget = 1;           // How many snapshots make a batch
    private int _samplesCollected = 0;        // How many snapshots we have so far

    /// <summary>This is a singleton.</summary>
    protected EditorDebugSnapshot() : base() { }

    public static EditorDebugSnapshot OverrideInstanceWithEditor()
    {
        var snapshot = new EditorDebugSnapshot();
        _instance = snapshot;
        return snapshot;
    }

    protected override void TakeSnapShotImpl(int samples, float divider)
    {
        _samplesTarget = Math.Max(1, samples);
        _aggregatedTimes.Clear();
        _aggregatedTotalTime = 0f;
        _samplesCollected = 0;

        _snapShotStarted = true;
        _recodedTimes.Clear();
        _currentlyRecording = null;
        _cachedEntries = null;

        _divider = divider;
        _totalTime = 0f;

        _stopwatch.Reset();
    }

    protected override void StartStopwatchImpl(string id)
    {
        if (!_snapShotStarted)
            return;

        RecordCurrentlyRunningSnapshot();

        _currentlyRecording = id;
        _stopwatch.Restart();
    }

    protected override void PauseStopWatchImpl()
    {
        RecordCurrentlyRunningSnapshot();

        _currentlyRecording = null;
        _stopwatch.Stop();
    }

    private void RecordCurrentlyRunningSnapshot()
    {
        float elapsed = (float)_stopwatch.Elapsed.TotalSeconds * _divider;
        if (_currentlyRecording != null)
        {
            _recodedTimes[_currentlyRecording] = elapsed;
        }
        _totalTime += elapsed;
    }

    protected override void EndSnapshotImpl()
    {
        if (!_snapShotStarted)
            return;

        RecordCurrentlyRunningSnapshot();

        // Fold this snapshot into the running totals
        foreach (var (key, value) in _recodedTimes)
        {
            if (_aggregatedTimes.TryGetValue(key, out float sum))
                _aggregatedTimes[key] = sum + value;
            else
                _aggregatedTimes.Add(key, value);
        }

        _aggregatedTotalTime += _totalTime;
        _samplesCollected = Math.Min(_samplesCollected + 1, _samplesTarget);
        if (_samplesCollected >= _samplesTarget)
        {
            _snapShotStarted = false; // Reset for the next batch
        }

        // Invalidate cache so next query is rebuilt
        _cachedEntries = null;

        // Reset per-snapshot state
        _currentlyRecording = null;
        _recodedTimes.Clear();
        _totalTime = 0f;

        _stopwatch.Stop();
    }

    protected override float GetTotalTimeImpl()
        => _samplesCollected == 0 ? 0f : _aggregatedTotalTime / _samplesCollected;

    protected override ImmutableArray<(string id, float time)> GetAllEntriesImpl()
    {
        if (_cachedEntries is not null)
            return _cachedEntries.Value;

        if (_samplesCollected == 0)
        {
            _cachedEntries = ImmutableArray<(string id, float time)>.Empty;
            return _cachedEntries.Value;
        }

        var builder = ImmutableArray.CreateBuilder<(string id, float time)>();
        float divisor = _samplesCollected;

        foreach (var (key, sum) in _aggregatedTimes)
            builder.Add((key, sum / divisor));

        _cachedEntries = builder.ToImmutable();
        return _cachedEntries.Value;
    }
}
