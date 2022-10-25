namespace Murder.Generator.Serialization
{
    /// <summary>
    /// This is the component that describes a component which will be generated in the final code.
    /// </summary>
    internal readonly struct ComponentDescriptor
    {
        public readonly int Index;
        public readonly string Name;

        public readonly Type Type;

        public ComponentDescriptor(int index, string name, Type type)
            => (Index, Name, Type) = (index, name, type);
    }
}
