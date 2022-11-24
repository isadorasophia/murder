namespace Murder.Attributes
{
    /// <summary>
    /// This is an attribute used for a field guid that point to a game asset id.
    /// </summary>
    public class GameAssetIdAttribute : Attribute
    {
        /// <summary>
        /// The type of the game asset.
        /// </summary>
        public readonly Type AssetType;

        /// <summary>
        /// Creates a new <see cref="GameAssetIdAttribute"/>.
        /// </summary>
        /// <param name="type">The game asset type.</param>
        public GameAssetIdAttribute(Type type) => AssetType = type;
    }
    
}
