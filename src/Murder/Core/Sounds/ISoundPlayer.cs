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
        public void Initialize(string resourcesPath);
        
        public void Update();

        /// <summary>
        /// Play a sound/event with the id of <paramref name="id"/>.
        /// If <paramref name="properties"/> of the sound.
        /// </summary>
        public ValueTask PlayEvent(SoundEventId id, SoundProperties properties);
        
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
