using Bang.Components;
using Murder.Core;
using Murder.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Graphics;

public readonly struct FlipSpriteComponent : IComponent
{
    public readonly ImageFlip Orientation = ImageFlip.Horizontal;

    public FlipSpriteComponent(ImageFlip flip)
    {
        Orientation = flip;
    }
}
