using InstallWizard.Components;
using InstallWizard.Core;
using InstallWizard.Core.Engine;
using InstallWizard.Core.Graphics;
using InstallWizard.Util;
using Bang.Contexts;
using Bang.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Systems
{
    [Filter(typeof(RectPositionComponent), typeof(DebugColorComponent))]
    internal class RectPositionDebugRenderer : IMonoRenderSystem
    {
        public ValueTask Draw(RenderContext render, Context context)
        {
            foreach (var e in context.Entities)
            {
                var rect = e.GetComponent<RectPositionComponent>();
                var color = e.GetComponent<DebugColorComponent>().Color;
                var colorFaded = color.WithAlpha(0.5f);
                
                IntRectangle box = rect.GetBox(e, render.ScreenSize, render.UiReferenceScale);

                RenderServices.DrawHorizontalLine(render.UiBatch, 0, box.Top, render.ScreenSize.X, colorFaded, 1);
                RenderServices.DrawHorizontalLine(render.UiBatch, 0, box.Bottom, render.ScreenSize.X, colorFaded, 1);
                RenderServices.DrawVerticalLine(render.UiBatch, box.Left, 0, render.ScreenSize.Y, colorFaded, 1);
                RenderServices.DrawVerticalLine(render.UiBatch, box.Right, 0, render.ScreenSize.Y, colorFaded, 1);
                RenderServices.DrawRectangleOutline(
                    render.UiBatch,
                    box,
                    color,2, .99f);
            }

            return default;
        }
    }
}
