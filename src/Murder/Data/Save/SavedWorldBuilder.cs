using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Assets;
using Murder.Attributes;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Save
{
    /// <summary>
    /// Asset for a map that has been generated within a world.
    /// </summary>
    internal class SavedWorldBuilder
    {
        private readonly World _world;

        private readonly Dictionary<int, EntityInstance> _worldEntities = new();

        /// <summary>
        /// Keeps track of all the children which have been added into their parents.
        /// </summary>
        private readonly HashSet<int> _allChildrenEntities = new();

        /// <summary>
        /// Speed things up by keeping track of entities which will be skipped.
        /// </summary>
        private readonly HashSet<int> _skipEntities = new();

        private bool HasGenerated => _worldEntities.Count != 0;

        internal SavedWorldBuilder(World world) => _world = world;

        internal async ValueTask<SavedWorld> CreateAsync(ImmutableArray<Entity> entities)
        {
            if (HasGenerated)
            {
                return ToSavedWorld();
            }

            Task convertAllEntities = Task.Run(() =>
            {
                foreach (Entity e in entities)
                {
                    if (TryConvertToInstance(e) is EntityInstance instance)
                    {
                        _worldEntities[e.EntityId] = instance;
                    }
                }
            });

            await convertAllEntities;

            return ToSavedWorld();
        }

        private SavedWorld ToSavedWorld() => new(_worldEntities.Values.ToImmutableDictionary(e => e.Guid, e => e));

        /// <summary>
        /// Convert this entity to a an instance which has not been placed in the world.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private EntityInstance? TryConvertToInstance(Entity? e)
        {
            if (e is null || e.IsDestroyed || _allChildrenEntities.Contains(e.EntityId) || _skipEntities.Contains(e.EntityId))
            {
                return null;
            }

            EntityInstance instance = new EntityInstance();

            // TODO: This was previously fetching the prefab entity for *performance* reasons.
            // However, that might complicate stuff with children and removing components of the prefab.
            // So let's deal with that later... if needed...
            //if (e.TryGetComponent<PrefabRefComponent>() is PrefabRefComponent asset)
            //{
            //    // We will not track the instance children since these were already placed into the world.
            //    instance = PrefabEntityInstance.CreateChildrenlessInstance(asset.AssetGuid);

            //    // We no longer need this.
            //    e.RemoveComponent<PrefabRefComponent>();
            //}

            InstancePersistKind kind = FilterComponents(ref instance, e);
            if (!kind.HasFlag(InstancePersistKind.All))
            {
                if (kind.HasFlag(InstancePersistKind.NoChildren))
                {
                    // Skip persisting any of its children.
                    foreach (int childId in e.Children)
                    {
                        if (_worldEntities.ContainsKey(childId))
                        {
                            _worldEntities.Remove(childId);
                        }

                        _skipEntities.Add(childId);
                    }
                }

                if (kind.HasFlag(InstancePersistKind.NoPersist))
                {
                    _skipEntities.Add(e.EntityId);

                    return null;
                }
            }

            if (string.IsNullOrEmpty(instance.Name))
            {
                instance.SetName($"Unknown ({e.EntityId})");
            }
            else
            {
                // Append the id to the name.
                instance.SetName($"{instance.Name} ({e.EntityId})");
            }

            // Persisted entity id.
            instance.Id = e.EntityId;
            instance.IsDeactivated = e.IsDeactivated;
            instance.ActivateWithParent = e.IsActivateWithParent();

            foreach (var (childId, childName) in e.FetchChildrenWithNames)
            {
                if (_skipEntities.Contains(e.EntityId))
                {
                    continue;
                }

                if (!_worldEntities.TryGetValue(childId, out EntityInstance? childInstance))
                {
                    // Child has not been fetched yet, convert it now.
                    childInstance = TryConvertToInstance(_world.TryGetEntity(childId));
                }

                if (childInstance != null)
                {
                    if (!string.IsNullOrEmpty(childName))
                    {
                        childInstance.SetName(childName);
                    }

                    childInstance.Id = childId;

                    instance.AddChild(childInstance);

                    // If this was previously a world entity, it no longer is.
                    // It is a child now.
                    _worldEntities.Remove(childId);
                    _allChildrenEntities.Add(childId);
                }
            }

            return instance;
        }

        [Flags]
        private enum InstancePersistKind
        {
            All = 0b1,
            NoChildren = 0b10,
            NoPersist = 0b110
        }

        /// <summary>
        /// Filter the components for a given instance.
        /// </summary>
        /// <returns>If false, this entity should not be serialized.</returns>
        private static InstancePersistKind FilterComponents(ref EntityInstance instance, Entity entityInWorld)
        {
            ImmutableArray<IComponent> components = entityInWorld.Components;
            foreach (IComponent c in components)
            {
                Type t = c.GetType();
                if (t.IsGenericType && t.GetGenericArguments().Length > 0)
                {
                    // Grab the component within the generic instead to study its attributes.
                    t = t.GetGenericArguments()[0];
                }

                if (Attribute.IsDefined(t, typeof(OnlyPersistThisComponentForEntityOnSaveAttribute)))
                {
                    // TODO: Should we check this before everything? Or this might be ignored by DoNotPersistEntityOnSaveAttribute.
                    instance.RemoveAllComponents();
                    instance.AddOrReplaceComponent(c is IModifiableComponent ? SerializationHelper.DeepCopy(c) : c);

                    return InstancePersistKind.NoChildren;
                }

                if (Attribute.IsDefined(t, typeof(DoNotPersistEntityOnSaveAttribute)))
                {
                    // The entity should not be serialized into this world.
                    return InstancePersistKind.NoPersist;
                }

                // This attribute would override any other attribute that locks
                // the component being persisted.
                if (!Attribute.IsDefined(t, typeof(PersistOnSaveAttribute)))
                {
                    if (Attribute.IsDefined(t, typeof(DoNotPersistOnSaveAttribute)))
                    {
                        DoNotPersistOnSaveAttribute? persistOnSave = 
                            Attribute.GetCustomAttribute(t, typeof(DoNotPersistOnSaveAttribute)) as DoNotPersistOnSaveAttribute;

                        bool skipComponent = true;

                        if (persistOnSave?.ExceptIfComponentIsPresent is Type targetComponent)
                        {
                            if (entityInWorld.HasComponent(targetComponent))
                            {
                                // Actually, we want to persist this component.
                                skipComponent = false;
                            }
                        }

                        if (skipComponent)
                        {
                            continue;
                        }
                    }

                    if (Attribute.IsDefined(t, typeof(RuntimeOnlyAttribute)))
                    {
                        // Do not persist this component.
                        continue;
                    }
                }

                if (instance.IsComponentInAsset(c))
                {
                    // Parent asset already owns this component, no need to serialize it.
                    continue;
                }

                if (c is SpriteComponent sprite && sprite.TargetSpriteBatch == Batches2D.UiBatchId)
                {
                    // Do not persist ui entities.
                    continue;
                }

                instance.AddOrReplaceComponent(c is IModifiableComponent ? SerializationHelper.DeepCopy(c) : c);
            }

            return instance.IsEmpty ? InstancePersistKind.NoPersist : InstancePersistKind.All;
        }
    }
}