using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Bang.Entities;
using Murder.Utilities;
using Murder.Helpers;

namespace Murder.Systems.Agents;

[Filter(ContextAccessorFilter.AllOf, typeof(FacingTurnComponent))]
internal class FacingTurnSystem : IUpdateSystem
{
    public void Update(Context context)
    {
        foreach (var e in context.Entities)
        {
            FacingTurnComponent facingTurn = e.GetFacingTurn();

            float duration = facingTurn.EndTurnTime - facingTurn.StartTurnTime;
            float delta = Calculator.ClampTime(Game.Now - facingTurn.StartTurnTime, duration);

            Direction result = DirectionHelper.Lerp(facingTurn.From, facingTurn.To, delta);
            e.SetFacing(result);

            if (delta >= 1)
            {
                e.RemoveFacingTurn();
                e.SendFacingTurnCompleteMessage();
            }
        }
    }
}
