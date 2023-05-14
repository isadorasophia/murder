using Newtonsoft.Json;
using Bang.Components;
using Murder.Attributes;
using Murder.Assets.Graphics;
using Murder.Diagnostics;

namespace Murder.Components
{
    public readonly struct AnimationOverloadComponent : IComponent
    {
        public string AnimationId => _animationId[0];
        [ShowInEditor]
        public string CurrentAnimation { 
            get {
                if (Current>=0 && Current< _animationId.Length)
                    return _animationId[Current];

                GameLogger.Warning("Trying to play animation overload with out of bounds index");
                return string.Empty;
            } 
        }

        [JsonProperty]
        private readonly string[] _animationId;

        [GameAssetId<SpriteAsset>]
        private readonly Guid _customSprite = Guid.Empty;

        public readonly float Start;

        public readonly float Duration = -1.0f;

        public readonly bool Loop;

        public readonly bool IgnoreFacing;

        public readonly int Current = 0;
        public readonly int AnimationCount => _animationId.Length;
        public readonly float SortOffset = 0f;

        public SpriteAsset? CustomSprite
        {
            get
            {
                if (_customSprite != Guid.Empty && Game.Data.TryGetAsset<SpriteAsset>(_customSprite) is SpriteAsset asset)
                    return asset;

                return null;
            }
        }

        public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing) : this(animationId, -1, loop, ignoreFacing)
        { }

        public AnimationOverloadComponent(string[] animations, float duration, bool loop, bool ignoreFacing, int current, float sortOffset, Guid customSprite)
        {
            _animationId = animations;
            Duration = duration;
            Loop = loop;
            IgnoreFacing = ignoreFacing;
            Start = Game.Now;
            Current = current;
            _customSprite = customSprite;
            SortOffset = sortOffset;
        }

        public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing)
        {
            _animationId = new string[] { animationId };
            Duration = duration;
            Loop = loop;
            IgnoreFacing = ignoreFacing;
            
            Start = Game.Now;
            _customSprite = Guid.Empty;
        }

        public AnimationOverloadComponent PlayNext() => new AnimationOverloadComponent(
            _animationId, Duration, Loop, IgnoreFacing, Math.Min(_animationId.Length - 1, Current + 1), SortOffset, _customSprite);
        
    }
}
