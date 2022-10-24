using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Bang.StateMachines;
using Murder.Components;

namespace Murder.Systems
{
    [DoNotPause]
    [Filter(kind: ContextAccessorKind.Read, typeof(SituationComponent), typeof(IStateMachineComponent))]
    public class DialogStateMachineSystem : IUpdateSystem
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
