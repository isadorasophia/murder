using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Components
{
    /// <summary>
    /// This is a component used to translate entity instaces guid to an actual entity id.
    /// </summary>
    /// 
    [CustomName(" Guid To ID Collection")]
    public readonly struct GuidToIdTargetCollectionComponent : IComponent
    {
        /// <summary>
        /// Guid of the target entity.
        /// </summary>
        [InstanceId]
        public readonly ImmutableDictionary<string, Guid> Targets = ImmutableDictionary<string, Guid>.Empty;

        public GuidToIdTargetCollectionComponent() { }
    }
}
