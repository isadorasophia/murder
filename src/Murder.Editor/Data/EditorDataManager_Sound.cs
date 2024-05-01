using Murder.Diagnostics;
using Murder.Serialization;

namespace Murder.Editor.Data
{
    public partial class EditorDataManager
    {
        protected override void PreprocessSoundFiles()
        {
            string fullRawResourcesPath = FileHelper.GetPath(EditorSettings.RawResourcesPath);
            if (!Directory.Exists(fullRawResourcesPath))
            {
                GameLogger.Log($"Unable to find raw resources path at {FileHelper.GetPath(EditorSettings.RawResourcesPath)}. " +
                    $"Use this directory for loading sound assets.");
                return;
            }

            string soundsRawResourcesPath = Path.Join(fullRawResourcesPath, GameProfile.SoundsPath);
            if (!Directory.Exists(soundsRawResourcesPath))
            {
                // No sounds found, just go away!
                return;
            }

            string soundsPackedPath = FileHelper.GetPath(Path.Join(EditorSettings.SourcePackedPath, GameProfile.SoundsPath));
            string soundsBinPath = FileHelper.GetPath(Path.Join(EditorSettings.BinResourcesPath, GameProfile.SoundsPath));

            FileHelper.DeleteDirectoryIfExists(soundsBinPath);
            FileHelper.DeleteDirectoryIfExists(soundsPackedPath);

            // Make sure we are copying the latest contents into packed and binary directories!
            EditorFileHelper.DirectoryDeepCopy(soundsRawResourcesPath, soundsPackedPath);
            EditorFileHelper.DirectoryDeepCopy(soundsRawResourcesPath, soundsBinPath);
        }
    }
}