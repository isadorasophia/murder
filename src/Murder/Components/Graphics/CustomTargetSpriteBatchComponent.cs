using Bang.Components;
using Murder.Core.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Murder.Core.Graphics.RenderContext;

namespace Murder.Components
{
    public readonly struct CustomTargetSpriteBatchComponent : IComponent
    {
        public readonly TargetSpriteBatches TargetBatch = TargetSpriteBatches.Gameplay;

        public CustomTargetSpriteBatchComponent(TargetSpriteBatches targetBatch)
        {
            TargetBatch = targetBatch;
        } 
    }
}
