using Bang.Contexts;
using Bang.Systems;
using Microsoft.Xna.Framework.Input;
using Murder.Utilities;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.None)]
internal class FullscreenShortcutListener : IUpdateSystem
{
    public void Update(Context context)
    {
        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F11) || Game.Input.Shortcut(new Core.Input.Chord(Keys.Enter, Keys.LeftAlt)))
        {
            Game.Input.ConsumeAll();

            Game.Instance.Fullscreen = !Game.Instance.Fullscreen;
        }
    }
}