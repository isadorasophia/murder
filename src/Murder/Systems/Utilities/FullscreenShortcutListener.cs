using Bang.Contexts;
using Bang.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Murder.Systems;

internal class FullscreenShortcutListener : IUpdateSystem
{
    public void Update(Context context)
    {
        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F12))
        {
            Game.Input.ConsumeAll();

            Game.Instance.Fullscreen = !Game.Instance.Fullscreen;
        }
    }
}
