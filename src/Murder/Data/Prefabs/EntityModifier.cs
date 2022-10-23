using Newtonsoft.Json;
using Bang.Components;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Murder.Attributes;

namespace Murder.Prefabs
{
    internal class EntityModifier
    {
        [JsonProperty]
        [HideInEditor]
        public readonly Guid Guid;

        [JsonProperty]
        private readonly Dictionary<Type, IComponent> _addComponent = new();

        [JsonProperty]
        private readonly Dictionary<Guid, EntityInstance> _children = new();

        [HideInEditor]
        public ImmutableArray<Guid> Children => _children.Keys.ToImmutableArray();

        [JsonProperty]
        private readonly HashSet<Type> _removeComponent = new();

        public EntityModifier(Guid guid)
        {
            Guid = guid;
        }

        public void AddChild(EntityInstance child)
        {
            _children.Add(child.Guid, child);
        }

        public bool HasChild(Guid childId) => _children.ContainsKey(childId);

        public void RemoveChild(Guid guid)
        {
            _children.Remove(guid);
        }

        public void AddOrReplaceComponent(IComponent c)
        {
            Type t = c.GetType();

            _addComponent[t] = c;
            _removeComponent.Remove(t);
        }

        public bool HasComponent(Type t)
        {
            return _addComponent.ContainsKey(t);
        }

        public bool TryGetComponent(Type t, [NotNullWhen(true)] out IComponent? result)
        {
            return _addComponent.TryGetValue(t, out result);
        }

        public bool IsComponentRemoved(Type t)
        {
            return _removeComponent.Contains(t);
        }

        public bool UndoCustomComponent(Type t)
        {
            return _addComponent.Remove(t);
        }

        public void RemoveComponent(Type t)
        {
            _addComponent.Remove(t);
            _removeComponent.Add(t);
        }

        public ImmutableArray<IComponent> FilterComponents(in IEnumerable<IComponent> allComponents)
        {
            var builder = ImmutableArray.CreateBuilder<IComponent>();

            foreach (IComponent c in allComponents)
            {
                Type t = c.GetType();

                // Skip any components that have been removed or added as a custom component.
                if (!_removeComponent.Contains(t) && !_addComponent.ContainsKey(t))
                {
                    builder.Add(c);
                }
            }

            foreach (var (_, component) in _addComponent)
            {
                builder.Add(component);
            }

            return builder.ToImmutable();
        }

        public ImmutableArray<EntityInstance> FetchChildren()
        {
            return _children.Values.ToImmutableArray();
        }
    }
}
