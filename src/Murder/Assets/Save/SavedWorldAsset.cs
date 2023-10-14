using Bang;
using Bang.Entities;
using Murder.Prefabs;
using Murder.Save;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Assets
{
    /// <summary>
    /// Asset for a map that has been generated within a world.
    /// </summary>
    public class SavedWorld : GameAsset, IWorldAsset
    {
        public Guid WorldGuid => Guid.Empty;

        public override bool IsStoredInSaveData => true;

        [JsonProperty]
        private readonly ImmutableDictionary<Guid, EntityInstance> _instances;

        private ImmutableArray<EntityInstance>? _cachedInstances;

        internal ImmutableArray<EntityInstance> FetchInstances()
        {
            _cachedInstances ??= _instances.Values.ToImmutableArray();
            return _cachedInstances.Value;
        }

        public override string EditorFolder => "Worlds/";

        internal SavedWorld() => _instances = ImmutableDictionary<Guid, EntityInstance>.Empty;

        internal SavedWorld(ImmutableDictionary<Guid, EntityInstance> instances) => _instances = instances;

        public static ValueTask<SavedWorld> CreateAsync(World world, ImmutableArray<Entity> entitiesOnSaveRequested)
        {
            SavedWorldBuilder builder = new(world);
            return builder.CreateAsync(entitiesOnSaveRequested);
        }

        public ImmutableArray<Guid> Instances => _instances.Keys.ToImmutableArray();

        public EntityInstance? TryGetInstance(Guid instanceGuid)
        {
            if (_instances.TryGetValue(instanceGuid, out var entity))
            {
                return entity;
            }

            return null;
        }

    }
}