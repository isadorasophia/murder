using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Components
{
    public readonly struct GuidId
    {
        public readonly string Id = string.Empty;

        [InstanceId]
        public readonly Guid Target = default;

        public GuidId() { }

        public GuidId(string id, Guid target) => (Id, Target) = (id, target);
    }

    /// <summary>
    /// This is a component used to translate entity instaces guid to an actual entity id.
    /// </summary>
    [CustomName(" Guid To ID Collection")]
    [Generates(typeof(IdTargetCollectionComponent))]
    public readonly struct GuidToIdTargetCollectionComponent : IComponent
    {
        /// <summary>
        /// Guid of the target entity.
        /// </summary>
        public readonly ImmutableArray<GuidId> Collection = ImmutableArray<GuidId>.Empty;

        public GuidToIdTargetCollectionComponent() { }

        public Guid? TryFindGuid(string name)
        {
            foreach (GuidId target in Collection)
            {
                if (target.Id.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return target.Target;
                }
            }

            return null;
        }
    }
}