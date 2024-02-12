using Bang.Components;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Components;

public readonly struct TweenComponent : IComponent
{
    public readonly struct Vector2FromTo
    {
        public Vector2FromTo(Vector2 from, Vector2 to)
        {
            From = from;
            To = to;
        }

        public readonly Vector2 From { get; init; }
        public readonly Vector2 To { get; init; }

        internal Vector2 Get(float progress)
        {
            return Vector2.Lerp(From, To, progress);
        }
    }

    public readonly Vector2FromTo? Position { get; init; } = null;
    public readonly Vector2FromTo? Scale { get; init; } = null;
    public readonly FloatRange Time { get; init; }
    public readonly EaseKind Ease { get; init; }
    public TweenComponent(float timeStart, float timeEnd)
    {
        Time = new FloatRange(timeStart,timeEnd);
    }
    public TweenComponent()
    {
        
    }
}
