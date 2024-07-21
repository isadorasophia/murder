using Bang;
using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Services;
using Murder.Editor.Utilities;
using Murder.Utilities;
using System.Collections.Immutable;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// System that places an entity within the map.
    /// </summary>
    [EditorSystem]
    [WorldEditor(startActive: true)]
    [Watch(typeof(IsPlacingComponent))]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(IsPlacingComponent))]
    internal class EntitiesPlacerSystem : IUpdateSystem, IReactiveSystem, IMurderRenderSystem
    {
        public void Update(Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            if (!hook.IsMouseOnStage || hook.EntityToBePlaced is null)
            {
                return;
            }
            MonoWorld world = (MonoWorld)context.World;
            if (hook.CursorWorldPosition is not Point cursorPosition)
                return;

            // If user has selected to destroy entities.
            bool destroy = Game.Input.Pressed(MurderInputButtons.Esc);

            bool clicked = Game.Input.Pressed(MurderInputButtons.LeftClick);
            bool doCopy = Game.Input.Down(MurderInputButtons.Shift);


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
                    hook.AddPrefabWithStage?.Invoke(hook.EntityToBePlaced.Value, [e.GetTransform()], targetGroup);

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
        }

        private void DestroyPlacerEntity(Entity e, EditorHook hook)
        {
            e.Destroy();
            hook.EntityToBePlaced = null;
        }

        public void OnAdded(World world, ImmutableArray<Entity> entities)
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
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        { }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        { }

        /// <summary>
        /// This draws and create a new empty entity if the user prompts.
        /// </summary>
        private bool DrawCreateEmptyEntity(World world, EditorHook hook)
        {
            if (ImGui.BeginPopupContextItem("GameplayContextMenu", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.NoReopen))
            {
                Point screenPosition = ImGui.GetMousePosOnOpeningCurrentPopup().Point();

                if (ImGui.Selectable("Add empty entity"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    string? targetGroup = EditorTileServices.FindTargetGroup(world, hook, cursorWorldPosition);

                    hook.AddEntityWithStage?.Invoke(
                        [
                            new PositionComponent(cursorWorldPosition)
                        ],
                        targetGroup,
                        /* name */ null);
                }

                if (ImGui.Selectable("Add unique prop"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    string? targetGroup = EditorTileServices.FindTargetGroup(world, hook, cursorWorldPosition);

                    hook.AddEntityWithStage?.Invoke(
                        [
                            new PositionComponent(cursorWorldPosition),
                            new SpriteComponent(),
                        ],
                        targetGroup,
                        /* name */ null);
                }

                ImGui.EndPopup();
            }

            return true;
        }

        public void Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            DrawCreateEmptyEntity(context.World, hook);
        }
    }
}