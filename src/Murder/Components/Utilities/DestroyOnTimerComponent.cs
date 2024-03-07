using Bang;
using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    public enum RemoveStyle
    {
        Destroy,
        Deactivate,
        RemoveComponents,
        None,
    }

    [RuntimeOnly, DoNotPersistOnSave]
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