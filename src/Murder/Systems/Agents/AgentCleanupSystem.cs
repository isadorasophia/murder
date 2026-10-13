using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Agents;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems
{
    [Filter(typeof(AgentComponent))]
    [Filter(ContextAccessorFilter.AnyOf, typeof(VelocityComponent), typeof(AgentImpulseComponent), typeof(AgentWalkingComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
    public class AgentCleanupSystem : IFixedUpdateSystem
    {
        public void FixedUpdate(Context context)
        {
            Cleanup(context);
        }

        private void Cleanup(Context context)
        {
            foreach (var e in context.Entities)
            {
                var agent = e.GetAgent();

                AgentImpulseComponent? impulse = e.TryGetAgentImpulse();

                bool requiresFriction = impulse?.Impulse.HasValue() ?? false;
                if (impulse?.Impulse.LengthSquared() > agent.Speed * agent.Speed)
                {
                    requiresFriction = true;
                }

                if (!requiresFriction) // Cleanup the impulse
                {
                    // Set the friction if there is no impulse
                    e.SetFriction(agent.Friction);
                }

                if (impulse?.Impulse is Vector2 intent && intent.HasValue())
                {
                    e.SetAgentWalking(new AgentWalkingComponent(intent));
                }
                else if (e.HasAgentWalking())
                {
                    e.RemoveAgentWalking();
                }

                if (impulse is not null && !impulse.Value.Flags.HasFlag(AgentImpulseFlags.DoNotClear))
                {

                    e.RemoveAgentImpulse();
                }
            }
        }
    }

}