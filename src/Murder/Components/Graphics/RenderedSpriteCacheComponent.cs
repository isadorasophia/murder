using Bang.Components;
using Murder.Core.Graphics;
using System.Numerics;

namespace Murder.Components.Graphics
{
    
    public readonly struct RenderedSpriteCacheComponent : IComponent
    {
        public readonly Vector2 RenderPosition { get; init; }
        public readonly Vector2 Scale { get; init; }
        public readonly Vector2 Offset { get; init; }
        public readonly float Rotation { get; init; }
        public readonly Guid RenderedSprite { get; init; }
        public readonly bool Flipped { get; init; }
        public readonly float Sorting { get; init; }
        public readonly BlendStyle Blend { get; init; }
        public readonly AnimationInfo AnimInfo { get; init; }
        public readonly OutlineStyle Outline { get; init; }
        public readonly Color Color { get; init; }
    }
}
