using Bang.Contexts;
using Bang.Systems;
using Murder.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Utilities;

[Filter(ContextAccessorFilter.None)]
internal class ScreenshotShortcutListener : IMurderRenderSystem
{
    public void Draw(RenderContext render, Context context)
    {
        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F10))
        {
            Game.Input.ConsumeAll();
            render.SaveGameplayScreenshot();
        }
    }
}