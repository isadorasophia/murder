using Bang.Components;
using Murder.Helpers;
using Murder.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Components;

[RuntimeOnly]
public readonly struct FacingTurnComponent : IComponent
{
    public readonly float StartTurnTime;
    public readonly float EndTurnTime;
    public readonly Direction From;
    public readonly Direction To;


    public FacingTurnComponent(Direction from, Direction to)
    {
        // Simpler version that assumes 0.5f duration
        StartTurnTime = Game.Now;
        EndTurnTime = Game.Now + 0.5f;
        From = from;
        To = to;
    }
    public FacingTurnComponent(Direction from, Direction to, float duration)
    {
        StartTurnTime = Game.Now;
        EndTurnTime = Game.Now + duration;
        From = from;
        To = to;
    }
    public FacingTurnComponent(float startTurnTime, float endTurnTime, Direction from, Direction to)
    {
        StartTurnTime = startTurnTime;
        EndTurnTime = endTurnTime;
        From = from;
        To = to;
    }
}
