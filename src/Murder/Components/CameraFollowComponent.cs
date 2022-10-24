using Bang.Components;

namespace Murder.Components
{
    /// <summary>
    /// Component used by the camera for tracking its target position.
    /// </summary>
    public readonly struct CameraFollowComponent : IComponent
    {
        public readonly bool Enabled = true;
        public CameraFollowComponent() { }
        public CameraFollowComponent(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
