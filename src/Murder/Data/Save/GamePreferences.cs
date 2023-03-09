using Murder.Assets;
using Murder.Data;
using Murder.Serialization;
using Newtonsoft.Json;

namespace Murder.Save
{
    /// <summary>
    /// Tracks preferences of the current session. This is unique per run.
    /// Used to track the game settings that are not tied to any game run (for example, volume).
    /// </summary>
    public class GamePreferences
    {
        private const string _filename = ".preferences";
        private readonly static string _path = Path.Join(GameDataManager.SaveBasePath, _filename);

        [JsonProperty]
        protected float _soundVolume = 1;

        [JsonProperty]
        protected float _musicVolume = 1;

        protected void SaveSettings()
        {
            FileHelper.SaveSerialized(this, _path, isCompressed: true);
        }

        internal static GamePreferences? TryFetchPreferences()
        {
            if (!FileHelper.FileExists(_path))
            {
                return null;
            }

            return FileHelper.DeserializeGeneric<GamePreferences>(_path)!;
        }

        public float SoundVolume => _soundVolume;

        public float MusicVolume => _musicVolume;

        /// <summary>
        /// This toggles the volume to the opposite of the current setting.
        /// Immediately serialize (and save) afterwards.
        /// </summary>
        public float ToggleSoundVolumeAndSave()
        {
            _soundVolume = _soundVolume == 1 ? 0 : 1;

            OnPreferencesChanged();
            return _soundVolume;
        }

        /// <summary>
        /// This toggles the volume to the opposite of the current setting.
        /// Immediately serialize (and save) afterwards.
        /// </summary>
        public float ToggleMusicVolumeAndSave()
        {
            _musicVolume = _musicVolume == 1 ? 0 : 1;

            OnPreferencesChanged();
            return _musicVolume;
        }

        public void OnPreferencesChanged()
        {
            SaveSettings();
            OnPreferencesChangedImpl();
        }
        
        public virtual void OnPreferencesChangedImpl()
        {
            Game.Sound.SetVolume(default, _soundVolume);
        }
    }
}