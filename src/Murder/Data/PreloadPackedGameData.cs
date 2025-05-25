using Murder.Assets;
using System.Text.Json.Serialization;

namespace Murder.Data;

[Serializable]
public class PreloadPackedGameData
{
    public const string Name = "preload_data.gz";

    /// <summary>
    /// The amount of save data that will be fetched.
    /// </summary>
    public readonly int TotalPackedData = 1;

    public readonly List<GameAsset> Assets;

    [JsonConstructor]
    public PreloadPackedGameData(int totalPackedData, List<GameAsset> assets)
    {
        TotalPackedData = totalPackedData;
        Assets = assets;
    }
}
