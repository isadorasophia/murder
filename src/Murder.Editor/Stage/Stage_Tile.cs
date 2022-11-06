using Bang.Contexts;
using Bang.Entities;
using Murder.Assets;
using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using System.Collections.Immutable;

namespace Murder.Editor.Stages
{
    /// <summary>
    /// Base implementation for rendering the world in the screen.
    /// </summary>
    public partial class Stage
    {
        /// <summary>
        /// Returns all entities in stage that apply a tile in the map.
        /// </summary>
        public IList<IEntity> FindTileEntities()
        {
            List<IEntity> result = new();

            ImmutableArray<Entity> entities = _world.GetEntitiesWith(typeof(TilesetComponent));
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

        private ImmutableArray<Type>? _tileSystems = default;
        private bool _isOnTileMode = true;

        internal bool ActivateTileEditorSystems(bool enable)
        {
            if (_isOnTileMode == enable) return false;

            _isOnTileMode = enable;
            _tileSystems ??= ReflectionHelper.GetAllTypesWithAttributeDefined<TileEditorAttribute>()
                .ToImmutableArray();

            if (enable)
            {
                foreach (Type s in _tileSystems)
                {
                    _world.ActivateSystem(s);
                }
            }
            else
            {
                foreach (Type s in _tileSystems)
                {
                    _world.DeactivateSystem(s);
                }
            }

            return true;
        }
    }
}
