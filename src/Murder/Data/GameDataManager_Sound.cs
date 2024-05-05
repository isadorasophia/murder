using Murder.Diagnostics;

namespace Murder.Data
{
    public partial class GameDataManager
    {
        /// <summary>
        /// This will load all the sounds to the game.
        /// </summary>
        public async Task LoadSoundsAsync(bool reload = false)
        {
            using PerfTimeRecorder recorder = new("Loading Sounds");

            PreprocessSoundFiles();

            string path = Path.Join(PublishedPackedAssetsFullPath, PackedSoundData.Name);
            PackedSoundData? data = File.Exists(path) ? FileManager.UnpackContent<PackedSoundData>(path) : null;

            if (reload)
            {
                await Game.Sound.ReloadAsync();
            }
            else
            {
                await Game.Sound.LoadContentAsync(data);
            }
        }

        /// <summary>
        /// Implemented by custom implementations of data manager that want to do some preprocessing on the sounds.
        /// </summary>
        protected virtual void PreprocessSoundFiles() { }
    }
}