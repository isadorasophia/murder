using Bang;
using Bang.Entities;
using Bang.Systems;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Systems.Graphics
{
    internal class RandomizeAsepriteSystem : IReactiveSystem
    {
        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }

        public ValueTask OnModified(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }

        public ValueTask OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }
    }
}
