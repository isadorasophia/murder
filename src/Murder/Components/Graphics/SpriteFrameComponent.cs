using Bang.Components;

namespace Murder.Components.Graphics;

public readonly struct SpriteFrameComponent : IComponent
{
    public readonly string Animation;
    public readonly float Y;

    public readonly int Frame;

    public SpriteFrameComponent(string animation, int frame, float y)
    {
        Animation = animation;
        Frame = frame;
        Y = y;
    }
}
