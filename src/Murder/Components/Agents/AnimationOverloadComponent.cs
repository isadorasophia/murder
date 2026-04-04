using Bang;
using Bang.Components;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Components
{
    public readonly struct AnimationOverloadComponent : IComponent
    {
        // ===== fields and setters =====
        [Serialize]
        [Tooltip("If this is set, replace the animation id.")]
#pragma warning disable IDE1006 // naming style
        private readonly ImmutableArray<string> _nextAnimationsOverload = [];
#pragma warning restore IDE1006

        [Tooltip("If this is set, replace the sprite animation.")]
        [Serialize]
        [GameAssetId<SpriteAsset>]
        private readonly Guid _customSprite = Guid.Empty;

        [JsonIgnore]
        public readonly float Start { get; init; } = 0;

        public readonly float Duration = -1.0f;

        public bool Loop { get; init; } = false;

        public readonly bool IgnoreFacing { get; init; } = false;

        public readonly ImageFlip Flip { get; init; } = ImageFlip.None;

        public readonly int Current { get; init; } = 0;
        public readonly float SortOffset { get; init; } = 0f;

        /// <summary>
        /// Supported facing directions, optional.
        /// </summary>
        public readonly int? SupportedDirections { get; init; } = null;

        /// <summary>
        /// Supported orientations, if applicable.
        /// </summary>
        public readonly Orientation? SupportedOrientation { get; init; } = null;

        [ShowInEditor]
        public string CurrentAnimation
        {
            get
            {
                if (_nextAnimationsOverload.Length == 0)
                {
                    return string.Empty;
                }

                if (Current >= 0 && Current < _nextAnimationsOverload.Length)
                {
                    return _nextAnimationsOverload[Current];
                }

                GameLogger.Warning("Trying to play animation overload with out of bounds index");
                return string.Empty;
            }
        }

        public readonly int AnimationCount => _nextAnimationsOverload.Length;
        public bool AtLast => Current == _nextAnimationsOverload.Length - 1;

        public SpriteAsset? CustomSprite
        {
            get
            {
                if (_customSprite != Guid.Empty && Game.Data.TryGetAsset<SpriteAsset>(_customSprite) is SpriteAsset asset)
                {
                    return asset;
                }

                return null;
            }
        }

        [JsonConstructor]
        public AnimationOverloadComponent() { }

        public AnimationOverloadComponent(
            ImmutableArray<string> nextAnimationsOverload, Guid customSprite, float start, float duration, 
            bool loop, bool ignoreFacing, ImageFlip flip, int current, float sortOffset, int? supportedDirections, Orientation? orientation) 
        {
            _nextAnimationsOverload = nextAnimationsOverload;
            _customSprite = customSprite;

            Loop = loop;
            IgnoreFacing = ignoreFacing;

            Start = start;
            Duration = duration;

            Flip = flip;

            Current = current;
            SortOffset = sortOffset;
            SupportedDirections = supportedDirections;

            SupportedOrientation = orientation;
        }

        public AnimationOverloadComponent(ImmutableArray<string> nextAnimationsOverload, Guid customSprite, bool loop, bool ignoreFacing)
        {
            _nextAnimationsOverload = nextAnimationsOverload;
            _customSprite = customSprite;

            Start = Game.Now;

            Loop = loop;
            IgnoreFacing = ignoreFacing;
        }

        public AnimationOverloadComponent(string animationId, Guid customSprite, bool loop, bool ignoreFacing) : this(
            [animationId],
            customSprite,
            loop,
            ignoreFacing)
        { }

        public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing) : this(
            [animationId],
            customSprite: Guid.Empty,
            loop,
            ignoreFacing)
        { }

        public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing) : this(
            [animationId],
            customSprite: Guid.Empty,
            start: Game.Now,
            duration,
            loop,
            ignoreFacing,
            flip: ImageFlip.None,
            current: 0,
            sortOffset: 0,
            supportedDirections: null,
            orientation: null)
        { }

        public AnimationOverloadComponent Play(string animation) =>
            new([animation],
                _customSprite,
                start: Game.Now,
                Duration,
                Loop,
                IgnoreFacing,
                Flip,
                current: 0,
                SortOffset,
                SupportedDirections,
                SupportedOrientation);

        public AnimationOverloadComponent PlayNext() => this with
            { Start = Game.Now, Current = Math.Min(_nextAnimationsOverload.Length - 1, Current + 1) };

        public AnimationOverloadComponent Now => this with { Start = Game.Now };

        public AnimationOverloadComponent NoLoop => this with { Start = Game.Now, Loop = false };

        public AnimationOverloadComponent FinishCurrentNoLoop =>
            new([CurrentAnimation],
                _customSprite,
                Start,
                Duration,
                loop: false,
                IgnoreFacing,
                Flip,
                current: 0,
                SortOffset,
                SupportedDirections,
                SupportedOrientation);
    }
}