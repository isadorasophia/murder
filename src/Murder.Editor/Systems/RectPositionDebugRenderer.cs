using Bang.Contexts;
using Bang.Systems;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Editor.Components;
using Murder.Services;

namespace Murder.Editor.Systems
{
    [EditorSystem]
    [Filter(typeof(RectPositionComponent), typeof(DebugColorComponent))]
    internal class RectPositionDebugRenderer : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                var rect = e.GetComponent<RectPositionComponent>();
                var color = e.GetComponent<DebugColorComponent>().Color;
                var colorFaded = color * 0.5f;

                IntRectangle box = rect.GetBox(e, render.Viewport.NativeResolution);

                RenderServices.DrawHorizontalLine(render.UiBatch, 0, box.Top, render.Viewport.NativeResolution.X, colorFaded, 1);
                RenderServices.DrawHorizontalLine(render.UiBatch, 0, box.Bottom, render.Viewport.NativeResolution.X, colorFaded, 1);
                RenderServices.DrawVerticalLine(render.UiBatch, box.Left, 0, render.Viewport.NativeResolution.Y, colorFaded, 1);
                RenderServices.DrawVerticalLine(render.UiBatch, box.Right, 0, render.Viewport.NativeResolution.Y, colorFaded, 1);
                RenderServices.DrawRectangleOutline(
                    render.UiBatch,
                    box,
                    color, 2, .99f);
            }
        }
    }
}