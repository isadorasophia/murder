using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// System that places an entity within the map.
    /// </summary>
    [WorldEditor]
    [Watch(typeof(IsPlacingComponent))]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(IsPlacingComponent))]
    internal class EntitiesPlacerSystem : IUpdateSystem, IReactiveSystem
    {
        public ValueTask Update(Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            DrawCreateEmptyEntity(context.World, hook);

            if (!hook.IsMouseOnStage || hook.EntityToBePlaced is null)
            {
                return default;
            }
            
            // If user has selected to destroy entities.
            bool destroy = Game.Input.Pressed(MurderInputButtons.Esc);

            bool clicked = Game.Input.Pressed(MurderInputButtons.LeftClick);
            bool doCopy = Game.Input.Down(MurderInputButtons.Shift);

            MonoWorld world = (MonoWorld)context.World;
            Point cursorPosition = world.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));

            foreach (Entity e in context.Entities)
            {
                e.SetTransform(new PositionComponent(cursorPosition));

                if (destroy)
                {
                    DestroyPlacerEntity(e, hook);
                }

                if (clicked)
                {
                    e.RemoveComponent(typeof(IsPlacingComponent));

                    string? targetGroup = EditorTileServices.FindTargetGroup(world, hook, cursorPosition);
                    
                    // Create itself from the hook and destroy this copy from the world.
                    hook.AddPrefabWithStage?.Invoke(hook.EntityToBePlaced.Value, new IComponent[] { e.GetTransform() }, targetGroup);

                    if (doCopy)
                    {
                        e.AddComponent(new IsPlacingComponent(), typeof(IsPlacingComponent));
                    }
                    else
                    {
                        // Only destroy if we are no longer interested in creating this entity.
                        DestroyPlacerEntity(e, hook);
                    }
                }
            }

            return default;
        }

        private void DestroyPlacerEntity(Entity e, EditorHook hook)
        {
            e.Destroy();
            hook.EntityToBePlaced = null;
        }
        
        public ValueTask OnAdded(World world, ImmutableArray<Entity> entities)
        {
            EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;
            Entity target = entities.Last();

            // Remove all other entities which also have the placing component.
            foreach (Entity e in world.GetEntitiesWith(typeof(IsPlacingComponent)))
            {
                if (e.EntityId != target.EntityId)
                {
                    e.Destroy();
                }
            }

            return default;
        }

        public ValueTask OnModified(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }

        public ValueTask OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            return default;
        }
        
        /// <summary>
        /// This draws and create a new empty entity if the user prompts.
        /// </summary>
        private bool DrawCreateEmptyEntity(World world, EditorHook hook)
        {
            ImGui.PushID("create_empty_entity");

            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.Selectable("Add empty entity!"))
                {
                    Point cursorWorldPosition = hook.CursorWorldPosition;
                    string? targetGroup = EditorTileServices.FindTargetGroup(world, hook, cursorWorldPosition);

                    hook.AddEntityWithStage?.Invoke(
                        new IComponent[]
                        {
                            new PositionComponent(cursorWorldPosition)
                        },
                        targetGroup);
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

            return true;
        }
    }
}
