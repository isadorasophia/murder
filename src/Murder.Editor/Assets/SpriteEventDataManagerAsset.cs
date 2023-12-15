using Murder.Assets;
using Murder.Diagnostics;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Editor.Assets.Graphics;

public class SpriteEventData
{
    public readonly Dictionary<string, Dictionary<int, string>> Events = new();

    public readonly Dictionary<string, HashSet<int>> DeletedEvents = new();

    public Dictionary<int, string> GetEventsForAnimation(string animation)
    {
        if (!Events.TryGetValue(animation, out var dictionary))
        {
            dictionary = new();
            Events[animation] = dictionary;
        }

        return dictionary;
    }

    public void AddEvent(string animation, int frame, string message)
    {
        Dictionary<int, string> dict = GetEventsForAnimation(animation);
        dict[frame] = message;

        RemoveFromDeletedEvents(animation, frame);
    }

    public void RemoveEvent(string animation, int frame)
    {
        if (!Events.TryGetValue(animation, out var dictionary))
        {
            AddToDeletedEvent(animation, frame);
            return;
        }

        bool hadValueDefined = dictionary.Remove(frame);
        if (!hadValueDefined)
        {
            // Value was previously not tracked here. It is likely added by aseprite.
            // So let's override that!
            AddToDeletedEvent(animation, frame);
        }
    }

    private void AddToDeletedEvent(string animation, int frame)
    {
        if (!DeletedEvents.TryGetValue(animation, out var dictionary))
        {
            dictionary = new();
            DeletedEvents[animation] = dictionary;
        }

        dictionary.Add(frame);
    }

    private bool RemoveFromDeletedEvents(string animation, int frame)
    {
        if (!DeletedEvents.TryGetValue(animation, out var dictionary))
        {
            return false;
        }

        return dictionary.Remove(frame);
    }

    public SpriteEventData() { }
}

[RuntimeOnly]
internal class SpriteEventDataManagerAsset : GameAsset
{
    /// <summary>
    /// Use <see cref="Data.GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => "_Hidden";

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
        if (assets.Count >= 1)
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
