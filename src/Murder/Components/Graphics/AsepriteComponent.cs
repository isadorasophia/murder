using Bang;
using Bang.Components;
using System.Collections.Immutable;
using Murder.Attributes;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Utilities;

namespace Murder.Components
{
    [Requires(typeof(ITransformComponent))]
    public readonly struct AsepriteComponent : IComponent
    {
        public readonly TargetSpriteBatches TargetSpriteBatch = TargetSpriteBatches.Gameplay;

        [GameAssetId(typeof(AsepriteAsset))]
        public readonly Guid AnimationGuid = Guid.Empty;

        [Tooltip("(0,0) is top left and (1,1) is bottom right"), Slider()]
        public readonly Vector2 Offset = Vector2.Zero;

        public readonly bool RotateWithFacing = false;
        public readonly bool FlipWithFacing = false;

        [HideInEditor]
        public readonly string AnimationId = string.Empty;
        public readonly ImmutableArray<string> NextAnimations = ImmutableArray<string>.Empty;

        internal bool HasAnimation(string animationName)
        {
            return Game.Data.GetAsset<AsepriteAsset>(AnimationGuid).Animations.ContainsKey(animationName);
        }

        public readonly float AnimationStartedTime = 0;

        public readonly int YSortOffset = 0;

        public AsepriteComponent() { }

        public AsepriteComponent(Guid guid, Vector2 offset, string id, int ySortOffset, bool backAnim, bool flip, float startTime, TargetSpriteBatches targetSpriteBatch)
            : this(guid, offset, ImmutableArray.Create(id), ySortOffset, backAnim, flip, startTime, targetSpriteBatch) { }

        public AsepriteComponent(Guid guid, Vector2 offset, ImmutableArray<string> id, int ySortOffset, bool rotate, bool flip, float startTime, TargetSpriteBatches targetSpriteBatch)
        {
            AnimationGuid = guid;
            Offset = offset;
            AnimationId = id[0];

            NextAnimations = id.Take(new Range(1, id.Length)).ToImmutableArray();
            AnimationStartedTime = startTime;
            YSortOffset = ySortOffset;
            RotateWithFacing = rotate;
            FlipWithFacing = flip;
            TargetSpriteBatch = targetSpriteBatch;
        }
        public AsepriteComponent PlayOnce(string id, bool useScaledTime)
        {
            if (id != AnimationId)
                return new AsepriteComponent(AnimationGuid, Offset, id, YSortOffset, RotateWithFacing, FlipWithFacing, useScaledTime ? Game.Now : Game.NowUnescaled, TargetSpriteBatch);
            else
                return this;
        }
        public AsepriteComponent PlayAfter(string id)
        {
            if (id != AnimationId && !NextAnimations.Contains(id))
            {
                var sequence = ImmutableArray.CreateBuilder<string>();
                sequence.Add(AnimationId);
                sequence.AddRange(NextAnimations);
                sequence.Add(id);
                return new AsepriteComponent(
                    AnimationGuid,
                    Offset,
                    sequence.ToImmutable(),
                    YSortOffset,
                    RotateWithFacing,
                    FlipWithFacing,
                    AnimationStartedTime,
                    TargetSpriteBatch);
            }
            else
                return this;
        }

        internal AsepriteComponent StartNow(float startTime) => new AsepriteComponent(AnimationGuid, Offset, NextAnimations.Insert(0,AnimationId), YSortOffset, RotateWithFacing, FlipWithFacing, startTime, TargetSpriteBatch);
        public AsepriteComponent Play(bool useScaledTime, params string[] id) => new AsepriteComponent(AnimationGuid, Offset, id.ToImmutableArray(), YSortOffset, RotateWithFacing, FlipWithFacing, useScaledTime ? Game.Now : Game.NowUnescaled, TargetSpriteBatch);
        public AsepriteComponent Play(bool useScaledTime, ImmutableArray<string> id) => new AsepriteComponent(
            AnimationGuid,
            Offset,
            HasAnimation(id[0]) ? id : ImmutableArray.Create(AnimationId),
            YSortOffset,
            RotateWithFacing,
            FlipWithFacing,
            useScaledTime ? Game.Now : Game.NowUnescaled,
            TargetSpriteBatch);

        public AsepriteComponent SetBatch(TargetSpriteBatches batch) => new AsepriteComponent(
            AnimationGuid,
            Offset,
            NextAnimations.Insert(0, AnimationId),
            YSortOffset,
            RotateWithFacing,
            FlipWithFacing,
            AnimationStartedTime,
            batch);

        public AsepriteComponent WithSort(int sort) => new AsepriteComponent(
            AnimationGuid,
            Offset,
            NextAnimations.Insert(0, AnimationId),
            sort,
            RotateWithFacing,
            FlipWithFacing,
            AnimationStartedTime,
            TargetSpriteBatch);
    }
}
