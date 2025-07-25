﻿using Bang.Components;
using Bang.Interactions;
using Bang.Systems;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Components;
using Murder.Components.Serialization;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.CustomEditors;
using Murder.Editor.Data.Graphics;
using Murder.Editor.Utilities.Attributes;
using Murder.Interactions;
using Murder.Prefabs;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;
using System.Reflection;

namespace Murder.Editor.Utilities;

public static class StageHelpers
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

    public static Vector2? GetPositionForSelectedEntity()
    {
        if (Architect.Instance.ActiveScene is EditorScene editor &&
            editor.EditorShown is AssetEditor assetEditor)
        {
            return assetEditor.SelectedEntity?.GetComponent<PositionComponent>().ToVector2();
        }

        return null;
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

        if (Architect.Instance.ActiveScene is EditorScene editorScene &&
            editorScene.EditorShown is CustomEditor editor)
        {
            if (editor.SelectedEntity is not IEntity selectedEntity)
            {
                return null;
            }

            CutsceneAnchorsEditorComponent? cutscene = null;
            if (selectedEntity.HasComponent(typeof(CutsceneAnchorsEditorComponent)))
            {
                cutscene = selectedEntity.GetComponent<CutsceneAnchorsEditorComponent>();
            }
            else if (Game.Data.TryGetAsset(editor.WorldReference) is WorldAsset world)
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
                    EntityInstance? instance = world.TryGetInstance(target.Value);
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

        if (Architect.Instance.ActiveScene is EditorScene editorScene &&
            editorScene.EditorShown is CustomEditor editor)
        {
            if (editor.SelectedEntity is not IEntity selectedEntity)
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

            WorldAsset? world = Game.Data.TryGetAsset(editor.WorldReference) as WorldAsset;

            if (target is not null)
            {
                EntityInstance? instance = world?.TryGetInstance(target.Value);
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

            if (guidToIdCollection is null || guidToIdCollection.Value.Collection.Length == 0)
            {
                foreach (Guid child in selectedEntity.Children)
                {
                    foreach (IComponent c in selectedEntity.GetChildComponents(child))
                    {
                        if (c is GuidToIdTargetCollectionComponent collection)
                        {
                            guidToIdCollection = collection;
                            break;
                        }
                    }
                }
            }

            return guidToIdCollection?.Collection.Select(a => a.Id).ToHashSet();
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

        (string[] Animations, HashSet<string> Events)? spriteProperties = 
            spriteAsset is null ? null : GetSpriteEventsForAsset(spriteAsset);

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

        if (Architect.Game?.CustomHelper is IStageCustomHelper customHelper)
        {
            customHelper.AddEventsForSelectedEntity(target, ref events);
        }

        if (events is null)
        {
            return null;
        }

        return (animations ?? [], events);
    }

    private static void AddEventsIfAny(IComponent c, ref HashSet<string>? events, bool child)
    {
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

        SpriteAsset? sprite = null;
        if (c is SpriteComponent spriteComponent)
        {
            sprite = Game.Data.TryGetAsset<SpriteAsset>(spriteComponent.AnimationGuid);
        }
        else if (c is AgentSpriteComponent agentSpriteComponent)
        {
            sprite = Game.Data.TryGetAsset<SpriteAsset>(agentSpriteComponent.AnimationGuid);
        }

        if (sprite is not null)
        {
            AddEventsFromSprite(sprite, ref events);
            return;
        }

        CheckForAttributeEventsOnType(c.GetType(), c, child, ref events);
    }

    private static void CheckForAttributeEventsOnType(Type t, IComponent c, bool isChildComponent, ref HashSet<string>? events)
    {
        Type tBase = t;
        if (t.IsGenericType)
        {
            t = t.GetGenericArguments()[0];
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

        if (isChildComponent && !attribute.Flags.HasFlag(EventMessageAttributeFlags.PropagateToParent))
        {
            return;
        }

        events ??= new();

        foreach (string e in attribute.Events)
        {
            events.Add(e);
        }

        if (attribute.CheckForSpriteFields is string[] fieldsToCheck)
        {
            foreach (string f in fieldsToCheck)
            {
                FieldInfo? fInfo = t.GetField(f, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (fInfo is null)
                {
                    continue;
                }

                object? cToGetField = c;
                if (tBase.IsGenericType && tBase.GetGenericTypeDefinition() == typeof(InteractiveComponent<>))
                {
                    FieldInfo? fInteraction = tBase
                        .GetField("_interaction", BindingFlags.NonPublic | BindingFlags.Instance);

                    cToGetField = fInteraction?.GetValue(cToGetField);
                }

                object? value = fInfo.GetValue(cToGetField);
                if (value is Guid spriteGuid)
                {
                    AddEventsFromSprite(spriteGuid, ref events);
                }
                else if (value is Portrait portrait)
                {
                    AddEventsFromPortrait(portrait, ref events);
                }
            }
        }

        if (t.BaseType is Type tParent &&
            (tParent.Assembly == t.Assembly || tParent.Assembly == typeof(IComponent).Assembly))
        {
            CheckForAttributeEventsOnType(tParent, c, isChildComponent, ref events);
        }
    }

    private static void AddEventsFromSprite(Guid spriteGuid, ref HashSet<string>? events)
    {
        SpriteAsset? sprite = Game.Data.TryGetAsset<SpriteAsset>(spriteGuid);
        if (sprite is null)
        {
            return;
        }

        AddEventsFromSprite(sprite, ref events);
    }

    private static void AddEventsFromSprite(SpriteAsset sprite, ref HashSet<string>? events)
    {
        events ??= [];

        (_, HashSet<string> spriteEventsForChild) = GetSpriteEventsForAsset(sprite);
        foreach (string e in spriteEventsForChild)
        {
            events.Add(e);
        }
    }

    private static void AddEventsFromPortrait(Portrait portrait, ref HashSet<string>? events)
    {
        SpriteAsset? sprite = Game.Data.TryGetAsset<SpriteAsset>(portrait.Sprite);
        if (sprite is null)
        {
            return;
        }

        events ??= [];

        if (sprite.Animations.TryGetValue(portrait.AnimationId, out Animation animation))
        {
            foreach ((_, var @event) in animation.Events)
            {
                events.Add(@event);
            }
        }
    }

    public static (string[] Animations, HashSet<string> Events) GetSpriteEventsForAsset(SpriteAsset asset)
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