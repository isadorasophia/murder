using Newtonsoft.Json;
using Bang.Components;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Murder.Attributes;
using Murder.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Murder.Prefabs
{
    internal class EntityModifier
    {
        [JsonProperty]
        [HideInEditor]
        public readonly Guid Guid;

        [JsonProperty]
        private readonly Dictionary<Type, IComponent> _addComponent = new(new ComponentTypeComparator());

        [JsonProperty]
        private readonly Dictionary<Guid, EntityInstance> _children = new();

        [HideInEditor]
        public ImmutableArray<Guid> Children => _children.Keys.ToImmutableArray();

        [JsonProperty]
        private readonly HashSet<Type> _removeComponent = new(new ComponentTypeComparator());

        public EntityModifier(Guid guid)
        {
            Guid = guid;
        }

        private EntityModifier(
            Guid guid,
            Dictionary<Type, IComponent> addComponent,
            Dictionary<Guid, EntityInstance> children,
            HashSet<Type> removeComponent)
        {
            Guid = guid;

            _addComponent = addComponent;
            _children = children;
            _removeComponent = removeComponent;
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

        /// <summary>
        /// Merge modifier with <paramref name="other"/>.
        /// This will prioritize items present in <paramref name="other"/>.
        /// </summary>
        public EntityModifier ApplyModifiersFrom(EntityModifier other)
        {
            GameLogger.Verify(other.Guid == Guid, "Merging children modifiers of instance with different guids?");

            Dictionary<Type, IComponent> addComponent = new(_addComponent);
            foreach (var (type, c) in other._addComponent)
            {
                addComponent[type] = c;

                // TODO: Do we also need to remove "removed" components that were added later on...?
                // I am just confused at this point.
                GameLogger.Verify(!_removeComponent.Contains(type), "Remove component from removed modifiers?");
            }

            Dictionary<Guid, EntityInstance> children = new(_children);
            foreach (var (guid, e) in other._children)
            {
                children[guid] = e;
            }

            HashSet<Type> removeComponent = new(_removeComponent);
            foreach (Type t in other._removeComponent)
            {
                removeComponent.Add(t);

                if (_addComponent.ContainsKey(t))
                {
                    addComponent.Remove(t);
                }
            }

            return new(Guid, addComponent, children, removeComponent);
        }
    }
}
