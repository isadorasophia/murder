using Bang.Components;
using Bang.Entities;
using Murder.Assets;
using Murder.Core.Geometry;
using Murder.Editor.Utilities;
using Murder.Prefabs;
using System.Collections.Immutable;

namespace Murder.Editor.Stages
{
    public partial class Stage
    {
        public int SelectEntity(Guid entityGuid, bool select, bool clear)
        {
            if (_instanceToWorld.TryGetValue(entityGuid, out int id))
            {
                return SelectEntity(id, select, clear);
            }

            return -1;
        }

        public int SelectEntity(int id, bool select, bool clear)
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
                    EditorHook.SelectEntity(e, clear);
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

        internal (Guid? parent, Guid? instance) FindInstanceGuid(int id)
        {
            if (_worldToInstance.TryGetValue(id, out Guid entityGuid))
            {
                return (parent: null, entityGuid);
            }

            if (_worldToChildInstance.TryGetValue(id, out ChildInstanceId childInstanceId))
            {
                return (parent: childInstanceId.Parent, childInstanceId.Id);
            }

            return (null, null);
        }

        internal (IEntity parent, Guid childId)? FindChildInstance(int id)
        {
            (Guid? parentGuid, Guid? instanceGuid) = FindInstanceGuid(id);
            if (parentGuid is null || instanceGuid is null)
            {
                return null;
            }

            if (!_instanceToWorld.TryGetValue(parentGuid.Value, out int parentId) ||
                FindInstance(parentId) is not IEntity parent)
            {
                return null;
            }

            return (parent, instanceGuid.Value);
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