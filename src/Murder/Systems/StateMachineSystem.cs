using Bang.Entities;
using Bang.Contexts;
using Bang.Systems;
using Bang.StateMachines;

namespace Murder.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(IStateMachineComponent))]
    public class StateMachineSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            foreach (Entity e in context.Entities)
            {
                IStateMachineComponent routine = e.GetComponent<IStateMachineComponent>();
                routine.Tick(Game.DeltaTime);
            }
        }
    }
}
