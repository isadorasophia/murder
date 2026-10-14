using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Components.Agents;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Systems;


/// <summary>
/// Clears agent impulses at the start of each update frame
/// Must run before any system that sets AgentImpulse.
/// This makes sure that AgentImpulse is still there during the FixedUpdate
/// </summary>
[Filter(typeof(AgentComponent), typeof(AgentImpulseComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
public class AgentImpulseClearSystem : IUpdateSystem, IFixedUpdateSystem
{
    private bool _fixedTickSinceClear = false;

    public void FixedUpdate(Context context)
    {
        _fixedTickSinceClear = true;
    }

    public void Update(Context context)
    {
        if (!_fixedTickSinceClear)
        {
            // No tick has seen the current impulses yet; keep them.
            return;
        }

        _fixedTickSinceClear = false;

        foreach (Entity e in context.Entities)
        {
            if (!e.GetAgentImpulse().Flags.HasFlag(AgentImpulseFlags.DoNotClear))
            {
                e.RemoveAgentImpulse();
            }
        }
    }
}

[Filter(typeof(AgentComponent))]
[Filter(ContextAccessorFilter.AnyOf, typeof(VelocityComponent), typeof(AgentImpulseComponent))]
[Filter(ContextAccessorFilter.NoneOf, typeof(DisableAgentComponent))]
public class AgentCleanupSystem : IFixedUpdateSystem
{
    public void FixedUpdate(Context context)
    {
        foreach (var e in context.Entities)
        {
            AgentImpulseComponent? impulse = e.TryGetAgentImpulse();

            bool movedByIntent = !e.HasKnockback() &&
                                 !e.HasAgentPause() &&
                                 !e.HasAgentPauseRuntime() &&
                                 (impulse?.Impulse.HasValue() ?? false);

            if (!e.HasKnockback() && !movedByIntent)
            {
                e.SetFriction(e.GetAgent().Friction);
            }
        }
    }
}