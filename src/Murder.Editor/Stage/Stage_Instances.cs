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
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Murder.Editor.Stages
{
    public readonly record struct ChildInstanceId
    {
        public readonly Guid Parent;
        public readonly Guid Id;

        public ChildInstanceId(Guid parent, Guid id) => (Parent, Id) = (parent, id);
    }

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

        /// <summary>
        /// Maps all the children guid.
        ///  [Parent id, Child id] -> [Entity id]
        /// </summary>
        private readonly Dictionary<ChildInstanceId, int> _childInstanceToWorld = new();

        /// <summary>
        /// Maps all the children guid.
        ///  [Entity id] -> [Parent id, Child id]
        /// </summary>
        private readonly Dictionary<int, ChildInstanceId> _worldToChildInstance = new();

        private const int MAXIMUM_CHILD_DEPTH = 15;

        public Stage(
            ImGuiRenderer imGuiRenderer,
            RenderContext renderContext,
            StageType type,
            IWorldAsset worldAsset) : this(imGuiRenderer, renderContext, type, worldAsset.WorldGuid)
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

        private void TrackInstance(int id, Guid instance, IEntity? instanceEntity = default, Guid? parentGuid = null, int depth = 0)
        {
            if (depth > MAXIMUM_CHILD_DEPTH)
            {
                GameLogger.Fail("Reached maximum threshold for depth. Is this a recursion?");
                return;
            }

            if (parentGuid is null)
            {
                // Add itself.
                _instanceToWorld[instance] = id;
                _worldToInstance[id] = instance;
            }
            else
            {
                ChildInstanceId childInstanceId = new(parentGuid.Value, instance);

                _childInstanceToWorld[childInstanceId] = id;
                _worldToChildInstance[id] = childInstanceId;
            }

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
                    if (FindChildInstance(e.Parent.Value) is (IEntity parentOfParent, Guid parentOfParentGuid))
                    {
                        if (parentOfParent is PrefabEntityInstance)
                        {
                            parent = parentOfParent;
                        }
                        else if (parentOfParent.TryGetChild(parentOfParentGuid, out EntityInstance? parentInstance))
                        {
                            parent = parentInstance;
                        }
                    }
                }

                // Now, try to ask the parent (if any!) where is our instance.
                EntityInstance? childInstanceEntity = default;
                parent?.TryGetChild(instance, out childInstanceEntity);

                instanceEntity = childInstanceEntity;
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

                TrackInstance(childrenAsId[i], childrenAsInstances[i].Guid, childrenAsInstances[i], parentGuid: instance, depth + 1);
            }
        }

        /// <summary>
        /// Untrack the entity and all of its children.
        /// </summary>
        private void UntrackEntity(int id)
        {
            if (_worldToInstance.TryGetValue(id, out Guid g))
            {
                // Remove itself.
                _instanceToWorld.Remove(g);
                _worldToInstance.Remove(id);
            }

            _childEntities.Remove(id);

            if (_worldToChildInstance.TryGetValue(id, out ChildInstanceId childInstanceId))
            {
                _childInstanceToWorld.Remove(childInstanceId);
                _worldToChildInstance.Remove(id);
            }

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

            EditorHook.UnselectEntity(e);
        }

        public virtual bool AddComponentForInstance(Guid? parentGuid, Guid entityGuid, IComponent c)
        {
            if (FindEntity(parentGuid, entityGuid) is not Entity entity)
            {
                return false;
            }

            c = ProcessComponent(c);
            entity.AddOrReplaceComponent(c, c.GetType());

            return true;
        }

        public virtual bool ReplaceComponentForInstance(Guid? parentGuid, Guid entityGuid, IComponent c)
        {
            if (FindEntity(parentGuid, entityGuid) is not Entity entity)
            {
                return false;
            }

            c = ProcessComponent(c);
            entity.AddOrReplaceComponent(c, c.GetType());

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

        public virtual bool RemoveComponentForInstance(Guid? parentGuid, Guid entityGuid, Type t)
        {
            if (FindEntity(parentGuid, entityGuid) is not Entity entity)
            {
                return false;
            }

            entity.RemoveComponent(t);

            return true;
        }

        public virtual bool AddChildForInstance(Guid? parentOfParentGuid, IEntity parentInstance, EntityInstance childInstance)
        {
            // First, add both entities to the world.
            int childId = childInstance.Create(_world, parentInstance);
            if (FindEntity(parentOfParentGuid, parentInstance.Guid) is not Entity parent)
            {
                return false;
            }

            parent.AddChild(childId);

            TrackInstance(childId, childInstance.Guid, childInstance, parentGuid: parentInstance?.Guid);

            // Map the child back to its parent.
            _childEntities.Add(childId, parent.EntityId);

            return true;
        }

        public virtual bool RemoveInstance(Guid? parentGuid, Guid instanceGuid)
        {
            if (FindEntity(parentGuid, instanceGuid) is not Entity entity)
            {
                return false;
            }

            UntrackEntity(entity.EntityId);
            entity.Destroy();

            return true;
        }

        public virtual bool ActivateInstance(Guid? parentGuid, Guid instanceGuid)
        {
            if (FindEntity(parentGuid, instanceGuid) is not Entity entity)
            {
                return false;
            }

            entity.Activate();
            return true;
        }

        public virtual bool DeactivateInstance(Guid? parentGuid, Guid instanceGuid)
        {
            if (FindEntity(parentGuid, instanceGuid) is not Entity entity)
            {
                return false;
            }

            entity.Deactivate();
            return true;
        }

        private Entity? FindEntity(Guid? parentGuid, Guid instanceGuid)
        {
            int entityId;

            if (parentGuid is not null)
            {
                if (!_childInstanceToWorld.TryGetValue(new ChildInstanceId(parentGuid.Value, instanceGuid), out entityId))
                {
                    return null;
                }
            }
            else if (!_instanceToWorld.TryGetValue(instanceGuid, out entityId))
            {
                return null;
            }

            return _world.TryGetEntity(entityId);
        }
    }
}