using Bang.Components;
using Bang.Systems;
using Murder.Assets;
using Murder.Components;
using Murder.Components.Cutscenes;
using Murder.Diagnostics;
using Murder.Editor.CustomEditors;
using Murder.Prefabs;
using System.Text;

namespace Murder.Editor.Utilities
{
    internal static class StageHelpers
    {
        public static IList<(ISystem system, bool isActive)> FetchEditorSystems()
        {
            List<(ISystem, bool)> systems = new();

            foreach ((Type t, bool isActive) in Architect.EditorSettings.EditorSystems)
            {
                if (t is null)
                {
                    GameLogger.Error("Skipping system not found in editor systems.");
                    continue;
                }

                if (Activator.CreateInstance(t) is ISystem system)
                {
                    systems.Add((system, isActive));
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

                CutsceneAnchorsComponent? cutscene = null;
                if (selectedEntity.HasComponent(typeof(CutsceneAnchorsComponent)))
                {
                    cutscene = selectedEntity.GetComponent<CutsceneAnchorsComponent>();
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
                        if (selectedEntity.GetComponent<GuidToIdTargetCollectionComponent>().Targets.TryGetValue(cutsceneName, out Guid result))
                        {
                            target = result;
                        }
                    }

                    if (target is not null)
                    {
                        EntityInstance? instance = world.TryFindInstance(target.Value);
                        if (instance?.HasComponent(typeof(CutsceneAnchorsComponent)) ?? false)
                        {
                            cutscene = (CutsceneAnchorsComponent)instance.GetComponent(typeof(CutsceneAnchorsComponent));
                        }
                    }
                }

                if (cutscene is null)
                {
                    return null;
                }

                return cutscene.Value.Anchors.Keys.ToHashSet();
            }

            return null;
        }
    }
}
