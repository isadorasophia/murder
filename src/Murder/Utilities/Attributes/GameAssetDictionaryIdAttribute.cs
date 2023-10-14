namespace Murder.Attributes
{
    /// <summary>
    /// This is an attribute used for a dictionary with a guid on both the key and values.
    /// </summary>
    public class GameAssetDictionaryIdAttribute : Attribute
    {
        /// <summary>
        /// The type of the game asset key.
        /// </summary>
        public readonly Type Key;

        /// <summary>
        /// The type of the game asset value.
        /// </summary>
        public readonly Type Value;

        /// <summary>
        /// Creates a new <see cref="GameAssetDictionaryIdAttribute"/>.
        /// </summary>
        public GameAssetDictionaryIdAttribute(Type key, Type value) => (Key, Value) = (key, value);
    }
}