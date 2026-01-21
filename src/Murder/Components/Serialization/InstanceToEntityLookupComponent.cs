using Bang.Components;
using Murder.Attributes;
using Murder.Diagnostics;
using Murder.Utilities.Attributes;
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

        public InstanceToEntityLookupComponent() { }

        public InstanceToEntityLookupComponent(IDictionary<Guid, int> instancesToEntities)
        {
            InstancesToEntities = instancesToEntities.ToImmutableDictionary();

            var idToGuidBuilder = ImmutableDictionary.CreateBuilder<int, Guid>();
            foreach ((Guid guid, int entityId) in instancesToEntities)
            {
                if (entityId == -1)
                {
                    // skip invalid entities
                    continue;
                }

                if (idToGuidBuilder.ContainsKey(entityId))
                {
                    GameLogger.Warning($"Somehow, we're adding {entityId} twice from {guid}.");
                }

                idToGuidBuilder[entityId] = guid;
            }

            EntitiesToInstances = idToGuidBuilder.ToImmutable();
        }
    }
}