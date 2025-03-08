using Bang.Contexts;
using Bang.Systems;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.None)]
internal class FullscreenShortcutListener : IUpdateSystem
{
    public void Update(Context context)
    {
        if (Game.Input.Shortcut(XnaKeys.F11) || 
            Game.Input.Shortcut(new Core.Input.Chord(XnaKeys.Enter, XnaKeys.LeftAlt)))
        {
            Game.Input.ConsumeAll();

            Game.Instance.Fullscreen = !Game.Instance.Fullscreen;
        }
    }
}