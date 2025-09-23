using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Components
{
    [Requires(typeof(PositionComponent))]
    [CustomName(" Sprite Component")]
    public readonly struct SpriteComponent : IComponent
    {
        [SpriteBatchReference]
        public readonly int TargetSpriteBatch { get; init; } = Batches2D.GameplayBatchId;

        /// <summary>
        /// The Guid of the Aseprite file.
        /// </summary>
        [GameAssetId(typeof(SpriteAsset))]
        public readonly Guid AnimationGuid { get; init; } = Guid.Empty;

        /// <summary>
        /// (0,0) is top left and (1,1) is bottom right
        /// </summary>
        [Tooltip("(0,0) is top left and (1,1) is bottom right"), Slider()]
        public readonly Vector2 Offset = Vector2.Zero;

        public readonly bool RotateWithFacing { get; init; } = false;
        public readonly OutlineStyle HighlightStyle { get; init; } = OutlineStyle.None;

        public readonly bool UseUnscaledTime = false;

        /// <summary>
        /// Current playing animation id.
        /// </summary>
        public readonly string CurrentAnimation => NextAnimations.FirstOrDefault() ?? string.Empty;

        public readonly ImmutableArray<string> NextAnimations { get; init; } = [];

        public bool HasAnimation(string animationName)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(AnimationGuid) is SpriteAsset sprite)
            {
                return sprite.Animations.ContainsKey(animationName);
            }

            return false;
        }

        public readonly int YSortOffset { get; init; } = 0;

        public SpriteComponent() { }

        public SpriteComponent(Guid guid) :
            this(guid, Vector2.Zero, [], 0, false, OutlineStyle.Full, Batches2D.GameplayBatchId)
        { }

        public SpriteComponent(Portrait portrait, int batchId) :
            this(portrait.Sprite, Vector2.Zero, [portrait.AnimationId], 0, false, OutlineStyle.Full, batchId)
        { }

        public SpriteComponent(Portrait portrait) :
            this(portrait.Sprite, Vector2.Zero, [portrait.AnimationId], 0, false, OutlineStyle.Full, Batches2D.GameplayBatchId)
        { }

        public SpriteComponent(Portrait portrait, int batchId, int yOffset) :
            this(portrait.Sprite, Vector2.Zero, [portrait.AnimationId], yOffset, false, OutlineStyle.Full, batchId)
        { }

        public SpriteComponent(Guid guid, Vector2 offset, ImmutableArray<string> id, int ySortOffset, bool rotate, OutlineStyle highlightStyle, int targetSpriteBatch)
        {
            AnimationGuid = guid;
            Offset = offset;

            NextAnimations = id;
            YSortOffset = ySortOffset;
            RotateWithFacing = rotate;
            HighlightStyle = highlightStyle;
            TargetSpriteBatch = targetSpriteBatch;
        }

        public bool IsPlaying(ImmutableArray<string> animations)
        {
            return NextAnimations.SequenceEqual(animations);
        }

        public SpriteComponent ClearAllNext() => this with
        {
            NextAnimations = [CurrentAnimation]
        };

        public SpriteComponent PlayAfter(IList<string> ids) => this with
        {
            NextAnimations = NextAnimations.AddRange(ids)
        };

        public SpriteComponent Play(params string[] id) => this with 
        {
            NextAnimations = [.. id]
        };

        public SpriteComponent Play(ImmutableArray<string> id) => this with
        {
            NextAnimations = HasAnimation(id[0]) ? id : [CurrentAnimation]
        };

        public SpriteComponent Play(ImmutableArray<string> id, Guid? sprite = null) => this with
        {
            NextAnimations = sprite is not null || HasAnimation(id[0]) ? id : [CurrentAnimation],
            AnimationGuid = sprite ?? AnimationGuid
        };

        public SpriteComponent SetBatch(int batch) => this with
        {
            TargetSpriteBatch = batch
        };

        public SpriteComponent WithSort(int sort) => this with { YSortOffset = sort };

        public SpriteComponent WithPortrait(Portrait portrait) => this with
        {
            AnimationGuid = portrait.Sprite,
            NextAnimations = [portrait.AnimationId]
        };
    }
}