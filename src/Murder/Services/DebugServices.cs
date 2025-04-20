using Bang;
using Bang.Entities;
using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using System.Numerics;
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


    public static void DrawText(World world, string ev, Vector2 position, float duration, Color color)
    {
#if DEBUG
        var e = world.AddEntity();
        float time = Game.NowUnscaled;

        e.SetCustomDraw((render) =>
        {
            float delta = (Game.NowUnscaled - time) / duration;
            if (delta > 1)
                e.Destroy();

            RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, ev, position, new DrawInfo(color * (1 - delta), 0)
            {
                Shadow = Color.Black * (1 - delta),
                Outline = Color.Black * (1 - delta)
            });
        });
#endif
    }

    public static void DrawLine(World world, Vector2 start, Vector2 end, Color color, float duration = 1/30f)
    {
#if DEBUG
        var e = world.AddEntity();
        var time = Game.NowUnscaled;

        e.SetCustomDraw((render) =>
        {
            if (Game.NowUnscaled - time > duration)
            {
                e.Destroy();
            }
            float delta = (Game.NowUnscaled - time) / duration;
            RenderServices.DrawLine(render.DebugBatch, start, end, color * delta);
        });
#endif
    }
}