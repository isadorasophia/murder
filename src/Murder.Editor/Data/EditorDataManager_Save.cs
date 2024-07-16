using Murder.Assets;
using Murder.Assets.Save;
using Murder.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace Murder.Editor.Data;

public partial class EditorDataManager
{
    /// <summary>
    /// Optional hook method for notifications when a file is dropped to the game.
    /// </summary>
    /// <param name="path">Target path that has been dropped.</param>
    public void OnFileDrop(string path)
    {
        if (!path.EndsWith(".zip"))
        {
            GameLogger.Log($"We can't really do anything by dropping a file {path}.");
            return;
        }

        // Try to import the save, if this is the case.
        _ = ExtractAndImportSaveZip(path);
    }

    private async ValueTask ExtractAndImportSaveZip(string path)
    {
        int slot = Game.Data.GetNextAvailableSlot();

        // Let's start by extracting to the directory.
        string destinationPath = Path.Join(Game.Data.SaveBasePath, $"{slot}");
        Serialization.FileManager.CreateDirectoryPathIfNotExists(destinationPath);

        using ZipArchive stream = ZipFile.OpenRead(path);
        stream.ExtractToDirectory(destinationPath);

        bool success = await Architect.EditorData.ImportExistingSaveAsync(slot);
        if (!success)
        {
            // Nevermind, undo the directory.
            Directory.Delete(destinationPath, recursive: true);
            return;
        }

        GameLogger.Log($"Successfully imported save on slot {slot}!");
    }

    /// <summary>
    /// Track a save file and serialize the tracker to track this change.
    /// </summary>
    private async ValueTask<bool> ImportExistingSaveAsync(int slot)
    {
        string destinationPath = Path.Join(Game.Data.SaveBasePath, $"{slot}");

        string saveDataPath = Path.Combine(destinationPath, SaveDataInfo.GetFullPackedSavePath(slot));
        if (!File.Exists(saveDataPath))
        {
            GameLogger.Error("Invalid save data file. Zip file is not supported!");
            return false;
        }

        PackedSaveData? packedSaveData = Game.Data.FileManager.UnpackContent<PackedSaveData>(saveDataPath);
        if (packedSaveData is null)
        {
            GameLogger.Error("Unsupported save data file! Undoing the work to extract it.");
            return false;
        }

        SaveData saveData = packedSaveData.Data;

        if (saveData.SaveVersion < _game?.Version)
        {
            GameLogger.Error("Save version is not supported. If we load that, it will very likely break.");
            return false;
        }

        if (saveData.SaveSlot != slot)
        {
            // Cheat our way into modifying the save slot.
            FieldInfo? fSaveSlot = typeof(SaveData).GetField(nameof(SaveData.SaveSlot));
            if (fSaveSlot is null)
            {
                return false;
            }

            fSaveSlot.SetValue(saveData, slot);
        }

        _allSavedData.Add(slot, new SaveDataInfo(saveData.SaveVersion, saveData.SaveName));

        // Immediately serialize the tracker and the save file.
        SaveDataTracker tracker = new(_allSavedData);

        string trackerJson = Serialization.FileManager.SerializeToJson(tracker);
        string packedDataJson = Serialization.FileManager.SerializeToJson(packedSaveData);

        await Task.WhenAll(
            FileManager.PackContentAsync(trackerJson, path: Path.Join(Game.Data.SaveBasePath, SaveDataTracker.Name)),
            FileManager.PackContentAsync(packedDataJson, path: SaveDataInfo.GetFullPackedSavePath(slot)));

        await LoadAllSaveAssets();

        return true;
    }
}