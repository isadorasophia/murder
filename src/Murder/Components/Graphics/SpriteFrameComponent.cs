using Bang.Components;

namespace Murder.Components.Graphics;

public readonly struct SpriteFrameComponent : IComponent
{
    public readonly string Animation;

    public readonly int Frame;

    public SpriteFrameComponent(string animation, int frame)
    {
        Animation = animation;
        Frame = frame;
    }
}
