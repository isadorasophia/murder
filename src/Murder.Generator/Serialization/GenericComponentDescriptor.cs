namespace Murder.Generator.Serialization
{
    /// <summary>
    /// This is the component that describes a component which will be generated in the final code.
    /// </summary>
    internal record GenericComponentDescriptor
    {
        public int Index;

        public readonly Type InstanceType;
        public readonly Type? GenericArgument;

        private string? _format;

        public string GetName()
        {
            if (_format is null)
            {
                _format = GenericArgument is null ? 
                    InstanceType.Name :
                    string.Format("{0}<{1}>",
                        InstanceType.Name.Substring(0, InstanceType.Name.LastIndexOf("`", StringComparison.InvariantCulture)),
                        GenericArgument.Name);
            }

            return _format;
        }

        public GenericComponentDescriptor(int index, Type instanceType, Type? genericArgument = null)
            => (Index, InstanceType, GenericArgument) = (index, instanceType, genericArgument);
    }
}
