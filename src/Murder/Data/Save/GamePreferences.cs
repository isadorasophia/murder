using Murder.Assets.Input;
using Murder.Assets.Localization;
using Murder.Core.Input;
using System.Collections.Immutable;
using System.Text.Json;

namespace Murder.Save
{
    /// <summary>
    /// Tracks preferences of the current session. This is unique per run.
    /// Used to track the game settings that are not tied to any game run (for example, volume).
    /// </summary>
    [Serializable]
    public class GamePreferences
    {
        private const string _filename = ".preferences";
        private readonly static string _path = Path.Join(Game.Data.SaveBasePath, _filename);

        [Bang.Serialize]
        protected float _soundVolume = 1;

        [Bang.Serialize]
        protected float _musicVolume = 1;

        [Bang.Serialize]
        protected bool _showFps = false;

        [Bang.Serialize]
        protected LanguageId _language = LanguageId.English;

        [Bang.Serialize]
        protected bool _fullscreen = true;

        [Bang.Serialize]
        protected ImmutableArray<ButtonBindingsInfo> buttonBindingsInfos = ImmutableArray<ButtonBindingsInfo>.Empty;

        [Bang.Serialize]
        protected ImmutableArray<AxisBindingsInfo> axisBindingsInfos = ImmutableArray<AxisBindingsInfo>.Empty;

        public ImmutableArray<ButtonBindingsInfo> ButtonBindingsInfos => buttonBindingsInfos;
        public ImmutableArray<AxisBindingsInfo> AxisBindingsInfos => axisBindingsInfos;

        protected void SaveSettings()
        {
            Game.Data.FileManager.SaveSerialized(this, _path);
        }

        internal static GamePreferences? TryFetchPreferences()
        {
            if (!File.Exists(_path))
            {
                return null;
            }

            try
            {
                return Game.Data.FileManager.DeserializeGeneric<GamePreferences>(_path);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        internal static void ResetPreferences()
        {
            Game.Data.FileManager.DeleteFileIfExists(_path);
        }

        public bool FullScreen => _fullscreen;

        public float SoundVolume => _soundVolume;

        public float MusicVolume => _musicVolume;

        public LanguageId Language => _language;

        public bool ShowFps => _showFps;

        public void SetButtonBindingsInfos(ImmutableArray<ButtonBindingsInfo> buttonBindingsInfos)
        {
            this.buttonBindingsInfos = buttonBindingsInfos;
            OnPreferencesChanged();
        }

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

        public bool ToggleShowFps()
        {
            SetShowFps(!_showFps);
            return _showFps;
        }

        public float SetMusicVolume(float value)
        {
            _musicVolume = value;

            OnPreferencesChanged();
            return _musicVolume;
        }

        public void SetLanguage(LanguageId id)
        {
            if (_language == id)
            {
                return;
            }

            _language = id;
            OnPreferencesChanged();
        }

        public bool SetFullScreen(bool value)
        {
            _fullscreen = value;
            OnPreferencesChanged();

            return _fullscreen;
        }

        private void SetShowFps(bool showFps)
        {
            if (_showFps == showFps)
            {
                return;
            }

            _showFps = showFps;
            OnPreferencesChanged();
        }

        public void OnPreferencesChanged()
        {
            OnPreferencesChangedImpl();
            SaveSettings();
        }

        public virtual void OnPreferencesChangedImpl()
        {
            Game.Sound.SetVolume(default, _soundVolume);
        }
    }
}