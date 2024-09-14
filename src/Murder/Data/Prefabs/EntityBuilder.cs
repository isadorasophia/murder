using Bang;
using Bang.Components;
using Bang.Entities;
using Murder.Assets;
using Murder.Assets.Sounds;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Prefabs
{
    public static class EntityBuilder
    {
        /// <summary>
        /// This will replace an existing entity in the world.
        /// It keeps some elements of the original entity: position and target id components.
        /// </summary>
        /// <param name="world">World in which this entity exists.</param>
        /// <param name="e">The target entity which will be replaced.</param>
        /// <param name="asset">The guid of the prefab to be copied.</param>
        /// <param name="components">
        /// Custom components for the instance. This overrides any existing components
        /// in the entity asset.
        /// </param>
        /// <param name="children">Children for the instance.</param>
        /// <param name="modifiers">List of custom customizations to the prefab components.</param>
        internal static void Replace(
            World world,
            Entity e,
            Guid asset,
            in ImmutableArray<IComponent> components,
            in ImmutableArray<EntityInstance> children,
            in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            var builder = ImmutableArray.CreateBuilder<IComponent>();

            if (asset != Guid.Empty)
            {
                builder.Add(new PrefabRefComponent(asset));
            }

            if (e.TryGetIdTarget() is IdTargetComponent idTarget)
            {
                builder.Add(idTarget);
            }

            if (e.TryGetIdTargetCollection() is IdTargetCollectionComponent idTargetCollection)
            {
                builder.Add(idTargetCollection);
            }

            for (int i = 0; i < components.Length; ++i)
            {
                IComponent c = components[i];
                if (c is ITransformComponent && e.TryGetTransform() is IMurderTransformComponent transform)
                {
                    c = transform.WithoutParent();
                }
                else if (c is IModifiableComponent)
                {
                    c = SerializationHelper.DeepCopy(c);
                }

                builder.Add(c);
            }

            List<(int, string)> childrenIds = new();
            if (children.Length > 0)
            {
                // Now, create any children that we might have.
                childrenIds = CreateChildren(world, children, modifiers);
            }

            e.Replace(builder.ToArray(), childrenIds, wipe: true);

            PostProcessEntityEditorComponents(e);
        }

        /// <summary>
        /// Create entity at a particular position. This will override the default position of the parent.
        /// Called from <see cref="EntityInstance.Create(World, IEntity)"/>.
        /// </summary>
        /// <param name="asset">The guid of the prefab to be created.</param>
        /// <param name="world">World in which this entity will be added.</param>
        /// <param name="components">Custom components for the instance. This overrides any existing components
        /// in the entity asset.</param>
        /// <param name="children">Children for the instance.</param>
        /// <param name="modifiers">List of custom customizations to the prefab components.</param>
        /// <param name="id">Optional identifier of the entity within the world. If this conflicts with another existing id, it will be ignored.</param>
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
            IComponent[] instanceComponents = CreateDeepCopyComponentsFromAsset(asset, components);

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

            PostProcessEntityEditorComponents(entity);

            return entity.EntityId;
        }

        private static void PostProcessEntityEditorComponents(Entity e)
        {
            if (e.TryGetEventListenerEditor() is EventListenerEditorComponent eventListenerEditor)
            {
                e.RemoveEventListenerEditor();
                e.SetEventListener(eventListenerEditor.ToEventListener());
            }

            if (e.TryGetSpeakerEditorListener() is SpeakerEditorListenerComponent eventSpeakerEditor)
            {
                e.RemoveSpeakerEditorListener();

                if (eventSpeakerEditor.Speaker.TryAsset is SpeakerAsset speaker && 
                    speaker.Events?.TryAsset is SpeakerEventsAsset speakerEvents)
                {
                    e.SetEventListener(speakerEvents.Events);
                }
            }

            if (e.TryGetOverrideSpriteFacingEditor() is OverrideSpriteFacingEditorComponent overrideFacing)
            {
                e.RemoveOverrideSpriteFacingEditor();

                var builder = ImmutableDictionary.CreateBuilder<string, SpriteFacingComponent>();
                foreach (var face in overrideFacing.SpriteFaces)
                {
                    builder[face.Id] = face.Info;
                }

                e.SetOverrideSpriteFacing(builder.ToImmutable());
            }
        }

        private static IComponent[] CreateDeepCopyComponentsFromAsset(
            Guid asset,
            in ImmutableArray<IComponent> components)
        {
            bool addPrefabRef = asset != Guid.Empty;

            IComponent[] result = new IComponent[addPrefabRef ? components.Length + 1 : components.Length];
            for (int i = 0; i < components.Length; ++i)
            {
                IComponent c = components[i];
                if (c is IModifiableComponent)
                {
                    c = SerializationHelper.DeepCopy(c);
                }

                result[i] = c;
            }

            if (addPrefabRef)
            {
                // Do not bother adding entity asset for empty assets.
                result[components.Length] = new PrefabRefComponent(asset);
            }

            return result;
        }

        /// <summary>
        /// Create all the children to <paramref name="world"/>.
        /// </summary>
        /// <returns>
        /// List of all children created within the world.
        /// </returns>
        private static List<(int id, string name)> CreateChildren(
            World world,
            in ImmutableArray<EntityInstance> children,
            in ImmutableDictionary<Guid, EntityModifier> modifiers)
        {
            GameLogger.Verify(children.Length > 0, "Why are we creating children if there is none!?");

            List<(int id, string name)> createdChildren = new();
            foreach (EntityInstance child in children)
            {
                int entityId = child.Create(world, modifiers);
                createdChildren.Add((entityId, child.Name));

                if (child.IsDeactivated)
                {
                    world.TryGetEntity(entityId)?.Deactivate();
                }
            }

            return createdChildren;
        }

        public static EntityInstance CreateInstance(Guid assetGuid, string? name = default, Guid? instanceGuid = null)
        {
            if (assetGuid == Guid.Empty)
            {
                return new EntityInstance(name);
            }

            return new PrefabEntityInstance(new(assetGuid), name, ignorePrefabChildren: false, instanceGuid);
        }
    }
}