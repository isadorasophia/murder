using Bang.Entities;
using Murder.Assets;
using Murder.Components;
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

            ImmutableArray<Entity> entities = _world.GetEntitiesWith(typeof(MapThemeComponent));
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
    }
}
