using Bang;
using Bang.Components;

namespace Murder.Components
{
    public enum RemoveStyle
    {
        Destroy,
        Deactivate,
        None
    }

    public readonly struct DestroyAtTimeComponent : IComponent
    {
        public readonly RemoveStyle Style = RemoveStyle.Destroy;
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

        public DestroyAtTimeComponent(RemoveStyle style, float timeToDestroy)
        {
            Style = style;
            TimeToDestroy = timeToDestroy;
        }
    }
}