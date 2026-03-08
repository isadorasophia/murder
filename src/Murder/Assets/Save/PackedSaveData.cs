using Bang;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Murder.Assets.Save;

[Serializable]
public class SaveDataInfo
{
    public float Version;
    public string Name;

    public SaveDataInfo(float version, string name)
    {
        Version = version;
        Name = name;
    }

    public static string GetFullPackedSavePath(int slot, string? basepath = null) => 
        Path.Join(basepath ?? Game.Data.SaveBasePath, slot.ToString(), PackedSaveData.Name);

    public static string GetFullPackedAssetsSavePath(int slot, string? basepath = null) =>
        Path.Join(basepath ?? Game.Data.SaveBasePath, slot.ToString(), PackedSaveAssetsData.Name);

    public static string GetFullPackedSaveDirectory(int slot, string? basePath = null) =>
        Path.Join(basePath ?? Game.Data.SaveBasePath, slot.ToString());

    public static string GetFullPackedSaveBackupDirectory(int slot) =>
        Path.Join(Game.Data.SaveBasePath, slot.ToString(), "backup");
}

[Serializable]
public class SaveDataTracker
{
    public const string Name = "save_data.gz";

    [Serialize]
    public ImmutableDictionary<int, SaveDataInfo> Data = ImmutableDictionary<int, SaveDataInfo>.Empty;

    public SaveDataTracker() { }

    public virtual void OnBeforeSave() { }
}

[Serializable]
public class PackedSaveData
{
    public const string Name = "saved.gz";

    public readonly SaveData Data;

    [JsonConstructor]
    public PackedSaveData(SaveData data)
    {
        Data = data;
    }
}

[Serializable]
public class PackedSaveAssetsData
{
    public const string Name = "saved_assets.gz";

    public readonly List<GameAsset> Assets = [];

    [JsonConstructor]
    public PackedSaveAssetsData(List<GameAsset> assets)
    {
        Assets = assets;
    }
}