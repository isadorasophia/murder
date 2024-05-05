using System.Collections.Immutable;

namespace Murder.Data;

/// <summary>
/// This has the data regarding all the sounds that will be loaded in the game.
/// </summary>
[Serializable]
public class PackedSoundData
{
    public const string Name = "sounds.gz";

    /// <summary>
    /// This has all the banks used by the sound engine, sorted by the supported platform, e.g. "Desktop".
    /// </summary>
    public ImmutableDictionary<string, List<string>> Banks { get; init; } = ImmutableDictionary<string, List<string>>.Empty;
    
    public ImmutableArray<string> Plugins { get; init; } = [];
}
