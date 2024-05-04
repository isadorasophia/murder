using Murder.Assets;
using System.Text.Json.Serialization;

namespace Murder.Data;

[Serializable]
public class PreloadPackedGameData
{
    public const string Name = "preload_data.gz";

    public readonly List<GameAsset> Assets;

    [JsonConstructor]
    public PreloadPackedGameData(List<GameAsset> assets)
    {
        Assets = assets;
    }
}
