using Microsoft.Xna.Framework.Graphics;
using Murder.Diagnostics;
using System.Text;

namespace Murder.Services;

public static class DebugServices
{
    public static Texture2D? DebugPreviewImage = null;

    private static DateTime _stopwatchStart;

    public static DateTime StopwatchStart()
    {
        _stopwatchStart = DateTime.Now;
        return _stopwatchStart;
    }

    public static float StopwatchStop()
    {
        float totalTime = (float)(DateTime.Now - _stopwatchStart).TotalMilliseconds;
        GameLogger.Log($"[STOPWATCH] {totalTime:0.000}");

        return totalTime;
    }

    public static Task SaveLogAsync(string fullpath)
    {
        StringBuilder content = new();
        foreach (string line in GameLogger.FetchLogs())
        {
            content.AppendLine(line);
        }

        return File.AppendAllTextAsync(fullpath, content.ToString());
    }
}