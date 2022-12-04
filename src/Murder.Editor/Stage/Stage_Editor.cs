using Bang.Entities;
using Bang.Components;
using Murder.Assets;
using Murder.Core.Geometry;
using Murder.Prefabs;
using Murder.Utilities;
using System.Collections.Immutable;
using Murder.Editor.Utilities;

namespace Murder.Editor.Stages
{
    public partial class Stage
    {
        public int SelectEntity(Guid entityGuid, bool select)
        {
            if (_instanceToWorld.TryGetValue(entityGuid, out int id))
            {
                return SelectEntity(id, select);
            }

            return -1;
        }

        public int SelectEntity(int id, bool select)
        {
            if (_world.TryGetEntity(id) is Entity e)
            {
                if (_childEntities.ContainsKey(id))
                {
                    // Do not allow selecting children entities.
                    return id;
                }

                if (select)
                {
                    EditorHook.SelectEntity(e, clear: true);
                }
                else
                {
                    EditorHook.CloseEntity(e);
                }

                return id;
            }

            return -1;
        }

        internal bool IsSelected(Guid entityGuid)
        {
            if (_instanceToWorld.TryGetValue(entityGuid, out int id))
            {
                return EditorHook.IsEntitySelected(id);
            }

            return false;
        }

        internal ImmutableArray<IComponent>? TryGetComponentsOf(int entityId)
        {
            if (_world.TryGetEntity(entityId) is Entity e)
            {
                return e.Components;
            }

            return default;
        }

        internal IEntity? FindInstance(int id)
        {
            if (_worldToInstance.TryGetValue(id, out Guid entity))
            {
                if (_worldAsset is not null)
                {
                    return _worldAsset.TryGetInstance(entity);
                }
                
                // Otherwise, this is not a world asset, so get the prefab entity.
                return Architect.Data.TryGetAsset<PrefabAsset>(entity);
            }
            else
            {
                return null;
            }
        }

        internal (IEntity parent, Guid childId)? FindChildInstance(int id)
        {
            if (_childEntities.TryGetValue(id, out int parentId))
            {
                if (FindInstance(parentId) is IEntity parent && 
                    _worldToInstance.TryGetValue(id, out Guid child))
                {
                    return (parent, child);
                }

            }

            return null;
        }

        public void AddDimension(Guid entityGuid, Rectangle rect) => 
            EditorHook.AddEntityDimensionForEditor(entityGuid, rect);

        public void ClearDimension(Guid entityGuid) =>
            EditorHook.ClearDimension(entityGuid);

        public bool SetGroupToEntity(Guid entityGuid, string? group)
        {
            if (_instanceToWorld.TryGetValue(entityGuid, out int id))
            {
                return SetGroupToEntity(id, group);
            }
            
            return false;
        }
        
        public bool SetGroupToEntity(int id, string? group)
        {
            if (group is null)
            {
                EditorHook.Groups.Remove(id);
            }
            else
            {
                EditorHook.Groups[id] = group;
            }

            return true;
        }

        private int GetEntityIdForGuid(Guid entityGuid)
        {
            if (_instanceToWorld.TryGetValue(entityGuid, out int id))
            {
                return id;
            }

            return -1;
        }

        private string? GetNameForEntityId(int entityId)
        {
            return FindInstance(entityId)?.Name;
        }
    }
}
