using Murder.Diagnostics;

namespace Murder.Core.Sounds
{
    public class SoundPlayer : ISoundPlayer
    {
        public void Initialize(string resourcesPath) { }

        public Task LoadContentAsync() => Task.CompletedTask;

        public Task ReloadAsync() => Task.CompletedTask;

        public void Update() { }

        public ValueTask PlayEvent(SoundEventId _, SoundProperties __)
        {
            GameLogger.Error("Default sound player has been deprecated. If we get back here, actually implement something?");
            return default;
        }

        /// <summary>
        /// Change volume.
        /// </summary>
        public void SetVolume(SoundEventId? _, float volume) { }

        public bool Stop(SoundEventId? id, bool fadeOut) => false;

        public void SetParameter(SoundEventId instance, ParameterId parameter, float value)
        {
        }

        public void SetGlobalParameter(ParameterId parameter, float value)
        {
        }

        public float GetGlobalParameter(ParameterId _) => 0;
    }
}
