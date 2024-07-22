using Bang.Components;
using Bang.Interactions;
using Bang.Systems;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Components;
using Murder.Components.Serialization;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomEditors;
using Murder.Editor.Utilities.Attributes;
using Murder.Interactions;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Numerics;
using System.Reflection;

namespace Murder.Editor.Utilities;

internal static class StageHelpers
{
    private static List<(Type tSystem, bool isActive)>? _cachedEditorSystems = null;

    public static List<(Type tSystem, bool isActive)> FetchEditorTypeSystems()
    {
        if (_cachedEditorSystems is not null)
        {
            return _cachedEditorSystems;
        }
        
        HashSet<Type> systemsAdded = new();
        List<(Type, bool)> systems = new();

        void AddSystem(Type t, bool isActive)
        {
            if (systemsAdded.Contains(t))
            {
                // Already added, so skip.
                return;
            }

            if (Attribute.GetCustomAttribute(t, typeof(RequiresAttribute)) is RequiresAttribute requiresAttribute)
            {
                foreach (Type tt in requiresAttribute.Types)
                {
                    AddSystem(tt, isActive);
                }
            }

            systems.Add((t, isActive));
            systemsAdded.Add(t);
        }

        // Fetch all the systems that are not included in the editor system.
        foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<WorldEditorAttribute>(typeof(ISystem)))
        {
            WorldEditorAttribute worldAttribute = (WorldEditorAttribute)t.GetCustomAttribute(typeof(WorldEditorAttribute))!;
            bool isActive = worldAttribute.StartActive;

            AddSystem(t, isActive);
        }

        foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<DefaultEditorSystemAttribute>(typeof(ISystem)))
        {
            DefaultEditorSystemAttribute attribute = (DefaultEditorSystemAttribute)t.GetCustomAttribute(typeof(DefaultEditorSystemAttribute))!;
            bool isActive = attribute.StartActive;

            AddSystem(t, isActive);
        }

        // Start with all the editor systems enabled by default.
        foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType<EditorSystemAttribute>(typeof(ISystem)))
        {
            AddSystem(t, isActive: true);
        }

        Type[] targetAttributesForDisabledByDefault = [
            typeof(StoryEditorAttribute),
            typeof(DialogueEditorAttribute),
            typeof(TileEditorAttribute),
            typeof(PathfindEditorAttribute),
            typeof(SoundEditorAttribute),
            typeof(SpriteEditorAttribute),
            typeof(PrefabEditorAttribute),
            typeof(EditorSystemAttribute)];

        foreach (Type tAttribute in targetAttributesForDisabledByDefault)
        {
            foreach (Type t in ReflectionHelper.GetAllTypesWithAttributeDefinedOfType(tAttribute, typeof(ISystem)))
            {
                AddSystem(t, isActive: false);
            }
        }

        _cachedEditorSystems = systems;
        return systems;
    }

    public static IList<(ISystem system, bool isActive)> FetchEditorSystems()
    {
        List<(Type, bool)> systems = FetchEditorTypeSystems();
        List<(ISystem, bool)> result = [];

        foreach ((Type t, bool enabled) in systems)
        {
            if (Activator.CreateInstance(t) is ISystem system)
            {
                result.Add((system, enabled));
            }
            else
            {
                GameLogger.Error($"The {t} is not a valid system!");
            }
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

    public static Vector2? GetSelectedEntityPosition()
    {
        if(GetSelectedEntity() is IEntity entity && entity.HasComponent(typeof(PositionComponent)))
        {
            return entity.GetComponent<PositionComponent>().ToVector2();
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

    public static bool ToggleSystem(Type t, bool enable)
    {
        if (Architect.Instance.ActiveScene is not EditorScene editor || editor.EditorShown is not AssetEditor assetEditor)
        {
            return false;
        }

        assetEditor.ToggleSystem(t, enable);

        return true;
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

    public static SpriteAsset? GetSpriteAssetForSelectedEntity()
    {
        IEntity? target = GetSelectedEntity();
        if (target is null)
        {
            return null;
        }

        return GetSpriteAssetForEntity(target);
    }

    public static SpriteAsset? GetSpriteAssetForEntity(IEntity target)
    {
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

        return asset;
    }

    public static (string[] Animations, HashSet<string> Events)? GetEventsForSelectedEntity()
    {
        IEntity? target = GetSelectedEntity();
        if (target is null)
        {
            return null;
        }

        SpriteAsset? spriteAsset = GetSpriteAssetForEntity(target); 

        var spriteProperties = spriteAsset is null ? null : GetSpriteEventsForAsset(spriteAsset);

        string[]? animations = spriteProperties?.Animations;
        HashSet<string>? events = spriteProperties?.Events;

        foreach (IComponent c in target.Components)
        {
            AddEventsIfAny(c, ref events, child: false);
        }

        foreach (Guid child in target.Children)
        {
            foreach (IComponent c in target.GetChildComponents(child))
            {
                AddEventsIfAny(c, ref events, child: true);
            }
        }

        if (events is null)
        {
            return null;
        }

        return (animations ?? [], events);
    }

    private static void AddEventsIfAny(IComponent c, ref HashSet<string>? events, bool child)
    {
        Type t = c.GetType();
        if (t.IsGenericType)
        {
            t = t.GetGenericArguments()[0];
        }

        if (c is InteractiveComponent<InteractionCollection>)
        {
            FieldInfo? f = typeof(InteractiveComponent<InteractionCollection>)
                .GetField("_interaction", BindingFlags.NonPublic | BindingFlags.Instance);

            if (f?.GetValue(c) is InteractionCollection collection)
            {
                foreach (IInteractiveComponent i in collection.Interactives)
                {
                    AddEventsIfAny(i, ref events, child);
                }
            }
        }

        if (Attribute.GetCustomAttribute(t, typeof(EventMessagesAttribute)) is not EventMessagesAttribute attribute)
        {
            return;
        }

        if (attribute.Flags.HasFlag(EventMessageAttributeFlags.CheckDisplayOnlyIf))
        {
            MethodInfo? method = t.GetMethod("VerifyEventMessages", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method is null)
            {
                return;
            }

            if (method.Invoke(c, parameters: null) is not bool result || result is false)
            {
                return;
            }
        }

        if (child && !attribute.Flags.HasFlag(EventMessageAttributeFlags.PropagateToParent))
        {
            return;
        }

        events ??= new();

        foreach (string e in attribute.Events)
        {
            events.Add(e);
        }
    }

    public static (string[] Animations, HashSet<string> Events)? GetSpriteEventsForAsset(SpriteAsset asset)
    {
        HashSet<string> animations = new();
        HashSet<string> events = new();

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