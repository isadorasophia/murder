using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Murder.Assets;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;

namespace Murder.Editor.Stages
{
    /// <summary>
    /// Base implementation for rendering the world in the screen.
    /// </summary>
    public partial class Stage
    {
        /// <summary>
        /// Returns all entities in stage that has a component with a given attribute.
        /// </summary>
        public List<IEntity> FindEntitiesWithAttribute<T>() where T : Attribute
        {
            List<IEntity> result = new();

            Type[] components = ReflectionHelper.FetchComponentsWithAttribute<T>();

            ImmutableArray<Entity> entities = _world.GetEntitiesWith(ContextAccessorFilter.AnyOf, components);
            foreach (Entity e in entities)
            {
                if (_worldToInstance.TryGetValue(e.EntityId, out Guid entityGuid))
                {
                    if (_worldAsset is not null && _worldAsset.TryGetInstance(entityGuid) is IEntity instance)
                    {
                        result.Add(instance);
                    }

                    // Otherwise, this is not a world asset, so get the prefab entity.
                    if (Game.Data.TryGetAsset<PrefabAsset>(entityGuid) is IEntity instanceAsset)
                    {
                        result.Add(instanceAsset);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all entities in stage that has a set of components.
        /// </summary>
        public IList<IEntity> FindEntitiesWith(params Type[] components)
        {
            List<IEntity> result = new();

            ImmutableArray<Entity> entities = _world.GetEntitiesWith(components);
            foreach (Entity e in entities)
            {
                if (_worldToInstance.TryGetValue(e.EntityId, out Guid entityGuid))
                {
                    if (_worldAsset is not null && _worldAsset.TryGetInstance(entityGuid) is IEntity instance)
                    {
                        result.Add(instance);
                    }

                    // Otherwise, this is not a world asset, so get the prefab entity.
                    if (Game.Data.TryGetAsset<PrefabAsset>(entityGuid) is IEntity instanceAsset)
                    {
                        result.Add(instanceAsset);
                    }
                }
            }

            return result;
        }

        private readonly Dictionary<Type, ImmutableArray<Type>> _attributeToSystems = new();
        private readonly Dictionary<Type, bool?> _activeAttributeToSystems = new();

        internal void ToggleSystem(Type system, bool enable)
        {
            if (enable)
            {
                _world.ActivateSystem(system);
            }
            else
            {
                _world.DeactivateSystem(system);
            }
        }

        internal bool ActivateSystemsWith(bool enable, Type attribute)
        {
            if (!_attributeToSystems.TryGetValue(attribute, out ImmutableArray<Type> systems))
            {
                systems = ReflectionHelper.GetAllTypesWithAttributeDefined(attribute)
                    .ToImmutableArray();

                _attributeToSystems[attribute] = systems;
                _activeAttributeToSystems[attribute] = systems.Length == 0 ? false : null;
            }

            if (_activeAttributeToSystems[attribute] == enable) return false;

            _activeAttributeToSystems[attribute] = enable;

            if (enable)
            {
                foreach (Type s in systems)
                {
                    _world.ActivateSystem(s, immediately: true);
                }
            }
            else
            {
                foreach (Type s in systems)
                {
                    _world.DeactivateSystem(s, immediately: true);
                }
            }

            return true;
        }
    }
}