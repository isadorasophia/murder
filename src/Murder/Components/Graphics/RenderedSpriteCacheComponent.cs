using Bang.Components;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Numerics;

namespace Murder.Components.Graphics
{
    [RuntimeOnly]
    [DoNotPersistOnSave]
    public readonly struct RenderedSpriteCacheComponent : IComponent, IModifiableComponent, IDoNotCheckOnReplaceTag
    {
        public readonly RenderedSpriteReference Ref = new();

        public Animation CurrentAnimation => Ref.Cache.CurrentAnimation;
        public Vector2 RenderPosition => Ref.Cache.RenderPosition;
        public Vector2 Scale => Ref.Cache.Scale;
        public Vector2 Offset => Ref.Cache.Offset;
        public float Rotation => Ref.Cache.Rotation;
        public Guid RenderedSprite => Ref.Cache.RenderedSprite;
        public ImageFlip ImageFlip => Ref.Cache.ImageFlip;
        public float Sorting => Ref.Cache.Sorting;
        public BlendStyle Blend => Ref.Cache.Blend;
        public AnimationInfo AnimInfo => Ref.Cache.AnimInfo;
        public OutlineStyle Outline => Ref.Cache.Outline;
        public Color Color => Ref.Cache.Color;

        /// <summary>
        /// The last recorded animation frame. Uses the internal animation frame, not the generic frame index.
        /// </summary>
        public int LastFrameIndex => Ref.Cache.LastFrameIndex;

        public Point SpriteSize => Ref.Cache.SpriteSize;

        public RenderedSpriteCacheComponent() { }

        public RenderedSpriteCacheComponent(RenderedSpriteCache cache) 
        {
            Ref = new() { Cache = cache };
        }

        public void Subscribe(Action notification) { } // don't support

        public void Unsubscribe(Action notification) { } // don't support
    }

    public class RenderedSpriteReference
    {
        public RenderedSpriteCache Cache = new();

        public RenderedSpriteReference() { }
    }

    public readonly struct RenderedSpriteCache
    {
        public Animation CurrentAnimation { get; init; }
        public Vector2 RenderPosition { get; init; }
        public Vector2 Scale { get; init; }
        public Vector2 Offset { get; init; }
        public float Rotation { get; init; }
        public Guid RenderedSprite { get; init; }
        public ImageFlip ImageFlip { get; init; }
        public float Sorting { get; init; }
        public BlendStyle Blend { get; init; }
        public AnimationInfo AnimInfo { get; init; }
        public OutlineStyle Outline { get; init; }
        public Color Color { get; init; }

        /// <summary>
        /// The last recorded animation frame. Uses the internal animation frame, not the generic frame index.
        /// </summary>
        public int LastFrameIndex { get; init; }

        public Point SpriteSize { get; init; }
    }
}
