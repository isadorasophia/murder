using Bang;
using Bang.Components;
using System.Collections.Immutable;
using Murder.Attributes;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using Murder.Core;

namespace Murder.Components
{
    [Requires(typeof(ITransformComponent))]
    [CustomName(" Aseprite Component")]
    public readonly struct SpriteComponent : IComponent
    {
        public readonly TargetSpriteBatches TargetSpriteBatch = TargetSpriteBatches.Gameplay;

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
        public readonly bool CanBeHighlighted = true;

        /// <summary>
        /// Current playing animation id.
        /// </summary>
        public readonly string CurrentAnimation => NextAnimations.FirstOrDefault() ?? string.Empty;
        
        
        public readonly ImmutableArray<string> NextAnimations = ImmutableArray<string>.Empty;

        public bool HasAnimation(string animationName)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(AnimationGuid) is SpriteAsset sprite)
            {
                return sprite.Animations.ContainsKey(animationName);
            }

            return false;
        }

        public readonly float AnimationStartedTime = -1;

        public readonly int YSortOffset = 0;

        public SpriteComponent() { }
        public SpriteComponent(Portrait portrait) :
            this(portrait.Aseprite, Vector2.Zero, portrait.AnimationId, 0, false, false, true, 0, TargetSpriteBatches.Gameplay) { }

        public SpriteComponent(Guid guid, Vector2 offset, string id, int ySortOffset, bool backAnim, bool flip, bool canBeHighlighted, float startTime, TargetSpriteBatches targetSpriteBatch)
            : this(guid, offset, ImmutableArray.Create(id), ySortOffset, backAnim, flip, canBeHighlighted, startTime, targetSpriteBatch) { }

        public SpriteComponent(Guid guid, Vector2 offset, ImmutableArray<string> id, int ySortOffset, bool rotate, bool flip, bool canBeHighlighted, float startTime, TargetSpriteBatches targetSpriteBatch)
        {
            AnimationGuid = guid;
            Offset = offset;

            NextAnimations = id;
            AnimationStartedTime = startTime;
            YSortOffset = ySortOffset;
            RotateWithFacing = rotate;
            FlipWithFacing = flip;
            CanBeHighlighted = canBeHighlighted;
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

        public SpriteComponent PlayOnce(string id, bool useScaledTime)
        {
            if (id != CurrentAnimation)
                return new SpriteComponent(AnimationGuid, Offset, id, YSortOffset, RotateWithFacing, FlipWithFacing, CanBeHighlighted, useScaledTime ? Game.Now : Game.NowUnescaled, TargetSpriteBatch);
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
                    CanBeHighlighted,
                    AnimationStartedTime,
                    TargetSpriteBatch);
            }
            else
                return this;
        }

        internal SpriteComponent StartNow(float startTime) => new SpriteComponent(AnimationGuid, Offset, NextAnimations, YSortOffset, RotateWithFacing, FlipWithFacing, CanBeHighlighted, startTime, TargetSpriteBatch);
        public SpriteComponent Play(bool useScaledTime, params string[] id) => new SpriteComponent(AnimationGuid, Offset, id.ToImmutableArray(), YSortOffset, RotateWithFacing, FlipWithFacing, CanBeHighlighted, useScaledTime ? Game.Now : Game.NowUnescaled, TargetSpriteBatch);
        public SpriteComponent Play(bool useScaledTime, ImmutableArray<string> id) => new SpriteComponent(
            AnimationGuid,
            Offset,
            HasAnimation(id[0]) ? id : ImmutableArray.Create(CurrentAnimation),
            YSortOffset,
            RotateWithFacing,
            FlipWithFacing,
            CanBeHighlighted,
            useScaledTime ? Game.Now : Game.NowUnescaled,
            TargetSpriteBatch);

        public SpriteComponent SetBatch(TargetSpriteBatches batch) => new SpriteComponent(
            AnimationGuid,
            Offset,
            NextAnimations,
            YSortOffset,
            RotateWithFacing,
            FlipWithFacing,
            CanBeHighlighted,
            AnimationStartedTime,
            batch);

        public SpriteComponent WithSort(int sort) => new SpriteComponent(
            AnimationGuid,
            Offset,
            NextAnimations,
            sort,
            RotateWithFacing,
            FlipWithFacing,
            CanBeHighlighted,
            AnimationStartedTime,
            TargetSpriteBatch);

    }
}
