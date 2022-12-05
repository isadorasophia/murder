namespace Murder
{
    /// <summary>
    /// A game that leverages murder and use custom shaders should implement this in their <see cref="IMurderGame"/>.
    /// </summary>
    public interface IShaderProvider
    {
        /// <summary>
        /// Names of custom shaders that will be provided.
        /// This is expected to be placed in ./<game_directory/>/../resources
        /// </summary>
        public string[] Shaders { get; }
    }
}
