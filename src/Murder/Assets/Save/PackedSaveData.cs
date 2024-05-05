using System.Text.Json.Serialization;

namespace Murder.Assets.Save;

public class SaveDataInfo
{
    public float Version;
    public string Name;

    public SaveDataInfo(float version, string name)
    {
        Version = version;
        Name = name;
    }

    public string GetFullPackedSavePath(int slot) => 
        Path.Join(Game.Data.SaveBasePath, slot.ToString(), PackedSaveData.Name);

    public string GetFullPackedSaveDirectory(int slot) =>
        Path.Join(Game.Data.SaveBasePath, slot.ToString());
}

[Serializable]
public struct SaveDataTracker
{
    public const string Name = "save_data";

    public readonly Dictionary<int, SaveDataInfo> Info = [];

    [JsonConstructor]
    public SaveDataTracker(Dictionary<int, SaveDataInfo> info)
    {
        Info = info;
    }
}

[Serializable]
public class PackedSaveData
{
    public const string Name = "saved.gz";

    public readonly SaveData Data;
    public readonly List<GameAsset> Assets = [];

    [JsonConstructor]
    public PackedSaveData(SaveData data, List<GameAsset> assets)
    {
        Data = data;
        Assets = assets;
    }
}
