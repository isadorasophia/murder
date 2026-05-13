

using Bang.Components;

namespace Murder.Components.Graphics;

public readonly struct SpritePausedComponent : IComponent
{
    public readonly float PausedAt;

    public SpritePausedComponent(float pausedAt)
    {
        PausedAt = pausedAt;
    }
}
