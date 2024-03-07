using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Utilities
{
    public readonly struct DestroyAfterSecondsComponent : IComponent
    {
        public readonly float Lifespan { get; init; }
    }
}
