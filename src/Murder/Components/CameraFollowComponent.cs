using Bang.Components;
using Bang.Entities;
using Newtonsoft.Json;

namespace Murder.Components
{
    /// <summary>
    /// Component used by the camera for tracking its target position.
    /// </summary>
    public readonly struct CameraFollowComponent : IComponent
    {
        public readonly bool Enabled = true;

        [JsonIgnore]
        public readonly Entity? SecondaryTarget;

        public CameraFollowComponent() { }
        public CameraFollowComponent(bool enabled)
        {
            Enabled = enabled;
        }
        public CameraFollowComponent(bool enabled, Entity secondaryTarget)
        {
            Enabled = enabled;
            SecondaryTarget = secondaryTarget;
        }
    }
}
