using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is a component used to translate entity instaces guid to an actual entity id.
    /// Gets translated to <see cref="IdTargetComponent"/>.
    /// </summary>
    /// 
    [CustomName(" Guid To ID")]
    public readonly struct GuidToIdTargetComponent : IComponent
    {
        /// <summary>
        /// Guid of the target entity.
        /// </summary>
        [InstanceId]
        public readonly Guid Target;

        public GuidToIdTargetComponent(Guid target) => Target = target;
    }
}