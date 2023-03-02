namespace Murder.Attributes
{
    /// <summary>
    /// Attribute for looking on a <see cref="Type"/> with specific properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeOfAttribute : Attribute
    {
        public readonly Type Type;

        /// <summary>
        /// Creates a new <see cref="TypeOfAttribute"/>.
        /// </summary>
        /// <param name="type">The base type which we will look for implementations.</param>
        public TypeOfAttribute(Type type) => Type = type;
    }
}
