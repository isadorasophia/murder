using Bang.Components;
using Bang.Entities;
using Murder.Assets;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Prefabs;
using Murder.Services;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Stages
{
    /// <summary>
    /// Base implementation for placing rendering entities in the stage.
    /// </summary>
    public partial class Stage
    {
        private readonly IWorldAsset? _worldAsset;

        public IWorldAsset? AssetReference => _worldAsset;

        private readonly Dictionary<Guid, int> _instanceToWorld = new();
        private readonly Dictionary<int, Guid> _worldToInstance = new();

        /// <summary>
        /// Maps all the children entities to their parents.
        ///  [Entity id] -> [Parent id]
        /// </summary>
        private readonly Dictionary<int, int> _childEntities = new();

        private const int MAXIMUM_CHILD_DEPTH = 15;

        public Stage(
            ImGuiRenderer imGuiRenderer,
            RenderContext renderContext,
            IEntity asset) : this(imGuiRenderer, renderContext)
        {
            AddEntity(asset);
        }

        public Stage(
            ImGuiRenderer imGuiRenderer,
            RenderContext renderContext,
            IWorldAsset worldAsset) : this(imGuiRenderer, renderContext, worldAsset.WorldGuid)
        {
            _worldAsset = worldAsset;

            foreach (Guid instance in _worldAsset.Instances)
            {
                int id = AddEntity(instance);

                // If this is a prefab world, keep track of the "group" layers.
                if (_worldAsset is WorldAsset prefabWorld &&
                    prefabWorld.GetGroupOf(instance) is string groupName)
                {
                    SetGroupToEntity(id, groupName);
                }
            }
        }

        public int AddEntity(IEntity instance)
        {
            int id = instance.Create(_world);
            TrackInstance(id, instance.Guid, instance);

            return id;
        }

        public int AddEntity(Guid instance)
        {
            if (_worldAsset is null)
            {
                GameLogger.Fail("Use AddEntity(IEntity) instead!");
                return -1;
            }

            int id = _worldAsset.TryCreateEntityInWorld(_world, instance);
            if (id == -1)
            {
                return -1;
            }

            TrackInstance(id, instance);

            return id;
        }

        /// <summary>
        /// This adds an entity without an asset to track it.
        /// </summary>
        public int AddEntityWithoutAsset(params IComponent[] components)
        {
            return _world.AddEntity(components).EntityId;
        }

        public virtual bool ReplaceComponentOnEntity(int entityId, IComponent c, bool forceReplace)
        {
            if (_world.TryGetEntity(entityId) is not Entity entity)
            {
                return false;
            }

            entity.ReplaceComponent(c, c.GetType(), forceReplace);

            return true;
        }
        public virtual bool AddOrReplaceComponentOnEntity(int entityId, IComponent c)
        {
            if (_world.TryGetEntity(entityId) is not Entity entity)
            {
                return false;
            }

            entity.AddOrReplaceComponent(c, c.GetType());

            return true;
        }

        private void TrackInstance(int id, Guid instance, IEntity? instanceEntity = default, int depth = 0)
        {
            if (depth > MAXIMUM_CHILD_DEPTH)
            {
                GameLogger.Fail("Reached maximum threshold for depth. Is this a recursion?");
                return;
            }

            // Add itself.
            _instanceToWorld[instance] = id;
            _worldToInstance[id] = instance;

            Entity? e = _world.TryGetEntity(id);
            GameLogger.Verify(e is not null);

            // Now, keep a reference of its children, if they have any.
            ImmutableArray<int> childrenAsId = e.Children;

            // Do we have an instance entity set by a previous parent?
            // If not, try to find the instance within the database or the world asset.
            instanceEntity ??= FindInstance(id);
            if (instanceEntity is null && e.Parent != null)
            {
                // No luck - this is a child though!
                // Since we won't be any deeper than two layers for entities that have not been
                // created all at once, find the parent instance.
                IEntity? parent = FindInstance(e.Parent.Value);

                if (parent is null)
                {
                    if (FindChildInstance(e.Parent.Value) is (IEntity parentOfParent, Guid parentGuid))
                    {
                        if (parentOfParent is PrefabEntityInstance)
                        {
                            parent = parentOfParent;
                        }
                        else if (parentOfParent.TryGetChild(parentGuid, out EntityInstance? parentInstance))
                        {
                            parent = parentInstance;
                        }
                    }
                }

                // Now, try to ask the parent (if any!) where is our instance.
                EntityInstance? childInstanceEntity = default;
                parent?.TryGetChild(instance, out childInstanceEntity);
                instanceEntity = childInstanceEntity as IEntity;
            }

            if (instanceEntity is null)
            {
                GameLogger.Fail("How is the instance entity null?");
                return;
            }

            ImmutableArray<Guid> childrenAsGuid = instanceEntity.Children;

            ImmutableArray<EntityInstance> childrenAsInstances;
            if (_childEntities.ContainsKey(id) && childrenAsGuid.Length != childrenAsId.Length)
            {
                // If there is a mismatch in the number of children of the instance, it means that we actually
                // have custom components that were modified within the editor.
                // In those scenarios, we will find the root entity and query its modifiers.
                if (EntityServices.FindRootEntity(e)?.EntityId is int rootId &&
                    _worldToInstance.TryGetValue(rootId, out Guid rootGuid) &&
                    _worldAsset?.TryGetInstance(rootGuid) is PrefabEntityInstance rootInstance)
                {
                    childrenAsInstances = rootInstance.FetchChildChildren(instanceEntity);
                }
                else
                {
                    GameLogger.Warning("Unable to load custom children of entity instance in world. Entity tracker will be off.");
                    return;
                }
            }
            else
            {
                var builder = ImmutableArray.CreateBuilder<EntityInstance>(childrenAsGuid.Length);
                for (int i = 0; i < childrenAsGuid.Length; ++i)
                {
                    instanceEntity.TryGetChild(childrenAsGuid[i], out EntityInstance? childInstanceEntity);
                    builder.Add(childInstanceEntity!);
                }

                childrenAsInstances = builder.ToImmutable();
            }

            for (int i = 0; i < childrenAsId.Length; ++i)
            {
                // Map the child back to its parent.
                _childEntities.Add(childrenAsId[i], id);

                TrackInstance(childrenAsId[i], childrenAsInstances[i].Guid, childrenAsInstances[i], depth + 1);
            }
        }

        /// <summary>
        /// Untrack the entity and all of its children.
        /// </summary>
        private void UntrackEntity(int id)
        {
            // Remove itself.
            _instanceToWorld.Remove(_worldToInstance[id]);
            _worldToInstance.Remove(id);

            _childEntities.Remove(id);

            Entity? e = _world.TryGetEntity(id);
            if (e is null)
            {
                GameLogger.Fail("Entity was already removed?");
                return;
            }

            // If this had children tied to it, remove all of them.
            foreach (int child in e.Children)
            {
                UntrackEntity(child);
            }
        }

        public virtual bool AddComponentForInstance(Guid entityGuid, IComponent c)
        {
            if (!_instanceToWorld.TryGetValue(entityGuid, out int id) ||
                _world.TryGetEntity(id) is not Entity entity)
            {
                return false;
            }

            c = ProcessComponent(c);
            entity.AddOrReplaceComponent(c, c.GetType());

            return true;
        }

        public virtual bool ReplaceComponentForInstance(Guid entityGuid, IComponent c)
        {
            if (!_instanceToWorld.TryGetValue(entityGuid, out int id) ||
                _world.TryGetEntity(id) is not Entity entity)
            {
                return false;
            }

            c = ProcessComponent(c);
            entity.ReplaceComponent(c, c.GetType());

            return true;
        }

        private IComponent ProcessComponent(IComponent c)
        {
            // Filter specific editor components.
            if (c is EventListenerEditorComponent eventListenerEditor)
            {
                return eventListenerEditor.ToEventListener();
            }

            return c;
        }

        public virtual bool RemoveComponentForInstance(Guid entityGuid, Type t)
        {
            if (!_instanceToWorld.TryGetValue(entityGuid, out int id) ||
                _world.TryGetEntity(id) is not Entity entity)
            {
                return false;
            }

            entity.RemoveComponent(t);

            return true;
        }

        public virtual bool AddChildForInstance(IEntity parentInstance, EntityInstance childInstance)
        {
            // First, add both entities to the world.
            int childId = childInstance.Create(_world, parentInstance);

            int parentId = _instanceToWorld[parentInstance.Guid];
            if (_world.TryGetEntity(parentId) is not Entity parent)
            {
                return false;
            }

            parent.AddChild(childId);

            TrackInstance(childId, childInstance.Guid, childInstance);

            // Map the child back to its parent.
            _childEntities.Add(childId, parentId);

            return true;
        }

        public virtual bool RemoveInstance(Guid instanceGuid)
        {
            int instanceId = _instanceToWorld[instanceGuid];
            if (_world.TryGetEntity(instanceId) is not Entity entity)
            {
                return false;
            }

            UntrackEntity(instanceId);
            entity.Destroy();

            return true;
        }

        protected Guid? TryGetParent(Guid childGuid)
        {
            if (!_instanceToWorld.ContainsKey(childGuid))
            {
                return default;
            }

            int childId = _instanceToWorld[childGuid];

            if (_world.TryGetEntity(childId) is not Entity child)
            {
                return default;
            }

            int? parentId = child.Parent;
            if (parentId is null)
            {
                return default;
            }

            return _worldToInstance[parentId.Value];
        }
    }
}