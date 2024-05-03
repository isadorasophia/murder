using Murder.Assets;
using System.Text.Json.Serialization;

namespace Murder.Data;

[Serializable]
public class PackedGameData
{
    public readonly List<GameAsset> PreloadAssets;
    public readonly List<GameAsset> Assets;

    [JsonConstructor]
    public PackedGameData(List<GameAsset> preloadAssets, List<GameAsset> assets)
    {
        PreloadAssets = preloadAssets;
        Assets = assets;
    }
}
