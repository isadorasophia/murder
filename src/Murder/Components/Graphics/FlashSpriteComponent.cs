using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components
{
    public readonly struct FlashSpriteComponent : IComponent
    {
        public readonly float DestroyAtTime;

        public FlashSpriteComponent(float destroyTimer)
        {
            DestroyAtTime = destroyTimer;
        }
    }
}