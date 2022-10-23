using InstallWizard.Core;
using InstallWizard.Data.Prefabs;
using InstallWizard.Util;
using Murder.Assets;

namespace Editor.Stages
{
    public partial class Stage
    {
        public int SelectEntity(Guid entityGuid)
        {
            if (_instanceToWorld.TryGetValue(entityGuid, out int id))
            {
                if (_childEntities.ContainsKey(id))
                {
                    // Do not allow selecting children entities.
                    return id;
                }

                EditorHook.Selected.AddOnce(id);
                return id;
            }

            return -1;
        }

        internal bool IsSelected(Guid entityGuid)
        {
            if (_instanceToWorld.TryGetValue(entityGuid, out int id))
            {
                return EditorHook.Selected.Contains(id);
            }

            return false;
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
    }
}
