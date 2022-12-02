using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Bang.StateMachines;
using Murder.Components;

namespace Murder.Systems
{
    [OnPause]
    [Filter(typeof(IStateMachineComponent), typeof(DoNotPauseComponent))]
    public class StateMachineOnPauseSystem : IUpdateSystem
    {
        public ValueTask Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                IStateMachineComponent routine = e.GetComponent<IStateMachineComponent>();
                routine.Tick(Game.DeltaTime);
            }

            return default;
        }
    }
}
