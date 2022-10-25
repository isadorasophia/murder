namespace Murder.Generator.Serialization
{
    internal readonly struct Descriptor
    {
        /// <summary>
        /// Namespace name of the target that generated this descriptor.
        /// </summary>
        public readonly string Namespace;

        public readonly Dictionary<string, ComponentDescriptor> Components;
        public readonly Dictionary<string, ComponentDescriptor> Messages;
        public readonly GenericComponentDescriptor[] Generics;

        public Descriptor(
            string @namespace,
            Dictionary<string, ComponentDescriptor> components, 
            Dictionary<string, ComponentDescriptor> messages, 
            GenericComponentDescriptor[] generics)
            => (Namespace, Components, Messages, Generics) = (@namespace, components, messages, generics);
    }
}
