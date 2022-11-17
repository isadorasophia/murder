using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Core.Geometry;
using Murder.Prefabs;
using Murder.Serialization;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Murder.Assets
{
    public class PrefabAsset : GameAsset, IEntity
    {
        private const char SingleIcon = '\uf11a';
        private const char FamilyIcon = '\uf500';

        public override char Icon => Children.Length == 0 ? SingleIcon : FamilyIcon;
        public override string EditorFolder => "#\uf0e8Prefabs";
        public override Vector4 EditorColor => new Vector4(0.75f, 0.45f, 1, 1);
        public override string SaveLocation => Path.Join(Game.Profile.ContentECSPath, FileHelper.Clean(EditorFolder));

        [JsonProperty]
        private readonly EntityInstance _entity = new();

        /// <summary>
        /// Dimensions of the prefab. Used when drawing it on the map or the editor.
        /// </summary>
        [JsonProperty]
        public IntRectangle Dimensions = IntRectangle.One;

        /// <summary>
        /// Create an instance of the entity and all of its children.
        /// </summary>
        public int Create(World world) =>
            EntityBuilder.Create(world, Guid, Components, FetchChildren(), ImmutableDictionary<Guid, EntityModifier>.Empty);

        /// <summary>
        /// Create an instance of the entity and all of its children with some custom components.
        /// </summary>
        public int Create(World world, params IComponent[] components)
        {
            var builder = ImmutableArray.CreateBuilder<IComponent>();

            builder.AddRange(Components);

            foreach (IComponent c in components)
            {
                Type t = c.GetType();

                if (HasComponent(t))
                {
                    builder.Remove(GetComponent(t));
                }

                builder.Add(c);
            }

            return EntityBuilder.Create(world, Guid, builder.ToImmutable(), FetchChildren(), ImmutableDictionary<Guid, EntityModifier>.Empty);
        }

        public Entity CreateAndFetch(World world) =>
            world.GetEntity(EntityBuilder.Create(world, Guid, Components, FetchChildren(), ImmutableDictionary<Guid, EntityModifier>.Empty));

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

        public IntRectangle GetBoundingBoxFromTile(Point tile)
        {
            return new IntRectangle(tile + Dimensions.TopLeft, Dimensions.Size);
        }
    }
}
