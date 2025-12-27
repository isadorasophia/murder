using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Prefabs;
using Murder.Serialization;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Murder.Assets
{
    public class PrefabAsset : GameAsset, IEntity
    {
        private const char SingleIcon = '\uf11a';
        private const char FamilyIcon = '\uf500';

        /// <summary>
        /// Whether this should show in the editor selector.
        /// </summary>
        public bool ShowOnPrefabSelector = false;

        public override char Icon => Children.Length == 0 ? SingleIcon : FamilyIcon;
        public override string EditorFolder => "#\uf0e8Prefabs";
        public override System.Numerics.Vector4 EditorColor => new System.Numerics.Vector4(0.75f, 0.45f, 1, 1);
        public override string SaveLocation => Path.Join(Game.Profile.ContentECSPath, FileHelper.Clean(EditorFolder));

        [Bang.Serialize]
        private readonly EntityInstance _entity = new();

        /// <summary>
        /// Dimensions of the prefab. Used when drawing it on the map or the editor.
        /// </summary>
        [Bang.Serialize]
        public readonly TileDimensions Dimensions;

        /// <summary>
        /// Create an instance of the entity and all of its children.
        /// </summary>
        public int Create(World world, Entity? replaceEntity = null) => _entity.Create(world, replaceEntity);

        public Entity CreateAndFetch(World world) => world.GetEntity(_entity.Create(world, replaceEntity: null));

        /// <summary>
        /// This will replace an existing entity in the world.
        /// It keeps some elements of the original entity: position and target id components.
        /// </summary>
        public void Replace(World world, Entity e) => _entity.Create(world, e);

        /// <summary>
        /// This will replace an existing entity in the world.
        /// It keeps some elements of the original entity: position and target id components.
        /// </summary>
        /// <param name="world">World.</param>
        /// <param name="e">Entity</param>
        /// <param name="startWithComponents">Custom components that will override whatever this prefab has.</param>
        public void Replace(World world, Entity e, params IComponent[] startWithComponents)
        {
            Replace(world, e);

            for (int i = 0; i < startWithComponents.Length; ++i)
            {
                IComponent c = startWithComponents[i];
                if (c is ITransformComponent && e.TryGetTransform() is IMurderTransformComponent transform)
                {
                    c = transform.WithoutParent();
                }
                else if (c is IModifiableComponent)
                {
                    c = SerializationHelper.DeepCopy(c);
                }

                e.AddOrReplaceComponent(c, c.GetType());
            }
        }

        /// <summary>
        /// Creates a new instance entity from the current asset.
        /// </summary>
        public EntityInstance ToInstance(string name) => EntityBuilder.CreateInstance(Guid, name);

        public PrefabAsset ToInstanceAsAsset(string name)
        {
            PrefabAsset asset = new(EntityBuilder.CreateInstance(Guid, name));
            asset.MakeGuid();

            return asset;
        }

        public PrefabAsset() { }

        public PrefabAsset(EntityInstance instance) => (_entity, Name) = (instance, instance.Name);

        Guid IEntity.Guid => Guid;

        string IEntity.Name => GetSimplifiedName();

        // Custom name is not supported for assets.
        void IEntity.SetName(string name) => throw new NotImplementedException();

        public string? PrefabRefName => _entity.PrefabRefName;

        public ImmutableArray<IComponent> Components => _entity.Components;

        public ImmutableArray<Guid> Children => _entity.Children;

        public ImmutableArray<EntityInstance> FetchChildren() => _entity.FetchChildren();

        public bool HasComponent(Type type) => _entity.HasComponent(type);

        public bool HasComponent(IComponent c)
        {
            Type t = c.GetType();
            if (HasComponent(t))
            {
                IComponent c2 = GetComponent(c.GetType());
                return c == c2;
            }

            return false;
        }

        public void AddOrReplaceComponent(IComponent c) => _entity.AddOrReplaceComponent(c);

        public bool RemoveComponent(Type t) => _entity.RemoveComponent(t);

        public bool CanRevertComponent(Type t) => _entity.CanRevertComponent(t);

        public virtual bool RevertComponent(Type t) => _entity.RevertComponent(t);

        public IComponent GetComponent(Type type) => _entity.GetComponent(type);

        /// <summary>
        /// Add an entity asset as a children of the current asset.
        /// Each of the children will be an instance of the current asset.
        /// </summary>
        public void AddChild(EntityInstance asset) => _entity.AddChild(asset);

        /// <summary>
        /// Add an entity asset as a children of the current asset.
        /// Each of the children will be an instance of the current asset.
        /// </summary>
        public bool RemoveChild(Guid instanceGuid) => _entity.RemoveChild(instanceGuid);

        /// <summary>
        /// Retrieve a child for this asset.
        /// </summary>
        public bool TryGetChild(Guid guid, [NotNullWhen(true)] out EntityInstance? instance) => _entity.TryGetChild(guid, out instance);

        public bool CanRemoveChild(Guid instanceGuid) => _entity.CanRemoveChild(instanceGuid);

        public bool AddOrReplaceComponentForChild(Guid childGuid, IComponent component) =>
            _entity.AddOrReplaceComponentForChild(childGuid, component);

        public void RemoveComponentForChild(Guid childGuid, Type t) =>
            _entity.RemoveComponentForChild(childGuid, t);

        public bool RevertComponentForChild(Guid childGuid, Type t) =>
            _entity.RevertComponentForChild(childGuid, t);

        public bool HasComponentAtChild(Guid childGuid, Type type) =>
            _entity.HasComponentAtChild(childGuid, type);

        public ImmutableArray<IComponent> GetChildComponents(Guid childGuid) =>
            _entity.GetChildComponents(childGuid);

        public IComponent? TryGetComponentForChild(Guid guid, Type t) =>
            _entity.TryGetComponentForChild(guid, t);

        /// <summary>
        /// If this is based on a prefab reference, enable access to its children modifiers when creating entities in the world.
        /// </summary>
        /// <returns></returns>
        internal ImmutableDictionary<Guid, EntityModifier>? GetChildrenModifiers() =>
            (_entity as PrefabEntityInstance)?.GetChildrenModifiers();
    }
}