using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;

namespace Murder.Systems
{

    [Filter(typeof(AgentComponent))]
    [Filter(ContextAccessorFilter.AnyOf, typeof(VelocityComponent), typeof(AgentImpulseComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent), typeof(AgentPauseComponent))]
    internal class AgentCleanupSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                var agent = e.GetAgent();

                AgentImpulseComponent? impulse = e.TryGetAgentImpulse();
                bool hasImpulse = impulse?.Impulse.HasValue() ?? false;

                if (!hasImpulse) // Cleanup the impulse
                {
                    // Set the friction if there is no impulse
                    e.SetFriction(agent.Friction);
                }

                if (impulse is not null && !impulse.Value.Flags.HasFlag(AgentImpulseFlags.DoNotClear))
                {
                    e.RemoveAgentImpulse();
                }
            }
        }
    }
}