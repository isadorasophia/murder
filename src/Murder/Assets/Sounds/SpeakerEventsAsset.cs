using Bang;
using Murder.Components;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Assets.Sounds;

public class SpeakerEventsAsset : GameAsset
{
    public override string EditorFolder => "#\uf7a6Sounds\\#\uf2c1Speakers";

    public override char Icon => '\uf7a6';

    public override System.Numerics.Vector4 EditorColor => new(0.5f, 1, 0.2f, 1);

    [Folder]
    [Serialize]
    private ImmutableArray<SpriteEventInfo> _emotes = [];

    [JsonConstructor]
    public SpeakerEventsAsset(ImmutableArray<SpriteEventInfo> emotes) => _emotes = emotes;

    private ImmutableDictionary<string, SpriteEventInfo>? _cache = null;

    public ImmutableDictionary<string, SpriteEventInfo> Events
    {
        get
        {
            if (_cache is not null)
            {
                return _cache;
            }

            _cache = WorldHelper.ToEventListener(_emotes);
            return _cache;
        }
    }

    /// <summary>
    /// Used by editor tools.
    /// </summary>
    public ImmutableArray<SpriteEventInfo> FetchEvents()
    {
        return _emotes;
    }

    /// <summary>
    /// Used by editor tools.
    /// </summary>
    public void ReplaceEvents(ImmutableArray<SpriteEventInfo> emotes)
    {
        _emotes = emotes;
        FileChanged = true;
    }

    protected override void OnModified()
    {
        _cache = null;
    }
}
