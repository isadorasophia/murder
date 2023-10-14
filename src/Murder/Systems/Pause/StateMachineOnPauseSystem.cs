using Bang.Contexts;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems
{
    [OnPause]
    [Filter(typeof(IStateMachineComponent), typeof(DoNotPauseComponent))]
    public class StateMachineOnPauseSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                IStateMachineComponent routine = e.GetComponent<IStateMachineComponent>();
                routine.Tick(Game.UnscaledDeltaTime);
            }
        }
    }
}