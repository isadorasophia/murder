using Murder.Diagnostics;

namespace Murder.Data
{
    public partial class GameDataManager
    {
        /// <summary>
        /// This will load all the sounds to the game.
        /// </summary>
        public async ValueTask LoadSounds(bool reload = false)
        {
            using PerfTimeRecorder recorder = new("Loading Sounds");

            PreprocessSoundFiles();

            if (reload)
            {
                await Game.Sound.ReloadAsync();
            }
            else
            {
                await Game.Sound.LoadContentAsync();
            }
        }

        /// <summary>
        /// Implemented by custom implementations of data manager that want to do some preprocessing on the sounds.
        /// </summary>
        protected virtual void PreprocessSoundFiles() { }
    }
}