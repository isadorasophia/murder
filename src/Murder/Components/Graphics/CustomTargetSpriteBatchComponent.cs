using Bang.Components;
using Murder.Core.Graphics;
using Murder.Utilities.Attributes;
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
        [SpriteBatchReference]
        public readonly int TargetBatch = Batches2D.GameplayBatchId;

        public CustomTargetSpriteBatchComponent(int targetBatch)
        {
            TargetBatch = targetBatch;
        } 
    }
}
