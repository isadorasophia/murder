using System.Collections.Immutable;

namespace Murder.Assets.Graphics;

internal class SpriteEventDataManagerAsset : GameAsset
{
    public readonly struct SpriteEventData
    {
        public readonly Dictionary<string, Dictionary<int, string>> Events = new();

        public readonly HashSet<string> DeletedEvents = new();

        public SpriteEventData() { }
    }
    
    /// <summary>
    /// Use <see cref="Data.GameDataManager.SKIP_CHAR"/> to hide this in the editor.
    /// </summary>
    public override string EditorFolder => "_Hidden";

    public ImmutableDictionary<Guid, SpriteEventData> Events { get; private set; } = 
        ImmutableDictionary<Guid, SpriteEventData>.Empty;
}
