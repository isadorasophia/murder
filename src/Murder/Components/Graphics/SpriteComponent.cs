using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Murder.Components
{
    [Requires(typeof(ITransformComponent))]
    [CustomName(" Sprite Component")]
    public readonly struct SpriteComponent : IComponent
    {
        [SpriteBatchReference]
        public readonly int TargetSpriteBatch = Batches2D.GameplayBatchId;

        /// <summary>
        /// The Guid of the Aseprite file.
        /// </summary>
        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid AnimationGuid = Guid.Empty;

        /// <summary>
        /// (0,0) is top left and (1,1) is bottom right
        /// </summary>
        [Tooltip("(0,0) is top left and (1,1) is bottom right"), Slider()]
        public readonly Vector2 Offset = Vector2.Zero;

        public readonly bool RotateWithFacing = false;
        public readonly bool FlipWithFacing = false;

        [HideInEditor]
        public readonly bool CanBeHighlighted
        {
            init
            {
                if (value)
                    HighlightStyle = OutlineStyle.Full;
                else
                    HighlightStyle = OutlineStyle.None;
            }
        }

        public readonly OutlineStyle HighlightStyle = OutlineStyle.None;

        public readonly bool UseUnscaledTime = false;

        /// <summary>
        /// Current playing animation id.
        /// </summary>
        public readonly string CurrentAnimation => NextAnimations.FirstOrDefault() ?? string.Empty;

        /// <summary>
        /// Legacy property for serialization. Do not use.
        /// </summary>
        public float AnimationStartedTime{set { }}

        public readonly ImmutableArray<string> NextAnimations = ImmutableArray<string>.Empty;

        public bool HasAnimation(string animationName)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(AnimationGuid) is SpriteAsset sprite)
            {
                return sprite.Animations.ContainsKey(animationName);
            }

            return false;
        }

        public readonly int YSortOffset = 0;

        public SpriteComponent() { }

        public SpriteComponent(Portrait portrait, int batchId) :
            this(portrait.Sprite, Vector2.Zero, [portrait.AnimationId], 0, false, false, OutlineStyle.Full, batchId)
        { }

        public SpriteComponent(Portrait portrait) :
            this(portrait.Sprite, Vector2.Zero, [portrait.AnimationId], 0, false, false, OutlineStyle.Full, Batches2D.GameplayBatchId)
        { }
        
        public SpriteComponent(Guid guid, Vector2 offset, ImmutableArray<string> id, int ySortOffset, bool rotate, bool flip, OutlineStyle highlightStyle, int targetSpriteBatch)
        {
            AnimationGuid = guid;
            Offset = offset;

            NextAnimations = id;
            YSortOffset = ySortOffset;
            RotateWithFacing = rotate;
            FlipWithFacing = flip;
            HighlightStyle = highlightStyle;
            TargetSpriteBatch = targetSpriteBatch;
        }

        public bool IsPlaying(string animationName)
        {
            return CurrentAnimation == animationName;
        }
        public bool IsPlaying(params string[] animations)
        {
            return NextAnimations.SequenceEqual(animations);
        }

        public SpriteComponent PlayOnce(string id, bool _)
        {
            if (id != CurrentAnimation)
                return new SpriteComponent(AnimationGuid, Offset, [id], YSortOffset, RotateWithFacing, FlipWithFacing, HighlightStyle, TargetSpriteBatch);
            else
                return this;
        }

        public SpriteComponent PlayAfter(string id)
        {
            if (id != CurrentAnimation && !NextAnimations.Contains(id))
            {
                return new SpriteComponent(
                    AnimationGuid,
                    Offset,
                    NextAnimations.Add(id),
                    YSortOffset,
                    RotateWithFacing,
                    FlipWithFacing,
                    HighlightStyle,
                    TargetSpriteBatch);
            }
            else
                return this;
        }
        
        public SpriteComponent Play(params string[] id) => new SpriteComponent(AnimationGuid, Offset, id.ToImmutableArray(), YSortOffset, RotateWithFacing, FlipWithFacing, HighlightStyle, TargetSpriteBatch);
        public SpriteComponent Play(ImmutableArray<string> id) => new SpriteComponent(
            AnimationGuid,
            Offset,
            HasAnimation(id[0]) ? id : [CurrentAnimation],
            YSortOffset,
            RotateWithFacing,
            FlipWithFacing,
            HighlightStyle,
            TargetSpriteBatch);

        public SpriteComponent SetBatch(int batch) => new SpriteComponent(
            AnimationGuid,
            Offset,
            NextAnimations,
            YSortOffset,
            RotateWithFacing,
            FlipWithFacing,
            HighlightStyle,
            batch);

        public SpriteComponent WithSort(int sort) => new SpriteComponent(
            AnimationGuid,
            Offset,
            NextAnimations,
            sort,
            RotateWithFacing,
            FlipWithFacing,
            HighlightStyle,
            TargetSpriteBatch);

        public SpriteComponent WithPortrait(Portrait portrait) => new SpriteComponent(
            portrait.Sprite,
            Offset,
            [portrait.AnimationId],
            YSortOffset,
            RotateWithFacing,
            FlipWithFacing,
            HighlightStyle,
            TargetSpriteBatch);

        public SpriteComponent Reset()
        {
            return new SpriteComponent(AnimationGuid, Offset, NextAnimations, YSortOffset, RotateWithFacing, FlipWithFacing, HighlightStyle, TargetSpriteBatch);
        }
    }
}