using Bang;
using Bang.Entities;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using System.Numerics;

namespace Murder.Services;

public static class DebugServices
{
    public static void DrawText(World world, string ev, Vector2 position, float duration)
    {
        var e = world.AddEntity();
        float time = Game.NowUnscaled;
        
        e.SetCustomDraw((render) =>
        {
            float delta = (Game.NowUnscaled - time) / duration;
            if (delta > 1)
                e.Destroy();

            RenderServices.DrawText(render.DebugBatch, MurderFonts.PixelFont, ev, position, new DrawInfo(Color.Green * (1 - delta), 0)
            {
                Shadow = Color.Black * (1 - delta),
                Outline = Color.Black * (1 - delta)
            });
        });
        
    }

    public static void DrawRect(World world, Rectangle rect, float duration)
    {
        var e = world.AddEntity();
        float time = Game.NowUnscaled;

        e.SetCustomDraw((render) =>
        {
            float delta = (Game.NowUnscaled - time) / duration;
            if (delta > 1)
                e.Destroy();

            RenderServices.DrawRectangleOutline(render.DebugBatch, rect, Color.Green * (1 - delta), 1, 0);
        });

    }
}
