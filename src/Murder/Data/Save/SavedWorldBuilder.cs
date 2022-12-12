using Bang;
using Bang.Components;
using Bang.Entities;
using System.Collections.Immutable;
using Murder.Assets;
using Murder.Attributes;
using Murder.Prefabs;
using Murder.Components;
using Murder.Utilities.Attributes;

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

        internal SavedWorld Create()
        {
            if (HasGenerated)
            {
                return ToSavedWorld();
            }

            foreach (Entity e in _world.GetAllEntities())
            {
                if (TryConvertToInstance(e) is EntityInstance instance)
                {
                    _worldEntities[e.EntityId] = instance;
                }
            }

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
            if (e is null || _allChildrenEntities.Contains(e.EntityId) || _skipEntities.Contains(e.EntityId))
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

            if (!FilterComponents(ref instance, e.Components))
            {
                _skipEntities.Add(e.EntityId);
                return null;
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

            // Persist entity id.
            instance.Id = e.EntityId;

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

        /// <summary>
        /// Filter the components for a given instance.
        /// </summary>
        /// <returns>If false, this entity should not be serialized.</returns>
        private static bool FilterComponents(ref EntityInstance instance, ImmutableArray<IComponent> components)
        {
            foreach (IComponent c in components)
            {
                Type t = c.GetType();
                if (Attribute.IsDefined(t, typeof(DoNotPersistEntityOnSaveAttribute)))
                {
                    // The entity should not be serialized into this world.
                    return false;
                }

                // This attribute would override any other attribute that locks
                // the component being persisted.
                if (!Attribute.IsDefined(t, typeof(PersistOnSaveAttribute)))
                {
                    if (Attribute.IsDefined(t, typeof(DoNotPersistOnSaveAttribute)))
                    {
                        // Do not persist this component.
                        continue;
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

                instance.AddOrReplaceComponent(c);
            }

            return !instance.IsEmpty;
        }
    }
}