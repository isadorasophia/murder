using Bang.Components;
using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Components
{
    public readonly struct PushAwayComponent : IComponent
    {
        public readonly float Size;
        public readonly float Strength;

        public PushAwayComponent(int size, int strength) =>
            (Size, Strength) = (size, strength);

        public Rectangle GetBoundingBox(IMurderTransformComponent position) =>
            new(position.X - Size / 2, position.Y - Size / 2, Size, Size);

        public Rectangle GetBoundingBox(Vector2 position) =>
            new(position.X - Size / 2, position.Y - Size / 2, Size, Size);
    }
}