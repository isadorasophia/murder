using Bang.Components;
using Murder.Attributes;
using System.Numerics;

namespace Murder.Components.Graphics;

public readonly struct ScaleComponent : IComponent
{
    public readonly Vector2 Scale;
    public ScaleComponent(Vector2 scale)
    {
        Scale = scale;
    }

    public ScaleComponent(float scaleX, float scaleY)
    {
        Scale = new Vector2(scaleX, scaleY);
    }
}
