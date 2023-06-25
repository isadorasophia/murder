using Bang.Components;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Components.Serialization;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomEditors;
using Murder.Editor.Data.Graphics;
using Murder.Prefabs;
using System.Collections.Immutable;
using System.Reflection;

namespace Murder.Editor.Utilities
{
    internal static class StageHelpers
    {
        public static IList<(ISystem system, bool isActive)> FetchEditorSystems()
        {
            HashSet<Type> systemsAdded = new();

            List<(ISystem, bool)> systems = new();

            ImmutableArray<(Type systemType, bool isActive)> editorSystems = Architect.EditorSettings.EditorSystems;
            for (int i = 0; i < editorSystems.Length; ++i)
            {
                (Type t, bool isActive) = editorSystems[i];
                if (systemsAdded.Contains(t))
                {
                    // Already added, so skip.
                    continue;
                }

                if (t is null)
                {
                    // Remove and save the editor without this system.
                    Architect.EditorSettings.UpdateSystems(editorSystems.RemoveAt(i));
                    Architect.EditorData.SaveAsset(Architect.EditorSettings);

                    continue;
                }

                if (Activator.CreateInstance(t) is ISystem system)
                {
                    systems.Add((system, isActive));
                    systemsAdded.Add(t);
                }
                else
                {
                    GameLogger.Error($"The {t} is not a valid system!");
                }
            }

            // Fetch all the systems that are not included in the editor system.
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<WorldEditorAttribute>(typeof(ISystem)))
            {
                if (systemsAdded.Contains(t))
                {
                    // Already added, so skip.
                    continue;
                }

                WorldEditorAttribute worldAttribute = (WorldEditorAttribute)t.GetCustomAttribute(typeof(WorldEditorAttribute))!;
                bool isActive = worldAttribute.StartActive;

                if (Activator.CreateInstance(t) is ISystem system)
                {
                    systems.Add((system, isActive));
                    systemsAdded.Add(t);
                }
                else
                {
                    GameLogger.Error($"The {t} is not a valid system!");
                }
            }

            // Also start all story editors disabled by default.
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<StoryEditorAttribute>(typeof(ISystem)))
            {
                if (systemsAdded.Contains(t))
                {
                    // Already added, so skip.
                    continue;
                }

                if (Activator.CreateInstance(t) is ISystem system)
                {
                    systems.Add((system, /* isActive */ false));
                    systemsAdded.Add(t);
                }
                else
                {
                    GameLogger.Error($"The {t} is not a valid system!");
                }
            }

