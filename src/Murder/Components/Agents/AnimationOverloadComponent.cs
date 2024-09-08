using Bang;
using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace Murder.Components
{
    public readonly struct AnimationOverloadComponent : IComponent
    {
        public string AnimationId => _animationId[0];

        [ShowInEditor]
        public string CurrentAnimation
        {
            get
            {
                if (_animationId.Length == 0)
                {
                    return string.Empty;
                }

                if (Current >= 0 && Current < _animationId.Length)
                {
                    return _animationId[Current];
                }

                GameLogger.Warning("Trying to play animation overload with out of bounds index");
                return string.Empty;
            }
        }

        [Tooltip("If this is set, replace the animation id.")]
        [Serialize]
        private readonly ImmutableArray<string> _animationId = ImmutableArray<string>.Empty;

        [Tooltip("If this is set, replace the sprite animation.")]
        [Serialize]
        [GameAssetId<SpriteAsset>]
        private readonly Guid _customSprite = Guid.Empty;

        public readonly float Start = 0;

        public readonly float Duration = -1.0f;

        public readonly bool Loop;

        public readonly bool IgnoreFacing;

        public readonly int Current = 0;
        public readonly int AnimationCount => _animationId.Length;
        public bool AtLast => Current == _animationId.Length - 1;

        public readonly float SortOffset { get; init; } = 0f;

        public SpriteAsset? CustomSprite
        {
            get
            {
                if (_customSprite != Guid.Empty && Game.Data.TryGetAsset<SpriteAsset>(_customSprite) is SpriteAsset asset)
                    return asset;

                return null;
            }
        }

        public AnimationOverloadComponent() { }

        public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing) : this(animationId, duration: -1, loop, ignoreFacing, Game.Now, Guid.Empty)
        { }

        public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing, Guid customSprite) : this(animationId, duration: -1, loop, ignoreFacing, Game.Now, customSprite)
        { }

        public AnimationOverloadComponent(ImmutableArray<string> animationId, bool loop, bool ignoreFacing, Guid customSprite) : this(animationId, duration: -1, loop, ignoreFacing, Game.Now, customSprite)
        { }

        public AnimationOverloadComponent(ImmutableArray<string> animationId, bool loop, bool ignoreFacing) : this(animationId, duration: -1, loop, ignoreFacing, Game.Now, Guid.Empty)
        { }

        public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing, float startTime) : this(animationId, duration: -1, loop, ignoreFacing, startTime, Guid.Empty)
        { }

        public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing, int current, float sortOffset, Guid customSprite) :
            this([animationId], duration, loop, ignoreFacing, current, sortOffset, customSprite, Game.Now)
        { }

        public AnimationOverloadComponent(ImmutableArray<string> animations, float duration, bool loop, bool ignoreFacing, int current, float sortOffset, Guid customSprite) :
            this(animations, duration, loop, ignoreFacing, current, sortOffset, customSprite, Game.Now)
        { }

        public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing, float startTime, Guid customSprite)
            : this([animationId], duration, loop, ignoreFacing, startTime, customSprite)
        { }

        public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing)
            : this(animationId, duration, loop, ignoreFacing, Game.Now, customSprite: Guid.Empty)
        { }

        public AnimationOverloadComponent(string animationId, Guid customSprite, float start, bool loop, bool ignoreFacing) :
            this([animationId], customSprite, start, loop, ignoreFacing)
        { }

        public AnimationOverloadComponent(ImmutableArray<string> animations, float duration, bool loop, bool ignoreFacing, int current, float sortOffset, Guid customSprite, float start)
        {
            _animationId = animations;
            _customSprite = customSprite;

            Start = start;
            Loop = loop;
            IgnoreFacing = ignoreFacing;
            Current = current;
            SortOffset = sortOffset;

            Duration = duration;
        }

        public AnimationOverloadComponent(ImmutableArray<string> animationId, float duration, bool loop, bool ignoreFacing, float startTime, Guid customSprite)
        {
            _animationId = animationId;
            _customSprite = customSprite;

            Start = startTime;
            Loop = loop;
            Duration = duration;

            IgnoreFacing = ignoreFacing;
        }

        public AnimationOverloadComponent(ImmutableArray<string> animationId, Guid customSprite, float start, bool loop, bool ignoreFacing)
        {
            _animationId = animationId;
            _customSprite = customSprite;

            Start = start;
            Loop = loop;
            IgnoreFacing = ignoreFacing;
        }

        public AnimationOverloadComponent PlayNext() => new AnimationOverloadComponent(
            _animationId, Duration, Loop, IgnoreFacing, Math.Min(_animationId.Length - 1, Current + 1), SortOffset, _customSprite);

        public AnimationOverloadComponent Now => new AnimationOverloadComponent(
            _animationId, Duration, Loop, IgnoreFacing, Current, SortOffset, _customSprite);

        public AnimationOverloadComponent NoLoop => new AnimationOverloadComponent(
            _animationId, Duration, loop: false, IgnoreFacing, Current, SortOffset, _customSprite, Start);

    }
}