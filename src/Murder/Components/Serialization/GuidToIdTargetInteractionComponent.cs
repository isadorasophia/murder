using Bang.Components;
using Murder.Attributes;

namespace Murder.Editor.Components
{
    /// <summary>
    /// This is a component used to translate entity instaces guid to an actual entity id.
    /// </summary>
    public readonly struct GuidToIdTargetInteractionComponent : IComponent
    {
        /// <summary>
        /// Guid of the target entity.
        /// </summary>
        [InstanceId]
        public readonly Guid Target;
        
        public GuidToIdTargetInteractionComponent(Guid target) => Target = target;
    }
}
