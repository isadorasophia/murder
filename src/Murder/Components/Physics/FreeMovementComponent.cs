using Bang.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;

public readonly struct FreeMovementComponent : IComponent
{
    public readonly float Since;

    public FreeMovementComponent()
    {
        Since = Game.Now;
    }
}