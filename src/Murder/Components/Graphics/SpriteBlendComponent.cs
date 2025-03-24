using Bang.Components;
using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Graphics;

public readonly struct SpriteBlendComponent : IComponent
{
    public readonly BlendStyle BlendStyle = BlendStyle.Normal;
    public readonly Murder.Core.Graphics.MurderBlendState BlendState = Murder.Core.Graphics.MurderBlendState.AlphaBlend;

    public SpriteBlendComponent()
    {
    }
}
