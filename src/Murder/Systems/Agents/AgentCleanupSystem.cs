using Bang.Contexts;
using Bang.Systems;
using Murder.Components;

namespace Murder.Systems
{

    [Filter(typeof(AgentImpulseComponent))]
    internal class AgentCleanupSystem : IFixedUpdateSystem
    {
        public ValueTask FixedUpdate(Context context)
        {
            foreach (var e in context.Entities)
            {
                // TODO: Generate extensions
                // e.RemoveAgentImpulse();     // Cleanup the impulse
            }

            return default;
        }
    }
}
