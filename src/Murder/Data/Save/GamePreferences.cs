using Bang;
using Murder.Assets.Localization;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Text.Json;

namespace Murder.Save
{
    public enum ScalingKind
    {
        Auto = 0,
        Large = 1,
        OneX = 2,
        TwoX = 3,
        ThreeX = 4,
        Snap = 5
    }

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
        public float AllVolume { get; protected set; } = .8f;

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
        public ScalingKind Scaling { get; private set; } = ScalingKind.Auto;

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

        public void SetScalingKind(ScalingKind scaling)
        {
            if (scaling == Scaling)
            {
                return;
            }

            Scaling = scaling;

            Point minimumSize = new(Game.DefaultWidth, Game.DefaultHeight);
            switch (scaling)
            {
                case ScalingKind.Auto:
                    minimumSize = new(Game.DefaultWidth * 2, Game.DefaultHeight * 2);
                    break;
                case ScalingKind.Large:
                    minimumSize = new(1280, 720);
                    break;
                case ScalingKind.OneX:
                    minimumSize = new(Game.DefaultWidth, Game.DefaultHeight);
                    break;
                case ScalingKind.TwoX:
                    minimumSize = new(Game.DefaultWidth * 2, Game.DefaultHeight * 2);
                    break;
                case ScalingKind.ThreeX:
                    minimumSize = new(Game.DefaultWidth * 3, Game.DefaultHeight * 3);
                    break;
            }

            // Clamp window size to the minimum required by the scaling.
            var currentSize = Game.Instance.Window.ClientBounds.Size();
            Point windowSize = new(
                Math.Max(currentSize.X, minimumSize.X),
                Math.Max(currentSize.Y, minimumSize.Y));

            Game.Instance.OnWindowChange(
                new WindowChangeNotification(ScreenUpdatedKind.ScalePreferenceModified)
                {
                    ApplyToSettings = new WindowChangeSettings(windowSize)
                    {
                        Force = true
                    }
                });

            OnPreferencesChanged();
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