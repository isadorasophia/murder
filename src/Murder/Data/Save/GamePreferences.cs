using Murder.Data;
using Murder.Serialization;
using Newtonsoft.Json;

namespace Murder.Save
{
    /// <summary>
    /// Tracks preferences of the current session. This is unique per run.
    /// </summary>
    public class GamePreferences
    {
        private const string _filename = ".preferences";

        private readonly static string _path = Path.Join(GameDataManager.SaveBasePath, _filename);

        public GamePreferences() { }

        [JsonProperty]
        private float _downsample;

        [JsonIgnore]
        public float Downsample
        {
            get => _downsample;
            set
            {
                _downsample = value;
                SaveSettings();
            }
        }
        
        [JsonProperty]
        private float _soundVolume = 1;

        [JsonIgnore]
        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = value;
                SaveSettings();
            }
        }
        
        private void SaveSettings() 
        {
            FileHelper.SaveSerialized(this, _path, isCompressed: true);
        }
        
        internal static GamePreferences FetchOrCreate()
        {
            if (!FileHelper.FileExists(_path))
            {
                return new();
            }

            return FileHelper.DeserializeGeneric<GamePreferences>(_path)!;
        }
    }
}