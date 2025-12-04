using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Physics
{
    public readonly struct GravityMultiplierComponent : IComponent
    {
        public readonly float Multiply = 1f;

        public GravityMultiplierComponent()
        {
        }
    }
}
