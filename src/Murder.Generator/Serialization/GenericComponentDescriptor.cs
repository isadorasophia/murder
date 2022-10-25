namespace Murder.Generator.Serialization
{
    /// <summary>
    /// This is the component that describes a component which will be generated in the final code.
    /// </summary>
    internal record GenericComponentDescriptor
    {
        public readonly int Index;

        public readonly Type GenericType;
        public readonly Type GenericArgument;

        private string? _format;

        public string GetName()
        {
            if (_format is null)
            {
                _format = string.Format("{0}<{1}>",
                        GenericType.Name.Substring(0, GenericType.Name.LastIndexOf("`", StringComparison.InvariantCulture)),
                        GenericArgument.Name);
            }

            return _format;
        }

        public GenericComponentDescriptor(int index, Type genericType, Type genericArgument)
            => (Index, GenericType, GenericArgument) = (index, genericType, genericArgument);
    }
}
