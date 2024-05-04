using System.Text.Json.Serialization;

namespace Murder.Assets.Save;

public readonly struct SaveDataInfo
{
    public readonly string Name;
    public readonly int Version;

    public readonly string RelativePath;
}

[Serializable]
public class PackedSaveDataTracker
{
    public readonly Dictionary<int, SaveDataInfo> Info = [];

    [JsonConstructor]
    public PackedSaveDataTracker(Dictionary<int, SaveDataInfo> info)
    {
        Info = info;
    }
}

[Serializable]
public class PackedSaveData
{
    public readonly SaveData Data;
    public readonly List<GameAsset> Assets = [];

    [JsonConstructor]
    public PackedSaveData(SaveData data, List<GameAsset> assets)
    {
        Data = data;
        Assets = assets;
    }
}
