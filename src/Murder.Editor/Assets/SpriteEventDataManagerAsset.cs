using Murder.Assets;
using Murder.Attributes;
using Murder.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Editor.Assets;

[HideInEditor] // This is created by the engine and should never be actually exposed to the UI.
internal class SpriteEventDataManagerAsset : GameAsset
{
    /// <summary>
    /// Use <see cref="Data.GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => "_Hidden";

    [JsonProperty]
    public ImmutableDictionary<Guid, SpriteEventData> Events { get; private set; } = 
        ImmutableDictionary<Guid, SpriteEventData>.Empty;

    /// <summary>
    /// Delete tracking of a particular sprite.
    /// </summary>
    public bool DeleteSprite(Guid spriteId)
    {
        if (Events.ContainsKey(spriteId)) 
        {
            Events = Events.Remove(spriteId);
            return true;
        }

        return false;
    }
        
    public SpriteEventData GetOrCreate(Guid spriteId)
    {
        if (Events.TryGetValue(spriteId, out SpriteEventData? spriteEventData))
        {
            return spriteEventData;
        }

        spriteEventData = new();
        Events = Events.SetItem(spriteId, spriteEventData);

        return spriteEventData;
    }

    private static SpriteEventDataManagerAsset? _instance = null;

    public static SpriteEventDataManagerAsset? TryGet()
    {
        if (_instance is not null)
        {
            return _instance;
        }

        ImmutableDictionary<Guid, GameAsset> assets = Architect.EditorData.FilterAllAssets(typeof(SpriteEventDataManagerAsset));
        if (assets.Count > 1)
        {
            GameLogger.Warning("How did we end up with more than one manager assets?");
        }

        foreach ((_, GameAsset asset) in assets)
        {
            if (asset is SpriteEventDataManagerAsset manager)
            {
                _instance = manager;
                return _instance;
            }
        }

        return null;
    }

    public static SpriteEventDataManagerAsset GetOrCreate()
    {
        if (TryGet() is SpriteEventDataManagerAsset manager)
        {
            return manager;
        }

        // Otherwise, this means we need to actually create one...
        _instance = new();
        _instance.Name = "_EventManager";

        Architect.EditorData.SaveAsset(_instance);

        return _instance;
    }
}
