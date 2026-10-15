using Bang;
using Bang.Entities;
using Murder.Diagnostics;
using Murder.Prefabs;
using Murder.Save;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Assets;

/// <summary>
/// Asset for a map that has been generated within a world.
/// </summary>
public class SavedWorld : GameAsset, IWorldAsset
{
    public override string EditorFolder => "Worlds/";
    public override bool IsStoredInSaveData => true;
    public override bool IsSavePacked => true;

    [Serialize]
    private readonly Guid _worldReferenceGuid = Guid.Empty;

    [Serialize]
    public SavedWorldData Data { get; private set; } = new();

    /// <summary>
    /// Whether <see cref="Data"/> was modified and processing is pending.
    /// </summary>
    [Serialize]
    public bool IsProcessingPending { get; private set; } = false;

    public Guid WorldGuid => _worldReferenceGuid;

    public ImmutableArray<EntityInstance> EntityInstances => Data.EntityInstances;
    public int NextEligibleEntityId => Data.NextEligibleEntityId;

    private ImmutableArray<Guid>? _cachedInstanceIds;

    /// <summary>
    /// Used for editor inspection. Returns all the instance ids.
    /// </summary>
    public ImmutableArray<Guid> Instances
    {
        get
        {
            if (_cachedInstanceIds is null)
            {
                var builder = ImmutableArray.CreateBuilder<Guid>();
                foreach (EntityInstance instance in Data.EntityInstances)
                {
                    builder.Add(instance.Guid);
                }

                _cachedInstanceIds = builder.ToImmutable();
            }

            return _cachedInstanceIds.Value;
        }
    }

    public EntityInstance? TryGetInstance(Guid instanceGuid)
    {
        if (Data.EntityIdToInstance.TryGetValue(instanceGuid, out EntityInstance? entity))
        {
            return entity;
        }

        return null;
    }

    internal SavedWorld() { }

    [JsonConstructor]
    public SavedWorld(Guid worldReferenceGuid, SavedWorldData data)
    {
        _worldReferenceGuid = worldReferenceGuid;
        Data = data;
    }

    public void UpdateData(SavedWorldData data)
    {
        Data = data;
        IsProcessingPending = true;
        FileChanged = true;
    }

    protected override void OnModified()
    {
        _cachedInstanceIds = null;
    }
}

public class SavedWorldData
{
    [Serialize]
    public readonly ImmutableArray<EntityInstance> EntityInstances = [];

    [Serialize]
    public readonly int NextEligibleEntityId = 0;

    private ImmutableDictionary<Guid, EntityInstance>? _cacheEntityIdToInstance;

    public ImmutableDictionary<Guid, EntityInstance> EntityIdToInstance
    {
        get
        {
            if (_cacheEntityIdToInstance is null)
            {
                var builder = ImmutableDictionary.CreateBuilder<Guid, EntityInstance>();
                foreach (EntityInstance instance in EntityInstances)
                {
                    builder[instance.Guid] = instance;
                }

                _cacheEntityIdToInstance = builder.ToImmutable();
            }

            return _cacheEntityIdToInstance;
        }
    }

    public SavedWorldData() { }

    [JsonConstructor]
    public SavedWorldData(ImmutableArray<EntityInstance> entityInstances, int nextEligibleEntityId)
    {
        EntityInstances = entityInstances;
        NextEligibleEntityId = nextEligibleEntityId;
    }

    public static ValueTask<SavedWorldData> CreateAsync(World world, ImmutableArray<Entity> entitiesOnSaveRequested)
    {
        using PerfTimeRecorder recorder = new("Creating saved world");

        SavedWorldBuilder builder = new(world);
        return builder.CreateAsync(entitiesOnSaveRequested);
    }
}