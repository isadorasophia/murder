using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Core.Graphics
{
    public readonly struct AnimationInfo
    {
        public static readonly AnimationInfo Default = new AnimationInfo();
        
        public static readonly AnimationInfo Ui = new AnimationInfo()
        {
            UseScaledTime = true
        };

        public float Start { get; init; } = 0f;
        public float Duration { get; init; } = -1f;
        public bool UseScaledTime { get; init; } = false;
        public bool Loop { get; init; } = true;
        public string Name { get; init; } = string.Empty;
        
        public AnimationInfo()
        {
        }

        public AnimationInfo(string name, float start) : this()
        {
            Name = name;
            Start = start;
        }

        public AnimationInfo(string name) : this()
        {
            Name = name;
        }
    }
}
