using Bang;
using Murder.Assets;
using Murder.Attributes;
using Murder.Data;

using System.Collections.Immutable;

namespace Murder.Editor.Assets;

[HideInEditor] // This is created by the engine and should never be actually exposed to the UI.
public class SpriteEventDataManagerAsset : GameAsset
{
    /// <summary>
    /// Use <see cref="Data.GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => GameDataManager.HiddenAssetsRelativePath;

    [Serialize]
    public ImmutableDictionary<Guid, SpriteEventData> Events { get; private set; } = 
        ImmutableDictionary<Guid, SpriteEventData>.Empty;

    public SpriteEventDataManagerAsset() { }

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
}
