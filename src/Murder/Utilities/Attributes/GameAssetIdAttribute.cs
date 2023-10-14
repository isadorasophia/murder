namespace Murder.Attributes
{
    public readonly struct GameAssetIdInfo
    {
        /// <summary>
        /// The type of the game asset.
        /// </summary>
        public readonly Type AssetType;

        /// <summary>
        /// Whether it should look for all assets that inherit from this asset.
        /// </summary>
        public readonly bool AllowInheritance = true;

        public GameAssetIdInfo(Type t, bool allowInheritance) => (AssetType, AllowInheritance) = (t, allowInheritance);
    }

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
        /// Whether it should look for all assets that inherit from this asset.
        /// </summary>
        public readonly bool AllowInheritance = false;

        /// <summary>
        /// Creates a new <see cref="GameAssetIdAttribute"/>.
        /// </summary>
        /// <param name="type">The game asset type.</param>
        /// <param name="allowInheritance">Whether it should look for all assets that inherit from this asset.</param>
        public GameAssetIdAttribute(Type type, bool allowInheritance = false) => (AssetType, AllowInheritance) = (type, allowInheritance);
    }

    public class GameAssetIdAttribute<T> : GameAssetIdAttribute
    {
        public GameAssetIdAttribute() : base(typeof(T))
        {
        }
    }
}