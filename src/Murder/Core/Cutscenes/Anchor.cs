using System.Numerics;

namespace Murder.Core.Cutscenes
{
    public readonly struct Anchor
    {
        public readonly Vector2 Position = Vector2.Zero;

        public Anchor() { }

        public Anchor(Vector2 position) =>
            Position = position;

        public Anchor WithPosition(Vector2 position) =>
            new Anchor(position);
    }
}