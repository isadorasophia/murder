using Bang.Components;
using Murder.Attributes;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components.Graphics
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct RenderedSpriteCacheComponent : IComponent, IDoNotCheckOnReplaceTag
    {
        public readonly Animation CurrentAnimation { get; init; }
        public readonly Vector2 RenderPosition { get; init; }
        public readonly Vector2 Scale { get; init; }
        public readonly Vector2 Offset { get; init; }
        public readonly float Rotation { get; init; }
        public readonly Guid RenderedSprite { get; init; }
        public readonly ImageFlip ImageFlip { get; init; }
        public readonly float Sorting { get; init; }
        public readonly BlendStyle Blend { get; init; }
        public readonly AnimationInfo AnimInfo { get; init; }
        public readonly OutlineStyle Outline { get; init; }
        public readonly Color Color { get; init; }
        /// <summary>
        /// The last recorded animation frame. Uses the internal animation frame, not the generic frame index.
        /// </summary>
        public readonly int LastFrameIndex { get; init; }
    }
}
