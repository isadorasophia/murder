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

                float delta = Calculator.Clamp01((Game.Now - moveToPerfect.Start) / moveToPerfect.Duration);
                float easedDelta = Ease.Evaluate(delta, moveToPerfect.EaseKind);

                IMurderTransformComponent transform = e.GetGlobalTransform();

                Vector2 current = Vector2.LerpSnap(transform.Vector2, moveToPerfect.Target, easedDelta);
                e.SetGlobalTransform(transform.With(current));

                if (delta >= 1)
                {
                    e.RemoveMoveToPerfect();
                    // Send message?
                }
            }
        }
    }
}
