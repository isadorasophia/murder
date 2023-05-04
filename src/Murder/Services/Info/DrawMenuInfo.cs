using Murder.Core.Geometry;

namespace Murder.Services.Info
{
    public readonly struct DrawMenuInfo
    {
        public Vector2 SelectorPosition { get; init; } = Vector2.Zero;
        public Vector2 PreviousSelectorPosition { get; init; } = Vector2.Zero;
        public Vector2 SelectorEasedPosition { get; init; } = Vector2.Zero;

        public DrawMenuInfo()
        {
        }
    }
}