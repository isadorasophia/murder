using Bang.Contexts;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems
{
    [Filter(ContextAccessorFilter.AllOf, typeof(PositionComponent), typeof(AgentComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(AgentImpulseComponent))]
    internal class AgentHalterSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                // TODO: Generate extensions
                //var agent = e.GetAgent();
                //e.SetFriction(agent.Friction);
            }

            return default;
        }
    }
}
