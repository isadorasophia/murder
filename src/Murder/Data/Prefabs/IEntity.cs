using Bang;
using Bang.Components;
using Bang.Entities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Prefabs
{
    public interface IEntity
    {
        public Guid Guid { get; }

        public string Name { get; }

        public void SetName(string name);

        /// <summary>
        /// If this has a prefab reference, this will return its name.
        /// Otherwise, return null.
        /// </summary>
        public string? PrefabRefName { get; }

        /// <summary>
        /// Returns all the components of the entity asset, followed by all the components of the instance.
        /// </summary>
        public ImmutableArray<IComponent> Components { get; }

        /// <summary>
        /// Returns all the identifiers for this entity children.
        /// </summary>
        public ImmutableArray<Guid> Children { get; }

        /// <summary>
        /// **INTERNAL ONLY**
        /// Fetches the actual entities for all children.
        /// </summary>
        public ImmutableArray<EntityInstance> FetchChildren();

        /// <summary>
        /// Create the entity in the world!
        /// </summary>
        /// <returns>The entity id in this world.</returns>
        public int Create(World world, Entity? replace = null);

        public IComponent GetComponent(Type componentType);

        public T GetComponent<T>() where T : IComponent => (T)GetComponent(typeof(T));

        public void AddOrReplaceComponent(IComponent c);

        public bool HasComponent(Type type);

        public bool RemoveComponent(Type t);

        public bool CanRevertComponent(Type type);

        public bool RevertComponent(Type t);

        public void AddChild(EntityInstance asset);

        public bool TryGetChild(Guid guid, [NotNullWhen(true)] out EntityInstance? instance);

        public bool RemoveChild(Guid guid);

        public bool CanRemoveChild(Guid instanceGuid);

        public bool AddOrReplaceComponentForChild(Guid childGuid, IComponent component);

        public void RemoveComponentForChild(Guid childGuid, Type t);

        public bool RevertComponentForChild(Guid childGuid, Type t);

        public bool HasComponentAtChild(Guid childGuid, Type type);

        public ImmutableArray<IComponent> GetChildComponents(Guid guid);

        public IComponent? TryGetComponentForChild(Guid guid, Type t);
    }
}