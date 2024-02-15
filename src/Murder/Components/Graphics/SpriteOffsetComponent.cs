using Bang.Components;
using System.Numerics;

namespace Murder.Components;

public readonly struct SpriteOffsetComponent : IComponent
{
    public readonly Vector2 Offset;

    public SpriteOffsetComponent(float x, float y) : this(new Vector2(x, y)) { }
    public SpriteOffsetComponent(Vector2 offset)
    {
        Offset = offset;
    }
}
