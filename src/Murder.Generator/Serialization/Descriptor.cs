using System.Linq;

namespace Murder.Generator.Serialization
{
    internal record Descriptor
    {
        /// <summary>
        /// Namespace name of the target that generated this descriptor.
        /// </summary>
        public readonly string Namespace;

        public readonly Dictionary<string, ComponentDescriptor> ComponentsMap;
        public readonly Dictionary<string, ComponentDescriptor> MessagesMap;
        public readonly Dictionary<string, GenericComponentDescriptor> GenericsMap;

        public Descriptor? ParentDescriptor = default;

        private ComponentDescriptor[]? _components;
        private ComponentDescriptor[]? _messages;
        private GenericComponentDescriptor[]? _generics;

        public ComponentDescriptor[] Components =>
            _components ??= ComponentsMap.Values.OrderBy(c => c.Index).ToArray();

        public ComponentDescriptor[] Messages =>
            _messages ??= MessagesMap.Values.OrderBy(c => c.Index).ToArray();

        public GenericComponentDescriptor[] Generics =>
            _generics ??= GenericsMap.Values.OrderBy(c => c.Index).ToArray();

        public ComponentDescriptor[] ComponentsWithParent() =>
            ParentDescriptor is null ? Components : ParentDescriptor.Components.Concat(Components).ToArray();

        public ComponentDescriptor[] MessagesWithParent() =>
            ParentDescriptor is null ? Messages : ParentDescriptor.Messages.Concat(Messages).ToArray();

        public GenericComponentDescriptor[] GenericsWithParent() =>
            ParentDescriptor is null ? Generics : ParentDescriptor.Generics.Concat(Generics).ToArray();

        public Descriptor(
            string @namespace,
            Dictionary<string, ComponentDescriptor> componentsMap, 
            Dictionary<string, ComponentDescriptor> messagesMap,
            Dictionary<string, GenericComponentDescriptor> genericsMap)
            => (Namespace, ComponentsMap, MessagesMap, GenericsMap) = (@namespace, componentsMap, messagesMap, genericsMap);
    }
}
