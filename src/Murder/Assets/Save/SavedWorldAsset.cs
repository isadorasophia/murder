using Bang;
using Bang.Entities;
using Murder.Diagnostics;
using Murder.Prefabs;
using Murder.Save;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Assets
{
    /// <summary>
    /// Asset for a map that has been generated within a world.
    /// </summary>
    public class SavedWorld : GameAsset, IWorldAsset
    {
        public Guid WorldGuid => Guid.Empty;

        public override bool IsStoredInSaveData => true;
        public override bool IsSavePacked => true;

        [Bang.Serialize]
        private readonly ImmutableDictionary<Guid, EntityInstance> _instances;

        private ImmutableArray<EntityInstance>? _cachedInstances;

        internal ImmutableArray<EntityInstance> FetchInstances()
        {
            _cachedInstances ??= _instances.Values.ToImmutableArray();
            return _cachedInstances.Value;
        }

        public override string EditorFolder => "Worlds/";

        internal SavedWorld() => _instances = ImmutableDictionary<Guid, EntityInstance>.Empty;

        [JsonConstructor]
        internal SavedWorld(ImmutableDictionary<Guid, EntityInstance> instances) => _instances = instances;

        public static ValueTask<SavedWorld> CreateAsync(World world, ImmutableArray<Entity> entitiesOnSaveRequested)
        {
            using PerfTimeRecorder recorder = new("Creating saved world");

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