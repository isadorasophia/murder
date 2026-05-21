using Bang.Components;
using Murder.Utilities.Attributes;

namespace Murder.Components.Graphics;

[RuntimeOnly]
public readonly struct SpritePausedComponent : IComponent
{
    public readonly float PausedAt;

    public SpritePausedComponent()
    {
        PausedAt = Game.Now;
    }

    public SpritePausedComponent(float pausedAt)
    {
        PausedAt = pausedAt;
    }
}
