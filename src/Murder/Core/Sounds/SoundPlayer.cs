using Murder.Data;
using Murder.Diagnostics;
using System.Collections.Immutable;

namespace Murder.Core.Sounds
{
    public class SoundPlayer : ISoundPlayer
    {
        public void Initialize(string resourcesPath) { }

        public Task LoadContentAsync(PackedSoundData? packedData) => Task.CompletedTask;

        public Task ReloadAsync() => Task.CompletedTask;

        public void UpdateListener(SoundSpatialAttributes attributes) { }

        public void Update() { }

        public ValueTask PlayEvent(SoundEventId _, PlayEventInfo properties)
        {
            GameLogger.Error("Default sound player has been deprecated. If we get back here, actually implement something?");
            return default;
        }

        public bool UpdateEvent(SoundEventId id, SoundSpatialAttributes attributes) => false;

        /// <summary>
        /// Change volume.
        /// </summary>
        public void SetVolume(SoundEventId? _, float volume) { }

        public bool Stop(SoundEventId? id, bool fadeOut) => false;

        public bool Stop(SoundLayer layer, bool fadeOut) => false;

        public bool Resume(SoundLayer layer) => false;

        public bool Pause(SoundLayer layer) => false;

        public void SetParameter(SoundEventId instance, ParameterId parameter, float value)
        {
        }

        public void SetGlobalParameter(ParameterId parameter, float value)
        {
        }

        public float GetGlobalParameter(ParameterId _) => 0;

        public ImmutableDictionary<string, List<string>> FetchAllBanks() => ImmutableDictionary<string, List<string>>.Empty;

        public ImmutableArray<string> FetchAllPlugins() => [];
    }
}