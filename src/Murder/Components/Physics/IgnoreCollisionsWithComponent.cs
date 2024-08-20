using Bang.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Physics
{
    public readonly struct IgnoreCollisionsWithComponent : IComponent
    {
        public readonly ImmutableHashSet<int> Ids = [];

        public IgnoreCollisionsWithComponent()
        {
            
        }

        public IgnoreCollisionsWithComponent(int id)
        {
            Ids = [id];
        }

        public IgnoreCollisionsWithComponent(ImmutableHashSet<int> ids)
        {
            Ids = ids;
        }
    }
}
