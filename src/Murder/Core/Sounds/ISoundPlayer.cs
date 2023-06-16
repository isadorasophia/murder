namespace Murder.Core.Sounds
{
    [Flags]
    public enum SoundProperties
    {
        None = 0,
        Persist,
        SkipIfAlreadyPlaying
    }

    public interface ISoundPlayer
    {
        /// <summary>
        /// This will initialize the fmod libraries, but not load any banks.
        /// </summary>
        /// <param name="resourcesPath">The relative path to the resource binaries.</param>
        public void Initialize(string resourcesPath);

        /// <summary>
        /// This will load the actual bank content asynchonously.
        /// </summary>
        public Task LoadContentAsync();

        /// <summary>
        /// This will reload the content of all the fmod banks in the application.
        /// </summary>
        public Task ReloadAsync();

        public void Update();

        /// <summary>
        /// Play a sound/event with the id of <paramref name="id"/>.
        /// If <paramref name="properties"/> of the sound.
        /// </summary>
        public ValueTask PlayEvent(SoundEventId id, SoundProperties properties);

        public void SetParameter(SoundEventId instance, ParameterId parameter, float value);

        public void SetGlobalParameter(ParameterId parameter, float value);

        /// <summary>
        /// Stop all active streaming events.
        /// If <paramref name="fadeOut"/> is set, this will stop with a fadeout.
        /// </summary>
        public void Stop(bool fadeOut);
        
        /// <summary>
        /// Change volume.
        /// </summary>
        public void SetVolume(SoundEventId? id, float volume);
    }
}
