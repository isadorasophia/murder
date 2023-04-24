using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Geometry;
using Murder.Utilities;
using Bang.Components;

namespace Murder.Systems
{
    /// <summary>
    /// Simple system for moving agents to another position. Looks for 'MoveTo' components and adds agent inpulses to it.
    /// </summary>
    [Filter(typeof(ITransformComponent), typeof(MoveToPerfectComponent))]
    public class MoveToPerfectSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                MoveToPerfectComponent moveToPerfect = e.GetMoveToPerfect();
                if (moveToPerfect.StartPosition is not Vector2 startPosition)
                {
                    startPosition = e.GetGlobalTransform().Vector2;
                    e.SetMoveToPerfect(moveToPerfect.WithStartPosition(startPosition));
                }

                double delta = Calculator.Clamp01((Game.Now - moveToPerfect.StartTime) / moveToPerfect.Duration);
                double easedDelta = Ease.Evaluate(delta, moveToPerfect.EaseKind);
                
                Vector2 current = Vector2.LerpSnap(startPosition, moveToPerfect.Target, easedDelta);
                e.SetGlobalTransform(e.GetTransform().With(current.Point));

                if (delta >= 1)
                {
                    e.RemoveMoveToPerfect();
                    // Send message?
                }
            }
        }
    }
}
