namespace Murder.Generator.Serialization
{
    /// <summary>
    /// This is the component that describes a component which will be generated in the final code.
    /// </summary>
    internal readonly struct GenericComponentDescriptor
    {
        public readonly int Index;

        public readonly Type GenericType;
        public readonly Type GenericArgument;

        public GenericComponentDescriptor(int index, Type genericType, Type genericArgument)
            => (Index, GenericType, GenericArgument) = (index, genericType, genericArgument);
    }
}
