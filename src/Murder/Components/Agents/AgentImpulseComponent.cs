using Bang.Components;
using Murder.Core.Geometry;

namespace Murder.Components
{
    public readonly struct AgentImpulseComponent : IComponent
    {
        public readonly Vector2 Impulse;

        public AgentImpulseComponent(Vector2 impulse)
        {
            Impulse = impulse;
        }
    }
}
