using Murder.Assets;
using Murder.Assets.Localization;
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

        [JsonProperty]
        protected bool _bloom = false;

        [JsonProperty]
        protected bool _downscale = false;

        [JsonProperty]
        protected LanguageId _language = LanguageId.English;

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

        public LanguageId Language => _language;

        /// <summary>
        /// This toggles the volume to the opposite of the current setting.
        /// Immediately serialize (and save) afterwards.
        /// </summary>
        public float ToggleSoundVolumeAndSave()
        {
            return SetSoundVolume(_soundVolume == 1 ? 0 : 1);
        }

        public float SetSoundVolume(float value)
        {
            _soundVolume = value;

            OnPreferencesChanged();
            return _soundVolume;
        }

        /// <summary>
        /// This toggles the volume to the opposite of the current setting.
        /// Immediately serialize (and save) afterwards.
        /// </summary>
        public float ToggleMusicVolumeAndSave()
        {
            return SetMusicVolume(_musicVolume == 1 ? 0 : 1);
        }

        public float SetMusicVolume(float value)
        {
            _musicVolume = value;

            OnPreferencesChanged();
            return _musicVolume;
        }

        public bool ToggleBloomAndSave()
        {
            _bloom = !_bloom;
            OnPreferencesChanged();
            return _bloom;
        }

        public bool ToggleDownscaleAndSave()
        {
            _downscale = !_downscale;
            OnPreferencesChanged();
            return _downscale;
        }

        public void SetLanguage(LanguageId id)
        {
            _language = id;

            OnPreferencesChanged();
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