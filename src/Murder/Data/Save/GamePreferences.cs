using Bang;
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

        [Serialize]
        public LanguageId Language { get; protected set; } = LanguageId.English;

        [Serialize]
        public float AllVolume { get; protected set; } = .5f;

        [Serialize]
        public float SoundVolume { get; protected set; } = 1;

        [Serialize]
        public float MusicVolume { get; protected set; } = 1;

        [Serialize]
        public bool Fullscreen { get; protected set; } = true;

        [Serialize]
        public float Haptics { get; private set; } = 1;

        [Serialize]
        public float ScreenShake { get; private set; } = 1;

        [Serialize]
        public ImmutableArray<ButtonBindingsInfo> ButtonBindingsInfos { get; private set; } = [];

        [Serialize]
        public ImmutableArray<AxisBindingsInfo> AxisBindingsInfos { get; private set; } = [];

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

        public void SetButtonBindingsInfos(ImmutableArray<ButtonBindingsInfo> buttonBindingsInfos)
        {
            ButtonBindingsInfos = buttonBindingsInfos;
            OnPreferencesChanged();
        }

        public float SetAllVolume(float value)
        {
            AllVolume = value;
            OnPreferencesChanged();

            return value;
        }

        public float SetSoundVolume(float value)
        {
            SoundVolume = value;
            OnPreferencesChanged();

            return value;
        }

        public float SetMusicVolume(float value)
        {
            MusicVolume = value;
            OnPreferencesChanged();

            return value;
        }

        public void SetLanguage(LanguageId id)
        {
            if (Language == id)
            {
                return;
            }

            Language = id;
            OnPreferencesChanged();
        }

        public bool SetFullScreen(bool value)
        {
            Fullscreen = value;
            OnPreferencesChanged();

            return Fullscreen;
        }

        public float SetHapticsMultiplier(float value)
        {
            Haptics = value;
            OnPreferencesChanged();

            return value;
        }

        public float SetScreenShakeMultiplier(float value)
        {
            ScreenShake = value;
            OnPreferencesChanged();

            return value;
        }

        public void OnPreferencesChanged()
        {
            OnPreferencesChangedImpl();
            SaveSettings();
        }

        protected void SaveSettings()
        {
            Game.Data.FileManager.SaveSerialized(this, _path);
        }

        public virtual void OnPreferencesChangedImpl() { }
    }
}