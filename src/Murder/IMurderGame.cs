namespace Murder
{
    /// <summary>
    /// This is the main loop of a murder game. This has the callbacks to relevant events in the game.
    /// </summary>
    public interface IMurderGame
    {
        /// <summary>
        /// Called once, when the executable for the game starts and initializes.
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// This loads all the content within the game. Called after <see cref="Initialize"/>.
        /// </summary>
        public Task LoadContentAsync() => Task.CompletedTask;
        
        /// <summary>
        /// Called after each update.
        /// </summary>
        public void OnUpdate() { }

        /// <summary>
        /// Called after each draw.
        /// </summary>
        public void OnDraw() { }

        /// <summary>
        /// Called before a scene transition.
        /// </summary>
        public void OnSceneTransition() { }

        /// <summary>
        /// Called once the game exits.
        /// </summary>
        public void OnExit() { }
    }
}
