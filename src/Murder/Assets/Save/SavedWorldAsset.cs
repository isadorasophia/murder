using Newtonsoft.Json;
using Bang;
using System.Collections.Immutable;
using Murder.Save;
using Murder.Prefabs;

namespace Murder.Assets
{
    /// <summary>
    /// Asset for a map that has been generated within a world.
    /// </summary>
    public class SavedWorld : GameAsset, IWorldAsset
    {
        public Guid WorldGuid => Guid.Empty;
        
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

        public static SavedWorld Create(World world)
        {
            SavedWorldBuilder builder = new(world);
            return builder.Create();
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