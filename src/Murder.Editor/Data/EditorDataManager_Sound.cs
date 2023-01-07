using Murder.Diagnostics;
using Murder.Serialization;

namespace Murder.Editor.Data
{
    public partial class EditorDataManager
    {
        protected override void PreprocessSoundFiles()
        {
            if (!Directory.Exists(EditorSettings.RawResourcesPath))
            {
                GameLogger.Log($"Unable to find raw resources path at {FileHelper.GetPath(EditorSettings.RawResourcesPath)}. " +
                    $"Use this directory for loading sound assets.");
                return;
            }

            string soundsRawResourcesPath = FileHelper.GetPath(Path.Join(EditorSettings.RawResourcesPath, GameProfile.SoundsPath));
            if (!Directory.Exists(soundsRawResourcesPath))
            {
                // No sounds found, just go away!
                return;
            }

            string soundsPackedPath = FileHelper.GetPath(Path.Join(EditorSettings.SourcePackedPath, GameProfile.SoundsPath));
            string soundsBinPath = FileHelper.GetPath(Path.Join(EditorSettings.BinResourcesPath, GameProfile.SoundsPath));

            // Make sure we are copying the latest contents into packed and binary directories!
            FileHelper.DirectoryDeepCopy(soundsRawResourcesPath, soundsPackedPath);
            FileHelper.DirectoryDeepCopy(soundsRawResourcesPath, soundsBinPath);
        }
    }
}