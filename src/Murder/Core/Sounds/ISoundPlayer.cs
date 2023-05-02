namespace Murder.Core.Sounds
{
    public interface ISoundPlayer
    {
        public void Initialize(string resourcesPath);
        
        public void Update();

        /// <summary>
        /// Play a sound/event with the id of <paramref name="id"/>.
        /// If <paramref name="isLoop"/> is set, the sound will be persisted.
        /// </summary>
        public ValueTask PlayEvent(SoundEventId id, bool isLoop, bool stopLastMusic = true);
        
        /// <summary>
        /// Start a streaming sound/event in the background.
        /// This is called for music or ambience sounds.
        /// </summary>
        public ValueTask PlayStreaming(SoundEventId id, bool stopLastMusic = true);

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
