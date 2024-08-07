using Bang.Contexts;
using Bang.Systems;

namespace Murder.Systems;

[Filter(ContextAccessorFilter.None)]
internal class FullscreenShortcutListener : IUpdateSystem
{
    public void Update(Context context)
    {
        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F11))
        {
            Game.Input.ConsumeAll();

            Game.Instance.Fullscreen = !Game.Instance.Fullscreen;
        }
    }
}