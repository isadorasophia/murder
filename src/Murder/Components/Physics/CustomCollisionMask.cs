using Bang.Components;
using Murder.Core.Physics;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components
{
    public readonly struct CustomCollisionMask : IComponent
    {
        [CollisionLayer]
        public readonly int CollisionMask = CollisionLayersBase.SOLID | CollisionLayersBase.HOLE;

        public CustomCollisionMask()
        {
        }
    }
}
