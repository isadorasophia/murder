using Bang.Components;
using Murder.Utilities;

namespace Murder.Components
{
    public readonly struct LastSeenComponent : IComponent
    {
        public readonly float SeenTime;

        public LastSeenComponent()
        {
            SeenTime = Time.Elapsed;
        }

        public LastSeenComponent(float time)
        {
            SeenTime = time;
        }
    }
}
