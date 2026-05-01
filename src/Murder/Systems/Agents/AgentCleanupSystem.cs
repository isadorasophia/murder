using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Utilities;

namespace Murder.Systems
{
    [Filter(typeof(AgentComponent))]
    [Filter(ContextAccessorFilter.AnyOf, typeof(VelocityComponent), typeof(AgentImpulseComponent))]
    [Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
    public class AgentCleanupSystem : IUpdateSystem
    {
        public void Update(Context context)
        {
            foreach (var e in context.Entities)
            {
                var agent = e.GetAgent();

                AgentImpulseComponent? impulse = e.TryGetAgentImpulse();
                ChangeAgentFrictionUntil? changeFriction = e.TryGetChangeAgentFrictionUntil();
                if (changeFriction != null && changeFriction.Value.UntilTime < Game.Now)
                {
                    e.RemoveChangeAgentFrictionUntil();
                    changeFriction = null;
                }


                bool requiresFriction = impulse?.Impulse.HasValue() ?? false;
                if (impulse?.Impulse.LengthSquared() > agent.Speed * agent.Speed)
                {
                    requiresFriction = true;
                }

                if (!requiresFriction) // Cleanup the impulse
                {
                    // Set the friction if there is no impulse

                    if (changeFriction != null)
                    {
                        float changeFrictionAmount = 1 - Calculator.ClampTime(Game.Now - changeFriction.Value.StartTime, changeFriction.Value.UntilTime - changeFriction.Value.StartTime, EaseKind.QuadIn);
                        float finalFriction;
                        if ((changeFriction.Value.FrictionFlags.HasFlag(ChangeFrictionFlags.Reduce) && changeFriction.Value.Amount <= agent.Friction) ||
                            (changeFriction.Value.FrictionFlags.HasFlag(ChangeFrictionFlags.Increase) && changeFriction.Value.Amount >= agent.Friction))
                        {
                            finalFriction = Calculator.Lerp(agent.Friction, changeFriction.Value.Amount, changeFrictionAmount);
                        }
                        else
                        {
                            finalFriction = agent.Friction;
                        }

                        e.SetFriction(finalFriction);
                    }
                    else
                    {
                        e.SetFriction(agent.Friction);
                    }
                }

                if (impulse is not null && !impulse.Value.Flags.HasFlag(AgentImpulseFlags.DoNotClear))
                {
                    e.RemoveAgentImpulse();
                }
            }
        }
    }
}