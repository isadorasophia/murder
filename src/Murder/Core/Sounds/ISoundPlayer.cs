namespace Murder.Core.Sounds
{
    public interface ISoundPlayer
    {
        public void Initialize(string resourcesPath);
        
        public void Update();

        /// <summary>
        /// Play a sound/event with the name of <paramref name="name"/>.
        /// If <paramref name="isLoop"/> is set, the sound will be persisted.
        /// </summary>
        public ValueTask PlayEvent(string name, bool isLoop);

        /// <summary>
        /// Start a streaming sound/event in the background.
        /// This is called for music or ambience sounds.
        /// </summary>
        public ValueTask PlayStreaming(string name);

        /// <summary>
        /// Stop all active streaming events.
        /// If <paramref name="fadeOut"/> is set, this will stop with a fadeout.
        /// </summary>
        public void Stop(bool fadeOut);
    }
}
