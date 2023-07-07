using Bang;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.Utilities
{
    internal class DebugGraphics
    {
        public static List<(object shape, float startTime, float destroyTime)> _shapes = new();
        private readonly static Color[] _colors = {
            new Color(1, 0, 0),   // Red
            new Color(0, 1, 0),   // Green
            new Color(0, 0, 1),   // Blue
            new Color(1, 1, 0),   // Yellow
            new Color(1, 0, 1),   // Magenta
            new Color(0, 1, 1),   // Cyan
            new Color(1, 0.5f, 0), // Orange
            new Color(0.5f, 0, 0.5f) // Purple
        };

        public static DebugGraphics Current = null!;
        public World World = null!;

        public static void DrawLine(Line2 line, float duration)
        {
            CreateDebuggEntityIfNescessary();
            _shapes.Add(new(line, Game.NowUnscaled, Game.NowUnscaled + duration));
        }

        public static void DrawPoint(Point point, float duration)
        {
            CreateDebuggEntityIfNescessary();
            _shapes.Add(new(point, Game.NowUnscaled, Game.NowUnscaled + duration));
        }

        private static void CreateDebuggEntityIfNescessary()
        {
            if (Current == null || Current.World != Game.Instance.ActiveScene!.World)
            {
                var world = Game.Instance.ActiveScene!.World!;
                world.AddEntity(new CustomDrawComponent(DrawLine));
                _shapes.Clear();
                
                Current = new DebugGraphics();
                Current.World = world;
            }
        }

        private static void DrawLine(RenderContext render)
        {
            for (int i = _shapes.Count - 1; i >= 0; i--)
            {
                var item = _shapes[i];
                var duration = item.destroyTime - item.startTime;
                var delta = (Game.NowUnscaled - item.startTime)/duration;
                if (item.destroyTime < Game.NowUnscaled)
                {
                    _shapes.RemoveAt(i);
                    continue;
                }
                var color = _colors[Calculator.WrapAround(i,0,_colors.Length - 1)] * Calculator.Clamp01(1 - delta);
                switch (item.shape)
                {
                    case Line2 line:
                        RenderServices.DrawLine(render.DebugSpriteBatch, line.Start, line.End, color, 0.003f);
                        break;
                    case Point point:
                        RenderServices.DrawLine(render.DebugSpriteBatch, point + new Point(0,2) , point + new Point(0, -3), color, 0.003f);
                        RenderServices.DrawLine(render.DebugSpriteBatch, point + new Point(3,0), point + new Point(-2,0), color, 0.003f);
                        RenderServices.DrawPoint(render.DebugSpriteBatch, point, color, 0.001f);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
