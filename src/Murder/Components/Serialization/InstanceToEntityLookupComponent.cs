using Bang.Components;
using Murder.Attributes;
using Murder.Utilities.Attributes;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Components
{
    /// <summary>
    /// This is used when serializing save data. This keeps a reference between entities and their guid.
    /// </summary>
    [Unique]
    [RuntimeOnly]
    [PersistOnSave]
    public readonly struct InstanceToEntityLookupComponent : IComponent
    {
        /// <summary>
        /// This keeps a map of the instances (guid) to their entities (id).
        /// </summary>
        public readonly ImmutableDictionary<Guid, int> InstancesToEntities = ImmutableDictionary<Guid, int>.Empty;

        /// <summary>
        /// This keeps a map of the instances (guid) to their entities (id).
        /// </summary>
        public readonly ImmutableDictionary<int, Guid> EntitiesToInstances = ImmutableDictionary<int, Guid>.Empty;

        [JsonConstructor]
        public InstanceToEntityLookupComponent() { }

        public InstanceToEntityLookupComponent(IDictionary<Guid, int> instancesToEntities)
        {
            InstancesToEntities = instancesToEntities.ToImmutableDictionary();

            Dictionary<int, Guid> idToGuid = instancesToEntities.ToDictionary(kv => kv.Value, kv => kv.Key);
            EntitiesToInstances = idToGuid.ToImmutableDictionary();
        }
    }
}