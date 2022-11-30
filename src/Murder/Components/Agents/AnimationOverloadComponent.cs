using Newtonsoft.Json;
using Bang.Components;

namespace Murder.Components
{
    public readonly struct AnimationOverloadComponent : IComponent
    {
        public string AnimationId => _animationId[0];
        public string CurrentAnimation => _animationId[Current];

        [JsonProperty]
        private readonly string[] _animationId;

        public readonly float Start;

        public readonly float Duration = -1.0f;

        public readonly bool Loop;

        public readonly int Current = 0;

        public AnimationOverloadComponent(string animationId, bool loop) : this(animationId, -1, loop)
        { }
        
        public AnimationOverloadComponent(string animationId, float duration, bool loop)
        {
            _animationId = new string[] { animationId };
            Duration = duration;
            Loop = loop;

            Start = Game.Now;
        }

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

        public AnimationOverloadComponent PlayNext() => new AnimationOverloadComponent(Loop, _animationId, Math.Min(_animationId.Length - 1, Current + 1));
        
    }
}
