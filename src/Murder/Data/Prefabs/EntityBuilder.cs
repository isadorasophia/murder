using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Prefabs
{
    public static class EntityBuilder
    {
        /// <summary>
        /// Create entity at a particular position. This will override the default position of the parent.
        /// Called from <see cref="EntityInstance.Create(World, IEntity)"/>.
        /// </summary>
        /// <param name="world">World in which this entity will be added.</param>
        /// <param name="components">Custom components for the instance. This overrides any existing components
        /// in the entity asset.</param>
        /// <param name="children">Children for the instance.</param>
        /// <returns>
        /// Parent entity id.
        /// TODO: Do we need to return the id of all the children as well?
        /// </returns>
        internal static int Create(
            World world,
            Guid asset,
            in ImmutableArray<IComponent> components,
            in ImmutableArray<EntityInstance> children,
            in ImmutableDictionary<Guid, EntityModifier> modifiers,
            int? id = default)
        {
            List<IComponent> instanceComponents = new();
            foreach (IComponent c in components)
            {
                IComponent component = c is IModifiableComponent ? SerializationHelper.DeepCopy(c) : c;
                instanceComponents.Add(component);
            }

            // Do not bother adding entity asset for empty assets.
            if (asset != Guid.Empty)
            {
                instanceComponents.Add(new PrefabRefComponent(asset));
            }

            Entity entity = world.AddEntity(id, instanceComponents);

            // Now, add any children that we might have.
            if (children.Length > 0)
            {
                IList<(int, string)> childrenIds = CreateChildren(world, children, modifiers);
                foreach (var (childId, childName) in childrenIds)
                {
                    entity.AddChild(childId, childName);
                }
            }

            return entity.EntityId;
        }

        /// <summary>
        /// Create all the children to <paramref name="world"/>.
        /// This will tie each of the created children to the parent entity with <paramref name="parentId"/>.
        /// </summary>
        /// <returns>
        /// List of all children created within the world.
        /// </returns>
        private static IList<(int id, string name)> CreateChildren(
            World world,
            in ImmutableArray<EntityInstance> children,
            in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            GameLogger.Verify(children.Length > 0, "Why are we creating children if there is none!?");

            List<(int id, string name)> createdChildren = new();
            foreach (EntityInstance child in children)
            {
                createdChildren.Add((child.Create(world, modifiers), child.Name));
            }

            return createdChildren;
        }

        public static EntityInstance CreateInstance(Guid assetGuid, string? name = default)
        {
            if (assetGuid == Guid.Empty)
            {
                return new EntityInstance(name);
            }

            return new PrefabEntityInstance(new(assetGuid), name, ignoreChildren: false);
        }
    }
}
