using Bang.Components;
using Bang.Entities;
using Newtonsoft.Json;

namespace Murder.Components
{
    /// <summary>
    /// Component used by the camera for tracking its target position.
    /// </summary>
    [Unique]
    public readonly struct CameraFollowComponent : IComponent
    {
        public readonly bool Enabled = true;

        [JsonIgnore]
        public readonly Entity? SecondaryTarget;

        /// <summary>
        /// Force to centralize the camera without a dead zone.
        /// </summary>
        public readonly bool ForceCenter = false;

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

        public CameraFollowComponent(bool enabled, bool forceCenter)
        {
            Enabled = enabled;
            ForceCenter = forceCenter;
        }
    }
}
