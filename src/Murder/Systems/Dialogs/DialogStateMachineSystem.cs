using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Bang.StateMachines;
using Murder.Components;

namespace Murder.Systems
{
    [DoNotPause]
    [Filter(kind: ContextAccessorKind.Read, typeof(IStateMachineComponent))]
    [Filter(ContextAccessorFilter.AnyOf, typeof(DoNotPauseStateMachineComponent), typeof(SituationComponent))]
    public class DialogStateMachineSystem : IUpdateSystem
    {
        public ValueTask Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                IStateMachineComponent routine = e.GetStateMachine();
                routine.Tick(Game.DeltaTime);
            }

            return default;
        }
    }
}
