using Bang.Components;
using Bang;

namespace Murder.Components
{
    public readonly struct DestroyAtTimeComponent : IComponent
    {
        public readonly float TimeToDestroy;

        public DestroyAtTimeComponent(float timeToDestroy)
        {
            TimeToDestroy = timeToDestroy;
        }
    }
}