            // Aaaaand also start all sound editors.
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<SoundEditorAttribute>(typeof(ISystem)))
            {
                if (systemsAdded.Contains(t))
                {
                    // Already added, so skip.
                    continue;
                }

                if (Activator.CreateInstance(t) is ISystem system)
                {
                    systems.Add((system, /* isActive */ false));
                    systemsAdded.Add(t);
                }
                else
                {
                    GameLogger.Error($"The {t} is not a valid system!");
                }
            }

            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<DefaultEditorSystemAttribute>(typeof(ISystem)))
            {
                if (systemsAdded.Contains(t))
                {
                    // Already added, so skip.
                    continue;
                }

                if (Activator.CreateInstance(t) is ISystem system)
                {
                    systems.Add((system, true));
                    systemsAdded.Add(t);
                }
                else
                {
                    GameLogger.Error($"The {t} is not a valid system!");
                }
            }


            return systems;
        }

        /// <summary>
        /// Cache a dictionary that maps attributes -> components that have that attribute.
        /// This is used by <see cref="FetchComponentsWithAttribute"/>.
        /// </summary>
        private static readonly Dictionary<Type, Type[]> _componentsWithStoryCache = new();

        public static Type[] FetchComponentsWithAttribute<T>(bool cache = true) where T : Attribute
        {
            if (_componentsWithStoryCache.TryGetValue(typeof(T), out Type[]? result))
            {
                return result;
            }

            result = ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<T>(typeof(IComponent)).ToArray();
            if (cache)
            {
                _componentsWithStoryCache[typeof(T)] = result;
            }

            return result;
        }

        public static HashSet<string>? GetChildNamesForSelectedEntity()
        {
            if (Architect.Instance.ActiveScene is EditorScene editor &&
                editor.EditorShown is AssetEditor assetEditor)
            {
                return assetEditor.SelectedEntity?.FetchChildren().Select(i => i.Name).ToHashSet();
            }

            return null;
        }

        public static HashSet<string>? GetAnchorNamesForSelectedEntity()
        {
            const string cutsceneName = "cutscene";

            if (Architect.Instance.ActiveScene is EditorScene editor &&
                editor.EditorShown is AssetEditor assetEditor)
            {
                if (assetEditor.SelectedEntity is not IEntity selectedEntity)
                {
                    return null;
                }

                CutsceneAnchorsEditorComponent? cutscene = null;
                if (selectedEntity.HasComponent(typeof(CutsceneAnchorsEditorComponent)))
                {
                    cutscene = selectedEntity.GetComponent<CutsceneAnchorsEditorComponent>();
                }
                else if (assetEditor is WorldAssetEditor world)
                {
                    Guid? target = null;
                    if (selectedEntity.HasComponent(typeof(GuidToIdTargetComponent)))
                    {
                        target = selectedEntity.GetComponent<GuidToIdTargetComponent>().Target;
                    }
                    else if (selectedEntity.HasComponent(typeof(GuidToIdTargetCollectionComponent)))
                    {
                        if (selectedEntity.GetComponent<GuidToIdTargetCollectionComponent>().TryFindGuid(cutsceneName) is Guid result)
                        {
                            target = result;
                        }
                    }

                    if (target is not null)
                    {
                        EntityInstance? instance = world.TryFindInstance(target.Value);
                        if (instance?.HasComponent(typeof(CutsceneAnchorsEditorComponent)) ?? false)
                        {
                            cutscene = (CutsceneAnchorsEditorComponent)instance.GetComponent(typeof(CutsceneAnchorsEditorComponent));
                        }
                    }
                }

                if (cutscene is null)
                {
                    return null;
                }

                return cutscene.Value.Anchors.Select(a => a.Id).ToHashSet();
            }

            return null;
        }

        public static HashSet<string>? GetTargetNamesForSelectedEntity()
        {
            const string cutsceneName = "cutscene";

            if (Architect.Instance.ActiveScene is EditorScene editor &&
                editor.EditorShown is WorldAssetEditor worldEditor)
            {
                if (worldEditor.SelectedEntity is not IEntity selectedEntity)
                {
                    return null;
                }

                GuidToIdTargetCollectionComponent? guidToIdCollection = null;

                Guid? target = null;
                if (selectedEntity.HasComponent(typeof(GuidToIdTargetComponent)))
                {
                    target = selectedEntity.GetComponent<GuidToIdTargetComponent>().Target;
                }
                else if (selectedEntity.HasComponent(typeof(GuidToIdTargetCollectionComponent)))
                {
                    if (selectedEntity.GetComponent<GuidToIdTargetCollectionComponent>().TryFindGuid(cutsceneName) is Guid result)
                    {
                        target = result;
                    }
                }

                if (target is not null)
                {
                    EntityInstance? instance = worldEditor.TryFindInstance(target.Value);
                    if (instance?.HasComponent(typeof(GuidToIdTargetCollectionComponent)) ?? false)
                    {
                        guidToIdCollection = (GuidToIdTargetCollectionComponent)instance.GetComponent(typeof(GuidToIdTargetCollectionComponent));
                    }
                }
                else
                {
                    if (selectedEntity.HasComponent(typeof(GuidToIdTargetCollectionComponent)))
                    {
                        guidToIdCollection = selectedEntity.GetComponent<GuidToIdTargetCollectionComponent>();
                    }
                }

                if (guidToIdCollection is null)
                {
                    return null;
                }

                return guidToIdCollection.Value.Collection.Select(a => a.Id).ToHashSet();
            }

            return null;
        }

        public static IEntity? GetSelectedEntity()
        {
            if (Architect.Instance.ActiveScene is EditorScene editor && editor.EditorShown is AssetEditor assetEditor)
            {
                return assetEditor.SelectedEntity;
            }

            return null;
        }

        public static bool AddComponentsOnSelectedEntityForWorldOnly(params IComponent[] components)
        {
            if (Architect.Instance.ActiveScene is not EditorScene editor || editor.EditorShown is not AssetEditor assetEditor)
            {
                return false;
            }

            IEntity? target = assetEditor.SelectedEntity;
            if (target is null)
            {
                return false;
            }

            assetEditor.AddComponentsToWorldOnly(target, components);
            return true;
        }

        public static (string[] Animations, HashSet<string> Events)? GetSpriteEventsForSelectedEntity()
        {
            IEntity? target = GetSelectedEntity();
            if (target is null)
            {
                return null;
            }

            HashSet<string> animations = new();
            HashSet<string> events = new();

            SpriteAsset? asset = null;
            if (target.HasComponent(typeof(SpriteComponent)))
            {
                SpriteComponent sprite = target.GetComponent<SpriteComponent>();
                asset = Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid);
            }
            else if (target.HasComponent(typeof(AgentSpriteComponent)))
            {
                AgentSpriteComponent sprite = target.GetComponent<AgentSpriteComponent>();
                asset = Game.Data.TryGetAsset<SpriteAsset>(sprite.AnimationGuid);
            }

            if (asset is null)
            {
                return null;
            }

            // Iterate over all animations and grab all the possible events.
            foreach ((string name, Animation animation) in asset.Animations)
            {
                animations.Add(name);

                foreach ((_, var @event) in animation.Events)
                {
                    events.Add(@event);
                }
            }

            return (animations.ToArray(), events);
        }
    }
}
