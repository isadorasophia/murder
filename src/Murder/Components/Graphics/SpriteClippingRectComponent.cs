using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components
{
    public readonly struct SpriteClippingRectComponent : IComponent
    {
        public readonly float BorderLeft;
        public readonly float BorderRight;
        public readonly float BorderUp;
        public readonly float BorderDown;

        public SpriteClippingRectComponent(float borderLeft, float borderRight, float borderUp, float borderDown)
        {
            BorderLeft = borderLeft;
            BorderRight = borderRight;
            BorderUp = borderUp;
            BorderDown = borderDown;
        }
    }
}
