namespace Murder.Core.Sounds
{
    [Flags]
    public enum SoundProperties
    {
        None = 0,
        Persist = 0b1,
        SkipIfAlreadyPlaying = 0b10,
        StopOtherMusic = 0b100
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

        /// <summary>
        /// Update listener information (e.g. position and facing).
        /// </summary>
        /// <param name="attributes">Attributes of the origin of this sound.</param>
        public void UpdateListener(SoundSpatialAttributes attributes);

        /// <summary>
        /// Update spatial attributes for a specific event instance.
        /// </summary>
        /// <param name="id">Sound event instance id.</param>
        /// <param name="attributes">Attributes of the origin of this sound.</param>
        /// <returns>
        /// False if no event instance is found in the world, or something failed.
        /// </returns>
        public bool UpdateEvent(SoundEventId id, SoundSpatialAttributes attributes);

        public void Update();

        /// <summary>
        /// Play a sound/event with the id of <paramref name="id"/>.
        /// If <paramref name="properties"/> of the sound.
        /// </summary>
        public ValueTask PlayEvent(SoundEventId id, SoundProperties properties, SoundSpatialAttributes? attributes);

        public void SetParameter(SoundEventId instance, ParameterId parameter, float value);

        public void SetGlobalParameter(ParameterId parameter, float value);

        public float GetGlobalParameter(ParameterId parameter);

        /// <summary>
        /// Stop a specific sound event id.
        /// If <paramref name="fadeOut"/> is set, this will stop with a fadeout.
        /// </summary>
        /// <param name="id">Whether it should stop all events or only a specific one.</param>
        /// <param name="fadeOut">Apply fadeout on stop.</param>
        public bool Stop(SoundEventId? id, bool fadeOut);

        /// <summary>
        /// Stop all active streaming events.
        /// If <paramref name="fadeOut"/> is set, this will stop with a fadeout.
        /// </summary>
        public bool Stop(bool fadeOut, out SoundEventId[] stoppedEvents);

        public bool Stop(bool fadeOut, HashSet<SoundEventId> exceptForSoundsList, out SoundEventId[] stoppedEvents)
        {
            stoppedEvents = [];
            return false;
        }

        /// <summary>
        /// Change volume.
        /// </summary>
        public void SetVolume(SoundEventId? id, float volume);
    }
}