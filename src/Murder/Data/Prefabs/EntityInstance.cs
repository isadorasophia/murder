using Bang;
using Bang.Attributes;
using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Prefabs
{
    /// <summary>
    /// Represents an entity as an instance placed on the map.
    /// This map may be relative to the world or another entity.
    /// </summary>
    [Serializable]
    public class EntityInstance : IEntity
    {
        [Bang.Serialize]
        private Guid _guid;

        [HideInEditor]
        public Guid Guid => _guid;

        [Bang.Serialize]
        [ShowInEditor]
        private string _name;

        /// <summary>
        /// Entity id, if any. This will be persisted across save files.
        /// This only exists for instances in the world.
        /// </summary>
        [Bang.Serialize]
        [HideInEditor]
        public int? Id = default;

        public string Name => _name;

        /// <summary>
        /// By default, this is not based on any prefab.
        /// Return null.
        /// </summary>
        public virtual string? PrefabRefName => null;

        /// <summary>
        /// List of custom components that difer from the parent entity.
        /// </summary>
        [Bang.Serialize]
        protected readonly Dictionary<Type, IComponent> _components = new(new ComponentTypeComparator());

        /// <summary>
        /// Returns whether the entity is currently deactivated once instantiated in the map.
        /// </summary>
        public bool IsDeactivated = false;

        /// <summary>
        /// Whether this instance must have its activation propagated according to the parent. 
        /// <br/><br/>
        /// TODO: We might need to revisit on whether this is okay/actually scales well.
        /// </summary>
        [HideInEditor]
        [Bang.Serialize]
        public bool ActivateWithParent = false;

        private ImmutableArray<IComponent>? _cachedComponents;

        public virtual ImmutableArray<IComponent> Components
        {
            get
            {
                _cachedComponents ??= _components.Values.ToImmutableArray();
                return _cachedComponents.Value;
            }
        }

        [Bang.Serialize]
        protected Dictionary<Guid, EntityInstance>? _children;

        private ImmutableArray<EntityInstance>? _cachedChildren;

        public virtual ImmutableArray<Guid> Children => _children?.Keys.ToImmutableArray() ?? ImmutableArray<Guid>.Empty;

        public virtual ImmutableArray<EntityInstance> FetchChildren()
        {
            if (_cachedChildren is null)
            {
                _cachedChildren = _children?.Values.ToImmutableArray() ??
                    ImmutableArray<EntityInstance>.Empty;
            }

            return _cachedChildren.Value;
        }

        public EntityInstance() : this(name: default) { }

        public EntityInstance(string? name, Guid? guid = null) : this(name ?? string.Empty, guid ?? Guid.NewGuid()) { }

        [System.Text.Json.Serialization.JsonConstructor]
        public EntityInstance(string name, Guid guid)
        {
            _guid = guid;
            _name = name;
        }

        public virtual bool IsEmpty
        {
            get
            {
                if (HasNonIntrinsicComponent())
                {
                    return false;
                }

                // This is empty, unless it also has children.
                return Children.Any();
            }
        }

        /// <summary>
        /// Checks whether this entity has any non-intrinsic components, whether it's
        /// a distinct entity within the world.
        /// See <see cref="IntrinsicAttribute"/>.
        /// </summary>
        private bool HasNonIntrinsicComponent()
        {
            foreach (Type t in _components.Keys)
            {
                if (!Attribute.IsDefined(t, typeof(IntrinsicAttribute)))
                {
                    // Found one!
                    return true;
                }
            }

            return false;
        }

        internal virtual ImmutableArray<IComponent> FetchComponents(in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            if (modifiers.TryGetValue(Guid, out EntityModifier? modifier))
            {
                return modifier.FilterComponents(Components);
            }

            return Components;
        }

        public virtual IComponent GetComponent(Type componentType)
        {
            if (_components.TryGetValue(componentType, out IComponent? component))
            {
                return component;
            }

            throw new ArgumentException("Component type not found", nameof(componentType));
        }

        internal virtual ImmutableArray<EntityInstance> FetchChildren(in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            ImmutableArray<EntityInstance> children = FetchChildren();

            if (modifiers.TryGetValue(Guid, out EntityModifier? modifier))
            {
                children = children.AddRange(modifier.FetchChildren());
            }

            return children;
        }

        /// <summary>
        /// Create the instance entity in the world.
        /// </summary>
        /// <param name="world">The world this instance will be tied to.</param>
        /// <param name="replaceEntity">Entity that it will be replacing, if applicable.</param>
        public virtual int Create(World world, Entity? replaceEntity = null)
        {
            return Create(world, replaceEntity, ImmutableDictionary<Guid, EntityModifier>.Empty);
        }

        /// <summary>
        /// Create the instance entity in the world with a specified parent.
        /// </summary>
        /// <param name="world">The world this instance will be tied to.</param>
        /// <param name="parent">The parent, which may have custom modifiers.</param>
        public virtual int Create(World world, IEntity parent)
        {
            ImmutableDictionary<Guid, EntityModifier> modifiers = ImmutableDictionary<Guid, EntityModifier>.Empty;
            if (parent is PrefabEntityInstance prefabInstanceParent)
            {
                modifiers = prefabInstanceParent.GetChildrenModifiers();
            }

            return Create(world, replaceEntity: null, modifiers);
        }

        /// <summary>
        /// Create the instance entity in the world with a list of modifiers.
        /// </summary>
        /// <param name="world">The world this instance will be tied to.</param>
        /// <param name="replaceEntity">Entity that it will be replacing, if applicable.</param>
        /// <param name="modifiers">Components which might override any of the instances.</param>
        internal virtual int Create(World world, Entity? replaceEntity, in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            return CreateInternal(world, replaceEntity, Guid.Empty, modifiers);
        }

        /// <summary>
        /// Create the instance entity with a given asset in the world with a list of modifiers.
        /// This filters the modifiers according the children and components.
        /// </summary>
        /// <param name="world">The world this instance will be tied to.</param>
        /// <param name="replaceEntity">Entity that it will be replacing, if applicable.</param>
        /// <param name="asset">Prefab identifier.</param>
        /// <param name="modifiers">Components which might override any of the instances.</param>
        internal int CreateInternal(World world, Entity? replaceEntity, Guid asset, in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            int id;
            if (replaceEntity is not null)
            {
                id = replaceEntity.EntityId;
                EntityBuilder.Replace(world, replaceEntity, asset, FetchComponents(modifiers), FetchChildren(modifiers), modifiers);
            }
            else
            {
                id = EntityBuilder.Create(world, asset, FetchComponents(modifiers), FetchChildren(modifiers), modifiers, Id);
            }

            if (IsDeactivated)
            {
                Entity? entity = world.TryGetEntity(id);
                entity?.Deactivate();

                if (ActivateWithParent)
                {
                    entity?.SetActivateWithParent();
                }
            }

            return id;
        }

        /// <summary>
        /// Returns whether a component is present in the entity asset.
        /// </summary>
        public virtual bool IsComponentInAsset(IComponent c) => false;

        public void AddOrReplaceComponent(IComponent c)
        {
            _components[c.GetType()] = c;
            _cachedComponents = null;
        }

        /// <summary>
        /// Returns whether an instance of <paramref name="type"/> exists in the list of components.
        /// </summary>
        public virtual bool HasComponent(Type type) => _components.ContainsKey(type);

        public virtual bool RemoveComponent(Type t)
        {
            bool removed = _components.Remove(t);
            _cachedComponents = null;

            return removed;
        }

        public virtual bool RemoveAllComponents()
        {
            _components.Clear();
            _cachedComponents = null;

            return true;
        }

        public virtual void AddChild(EntityInstance asset)
        {
            _children ??= new();

            _children[asset.Guid] = asset;
            _cachedChildren = null;
        }

        public virtual EntityInstance GetChild(Guid instanceGuid)
        {
            if (_children is null)
            {
                throw new ArgumentException("This entity does not have any child!");
            }

            return _children[instanceGuid];
        }

        public virtual bool TryGetChild(Guid guid, [NotNullWhen(true)] out EntityInstance? instance)
        {
            instance = null;

            if (_children is null)
            {
                return false;
            }

            if (_children.TryGetValue(guid, out instance))
            {
                return true;
            }

            // If we did not find the children, we might actually be fetching the child of a child.
            foreach (EntityInstance? e in _children.Values)
            {
                if (e.TryGetChild(guid, out instance))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool RemoveChild(Guid instanceGuid)
        {
            if (_children is null)
            {
                return false;
            }

            bool removed = _children.Remove(instanceGuid);
            _cachedChildren = null;

            return removed;
        }

        public virtual bool CanRemoveChild(Guid instanceGuid)
        {
            return _children?.ContainsKey(instanceGuid) ?? false;
        }

        public virtual bool CanRevertComponent(Type t) => false;

        public virtual bool RevertComponent(Type t) => false;

        /// <summary>
        /// Set the name of the entity instance.
        /// </summary>
        public void SetName(string name) => _name = name;

        public virtual bool AddOrReplaceComponentForChild(Guid childGuid, IComponent component)
        {
            GameLogger.Verify(_children is not null && _children.ContainsKey(childGuid),
                "Adding component for child that does not exist!?");

            _children[childGuid].AddOrReplaceComponent(component);

            return true;
        }

        public virtual void RemoveComponentForChild(Guid childGuid, Type t)
        {
            GameLogger.Verify(_children is not null && _children.ContainsKey(childGuid),
                "Adding component for child that does not exist!?");

            _children[childGuid].RemoveComponent(t);
        }

        public virtual bool RevertComponentForChild(Guid childGuid, Type t) => false; // No operation

        public virtual bool HasComponentAtChild(Guid childGuid, Type type)
        {
            if (_children?.TryGetValue(childGuid, out EntityInstance? child) ?? false)
            {
                return child.HasComponent(type);
            }

            return false;
        }

        /// <summary>
        /// Try to get the components for a child.
        /// TODO: Do not expose the instance children directly...? Is this only necessary for prefabs?
        /// Are we limiting the amount of children recursive to two?
        /// </summary>
        /// <param name="guid"></param>
        public virtual ImmutableArray<IComponent> GetChildComponents(Guid guid)
        {
            if (TryGetChild(guid, out EntityInstance? child))
            {
                // This is our own child, no need to override this.
                return child.Components;
            }

            return ImmutableArray<IComponent>.Empty;
        }

        public virtual IComponent? TryGetComponentForChild(Guid guid, Type t)
        {
            if (TryGetChild(guid, out EntityInstance? child))
            {
                // This is our own child, no need to override this.
                return child.GetComponent(t);
            }

            return null;
        }

        public void UpdateGuid(Guid newGuid) => _guid = newGuid;
    }
}