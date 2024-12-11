using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Graphics;

public readonly struct SpriteFrameComponent : IComponent
{
    public readonly int Frame;

    public SpriteFrameComponent(int frame)
    {
        Frame = frame;
    }
}
