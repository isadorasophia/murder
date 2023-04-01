using Newtonsoft.Json;
using Bang.Components;
using Murder.Attributes;
using Murder.Assets.Graphics;

namespace Murder.Components
{
    public readonly struct AnimationOverloadComponent : IComponent
    {
        public string AnimationId => _animationId[0];
        [ShowInEditor]
        public string CurrentAnimation => _animationId[Current];

        [JsonProperty]
        private readonly string[] _animationId;

        [GameAssetId<AsepriteAsset>]
        private readonly Guid _customSprite = Guid.Empty;

        public readonly float Start;

        public readonly float Duration = -1.0f;

        public readonly bool Loop;

        public readonly bool IgnoreFacing;

        public readonly int Current = 0;
        public readonly int AnimationCount => _animationId.Length;

        public AsepriteAsset? CustomSprite
        {
            get
            {
                if (_customSprite != Guid.Empty && Game.Data.TryGetAsset<AsepriteAsset>(_customSprite) is AsepriteAsset asset)
                    return asset;

                return null;
            }
        }

        public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing) : this(animationId, -1, loop, ignoreFacing)
        { }

        public AnimationOverloadComponent(string[] animations, float duration, bool loop, bool ignoreFacing, int current, Guid customSprite)
        {
            _animationId = animations;
            Duration = duration;
            Loop = loop;
            IgnoreFacing = ignoreFacing;
            Start = Game.Now;
            Current = current;
            _customSprite = customSprite;
        }

        public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing)
        {
            _animationId = new string[] { animationId };
            Duration = duration;
            Loop = loop;
            IgnoreFacing = ignoreFacing;
            Start = Game.Now;
        }

        public AnimationOverloadComponent(bool loop, string animationId)
            : this(loop, new string[] { animationId })
        { }
        public AnimationOverloadComponent(bool loop, string animationId, Guid customSprite)
            : this(new string[] { animationId }, 0, loop, false, 0, customSprite)
        { }
        public AnimationOverloadComponent(bool loop, string[] animationId, Guid customSprite)
            : this(animationId, 0, loop, false, 0, customSprite)
        { }

        public AnimationOverloadComponent(bool loop, params string[] animationId)
            : this(loop, animationId, 0)
        { }

        public AnimationOverloadComponent(bool loop, string[] animationId, int current)
        {
            _animationId = animationId;
            Loop = loop;
            Current = current;

            Start = Game.Now;
        }

        public AnimationOverloadComponent PlayNext() => new AnimationOverloadComponent(
            _animationId, Duration, Loop,IgnoreFacing,  Math.Min(_animationId.Length - 1, Current + 1), _customSprite);
        
    }
}
