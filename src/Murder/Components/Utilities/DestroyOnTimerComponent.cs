using Bang.Components;
using Bang;

namespace Murder.Components
{
    public readonly struct DestroyAtTimeComponent : IComponent
    {
        public readonly float TimeToDestroy;

        /// <summary>
        /// Destroy at the end of the frame
        /// </summary>
        public DestroyAtTimeComponent()
        {
            TimeToDestroy = -1;
        }
        
        public DestroyAtTimeComponent(float timeToDestroy)
        {
            TimeToDestroy = timeToDestroy;
        }
    }
}
