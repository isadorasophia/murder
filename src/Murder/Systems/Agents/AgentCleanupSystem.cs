using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems
{

    [Filter(typeof(AgentComponent))]
    [Filter(ContextAccessorFilter.AnyOf, typeof(VelocityComponent), typeof(AgentImpulseComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
    internal class AgentCleanupSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var agent = e.GetAgent();
                var hasImpulse = e.TryGetAgentImpulse()?.Impulse.HasValue ?? false;

                if (!hasImpulse)     // Cleanup the impulse
                {
                    // Set the friction if there is no impulse
                    e.SetFriction(agent.Friction);
                }
                
                e.RemoveAgentImpulse();
            }
        }
    }
}
