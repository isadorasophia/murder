using Murder.Assets;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Data;

[Serializable]
public class PackedGameData
{
    public const string Name = "data.gz";

    public readonly List<GameAsset> Assets;

    public ImmutableArray<string> TexturesNoAtlasPath { get; init; } = [];
    public ImmutableArray<string> SoundDataPath { get; init; } = [];

    [JsonConstructor]
    public PackedGameData(List<GameAsset> assets)
    {
        Assets = assets;
    }
}