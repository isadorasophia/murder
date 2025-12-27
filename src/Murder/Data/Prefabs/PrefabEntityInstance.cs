using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Assets;
using Murder.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Prefabs
{
    public class PrefabEntityInstance : EntityInstance
    {
        /// <summary>
        /// This is the guid of the <see cref="PrefabAsset"/> that this refers to.
        /// </summary>
        public readonly PrefabReference PrefabRef;

        /// <summary>
        /// List of custom components that have been removed from the parent entity, if any.
        /// </summary>
        [Bang.Serialize]
        private readonly HashSet<Type> _removeComponent = new(new ComponentTypeComparator());

        [Bang.Serialize]
        private readonly Dictionary<Guid, EntityModifier> _childrenModifiers = new();

        [Bang.Serialize]
        private readonly bool _ignorePrefabChildren = false;

        public PrefabEntityInstance() { }

        internal PrefabEntityInstance(PrefabReference prefabRef, string? name, bool ignorePrefabChildren, Guid? guid = null)
            : this(prefabRef, name ?? prefabRef.Fetch().GetSimplifiedName(), ignorePrefabChildren, guid ?? Guid.NewGuid())
        {
            PrefabRef = prefabRef;
            _ignorePrefabChildren = ignorePrefabChildren;
        }

        [System.Text.Json.Serialization.JsonConstructor]
        internal PrefabEntityInstance(PrefabReference prefabRef, string name, bool ignorePrefabChildren, Guid guid)
            : base(name, guid)
        {
            PrefabRef = prefabRef;
            _ignorePrefabChildren = ignorePrefabChildren;
        }

        public static PrefabEntityInstance CreateChildrenlessInstance(Guid assetGuid) =>
            new(new(assetGuid), name: default, ignorePrefabChildren: true);

        public override string? PrefabRefName => PrefabRef.Fetch().GetSimplifiedName();

        public override bool IsEmpty
        {
            get
            {
                if (PrefabRef.CanFetch)
                {
                    // There is a non-empty prefab, so this is not empty.
                    return false;
                }

                return base.IsEmpty;
            }
        }

        /// <summary>
        /// Returns all the components of the entity asset, followed by all the components of the instance.
        /// </summary>
        public override ImmutableArray<IComponent> Components
        {
            get
            {
                var components = PrefabRef.Fetch().Components;
                var builder = components.ToBuilder();

                foreach (var c in components)
                {
                    // Skip components that we replaced in the instance or that have been removed.
                    if (_components.ContainsKey(c.GetType()) || _removeComponent.Contains(c.GetType()))
                    {
                        builder.Remove(c);
                    }
                }

                builder.AddRange(_components.Values);

                return builder.ToImmutable();
            }
        }

        public override IComponent GetComponent(Type componentType)
        {
            if (_components.TryGetValue(componentType, out IComponent? component))
            {
                return component;
            }

            return PrefabRef.Fetch().GetComponent(componentType);
        }

        public override ImmutableArray<Guid> Children
        {
            get
            {
                if (!_ignorePrefabChildren)
                {
                    return PrefabRef.Fetch().Children.AddRange(base.Children).ToImmutableArray();
                }

                return base.Children;
            }
        }

        /// <summary>
        /// Returns all the children of the entity asset, followed by all the children of the instance.
        /// </summary>
        public override ImmutableArray<EntityInstance> FetchChildren()
        {
            if (!_ignorePrefabChildren)
            {
                ImmutableArray<EntityInstance> children = PrefabRef.Fetch().FetchChildren()
                    .AddRange(base.FetchChildren()).ToImmutableArray();

                return children;
            }

            return base.FetchChildren();
        }

        public override bool TryGetChild(Guid guid, [NotNullWhen(true)] out EntityInstance? instance)
        {
            if (!_ignorePrefabChildren && PrefabRef.Fetch().TryGetChild(guid, out instance))
            {
                return true;
            }

            return base.TryGetChild(guid, out instance);
        }

        public override bool CanRemoveChild(Guid instanceGuid)
        {
            // Right now, we only won't remove children that belong to the prefab. If this child belongs to a
            // modifier, we will allow it.
            return !PrefabRef.Fetch().TryGetChild(instanceGuid, out _);
        }

        /// <summary>
        /// Create the instance entity in the world.
        /// </summary>
        /// <param name="world">The world this instance will be tied to.</param>
        /// <param name="replaceEntity">Entity that it will be replacing, if applicable.</param>
        public override int Create(World world, Entity? replaceEntity)
        {
            return Create(world, replaceEntity, ImmutableDictionary<Guid, EntityModifier>.Empty);
        }

        /// <summary>
        /// Create the instance entity in the world with a list of modifiers.
        /// </summary>
        /// <param name="world">The world this instance will be tied to.</param>
        /// <param name="replaceEntity">Entity that it will be replacing, if applicable.</param>
        /// <param name="modifiers">Components which might override any of the instances.</param>
        internal override int Create(World world, Entity? replaceEntity, in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            Dictionary<Guid, EntityModifier> currentModifiers = _childrenModifiers
                .ToDictionary(e => e.Key, e => e.Value);

            // If this has been created out of a prefab reference, merge its child modifiers.
            if (PrefabRef.Fetch().GetChildrenModifiers() is ImmutableDictionary<Guid, EntityModifier> prefabReferences)
            {
                foreach (var (guid, prefabModifier) in prefabReferences)
                {
                    currentModifiers[guid] = currentModifiers.ContainsKey(guid) ?
                        prefabModifier.ApplyModifiersFrom(currentModifiers[guid]) :
                        prefabModifier;
                }
            }

            foreach (var (guid, parentModifier) in modifiers)
            {
                currentModifiers[guid] = currentModifiers.ContainsKey(guid) ?
                    parentModifier.ApplyModifiersFrom(currentModifiers[guid]) :
                    parentModifier;
            }

            return CreateInternal(world, replaceEntity, PrefabRef.Guid, currentModifiers.ToImmutableDictionary());
        }

        internal ImmutableDictionary<Guid, EntityModifier> GetChildrenModifiers() => _childrenModifiers.ToImmutableDictionary();

        /// <summary>
        /// Returns whether a component is present in the entity asset.
        /// </summary>
        public override bool IsComponentInAsset(IComponent c) => PrefabRef.Fetch().HasComponent(c);

        /// <summary>
        /// Returns whether an instance of <paramref name="type"/> exists in the list of components.
        /// </summary>
        public override bool HasComponent(Type type) => PrefabRef.Fetch().HasComponent(type) || base.HasComponent(type);

        public override bool RemoveComponent(Type t)
        {
            bool removed = base.RemoveComponent(t);

            // This component belongs to the parent component and has never been removed before.
            // Otherwise, delete the component that this instance has.
            if (PrefabRef.Fetch().HasComponent(t) && !_removeComponent.Contains(t))
            {
                removed |= _removeComponent.Add(t);
            }

            return removed;
        }

        public override bool CanRevertComponent(Type t)
        {
            return base.HasComponent(t) && PrefabRef.Fetch().HasComponent(t);
        }

        public override bool RevertComponent(Type t)
        {
            return base.RemoveComponent(t);
        }

        /// <summary>
        /// This checks whether a child can be modified.
        /// This means that it does not belong to any prefab reference.
        /// </summary>
        public virtual bool CanModifyChildAt(Guid childId)
        {
            if (PrefabRef.Fetch().TryGetChild(childId, out _))
            {
                // This is actually from our prefab, do not allow!
                return false;
            }

            return true;
        }

        public virtual void AddChildAtChild(Guid childId, EntityInstance instance)
        {
            EntityModifier modifier = GetOrCreateModifier(childId);
            modifier.AddChild(instance);
        }

        public virtual void RemoveChildAtChild(Guid childId, Guid instance)
        {
            EntityModifier modifier = GetOrCreateModifier(childId);
            modifier.RemoveChild(instance);
        }

        public override bool AddOrReplaceComponentForChild(Guid instance, IComponent component)
        {
            EntityModifier? modifier;

            if (base.TryGetChild(instance, out EntityInstance? child))
            {
                child.AddOrReplaceComponent(component);

                return true;
            }
            else if (TryGetChild(instance, out child))
            {
                Type t = component.GetType();

                // On prefab children, we need to make sure we are not adding the same component in a fancy modifier.
                if (child.HasComponent(t) && component.Equals(child.GetComponent(t)))
                {
                    // If we had a modifier for this child and this component, remove it.
                    if (_childrenModifiers.TryGetValue(instance, out modifier))
                    {
                        modifier.UndoCustomComponent(t);
                    }

                    return false;
                }
            }

            modifier = GetOrCreateModifier(instance);
            modifier.AddOrReplaceComponent(component);

            return true;
        }

        public override void RemoveComponentForChild(Guid instance, Type t)
        {
            if (base.TryGetChild(instance, out EntityInstance? child))
            {
                child.RemoveComponent(t);
                return;
            }

            EntityModifier modifier = GetOrCreateModifier(instance);
            modifier.RemoveComponent(t);
        }

        public override bool RevertComponentForChild(Guid childGuid, Type t)
        {
            if (_childrenModifiers.TryGetValue(childGuid, out EntityModifier? modifier) && modifier.HasComponent(t))
            {
                return modifier.UndoCustomComponent(t);
            }

            return false;
        }

        public override bool HasComponentAtChild(Guid instance, Type type)
        {
            if (_childrenModifiers.TryGetValue(instance, out EntityModifier? modifier))
            {
                if (modifier.IsComponentRemoved(type))
                {
                    return false;
                }

                if (modifier.HasComponent(type))
                {
                    return true;
                }
            }

            if (_children?.TryGetValue(instance, out EntityInstance? child) ?? false)
            {
                return child.HasComponent(type);
            }

            return false;
        }

        private EntityModifier GetOrCreateModifier(Guid instance)
        {
            if (!_childrenModifiers.TryGetValue(instance, out EntityModifier? modifier))
            {
                modifier = new EntityModifier(instance);
                _childrenModifiers.Add(instance, modifier);
            }

            return modifier;
        }

        public override bool RemoveChild(Guid instanceGuid)
        {
            if (base.RemoveChild(instanceGuid))
            {
                return true;
            }

            // Otherwise, this might be a custom child of another child.
            // Iterate over each of our modifiers and try to find a child.
            foreach (EntityModifier modifier in _childrenModifiers.Values)
            {
                if (modifier.HasChild(instanceGuid))
                {
                    modifier.RemoveChild(instanceGuid);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fetch the components for a given child.
        /// This will filter any modifiers made to the children components.
        /// </summary>
        public override ImmutableArray<IComponent> GetChildComponents(Guid guid)
        {
            ImmutableArray<IComponent> childComponents = ImmutableArray<IComponent>.Empty;
            if (!_ignorePrefabChildren && PrefabRef.Fetch().TryGetChild(guid, out EntityInstance? child))
            {
                // Found child from the parent!
                childComponents = PrefabRef.Fetch().GetChildComponents(guid);
            }
            else if (base.TryGetChild(guid, out child))
            {
                // This is our own child, grab whatever we had for it.
                childComponents = child.Components;
            }

            if (_childrenModifiers.TryGetValue(guid, out EntityModifier? modifier))
            {
                childComponents = modifier.FilterComponents(childComponents);
            }

            return childComponents;
        }

        /// <summary>
        /// Get all the children of a child.
        /// This will take into account any modifiers of the parent.
        /// </summary>
        public ImmutableArray<EntityInstance> FetchChildChildren(IEntity child)
        {
            ImmutableArray<EntityInstance> children = child.FetchChildren();
            if (_childrenModifiers.TryGetValue(child.Guid, out EntityModifier? modifier))
            {
                children = children.AddRange(modifier.FetchChildren());
            }

            return children;
        }

        public override IComponent? TryGetComponentForChild(Guid guid, Type t)
        {
            if (!_ignorePrefabChildren && PrefabRef.Fetch().TryGetChild(guid, out EntityInstance? child))
            {
                if (_childrenModifiers.TryGetValue(guid, out EntityModifier? modifier) && modifier.TryGetComponent(t, out IComponent? c))
                {
                    return c;
                }

                // No components have been overriden, just return the component.
                return child.GetComponent(t);
            }

            return base.TryGetComponentForChild(guid, t);
        }

        public bool CanRevertComponentForChild(Guid guid, Type t)
        {
            if (_childrenModifiers.TryGetValue(guid, out EntityModifier? modifier))
            {
                return modifier.HasComponent(t);
            }

            return false;
        }
    }
}