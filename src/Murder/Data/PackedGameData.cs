using Murder.Assets;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Data;

/// <summary>
/// This has the data regarding all assets and textures loaded in the game.
/// </summary>
[Serializable]
public class PackedGameData
{
    public const string Name = "data{0}.gz";

    public readonly List<GameAsset> Assets;

    public ImmutableArray<string> TexturesNoAtlasPath { get; init; } = [];

    [JsonConstructor]
    public PackedGameData(List<GameAsset> assets)
    {
        Assets = assets;
    }
}