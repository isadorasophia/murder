using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components.Agents
{
    [RuntimeOnly, DoNotPersistOnSave]
    public readonly struct KnockbackComponent : IComponent
    {
        public readonly float HalfLife;
        public readonly float EndTime;

        public KnockbackComponent(float halfLife, float endTime)
        {
            HalfLife = halfLife;
            EndTime = endTime;
        }
    }
}
