using Bang.Components;
using Bang.Contexts;
using Bang.Interactions;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Effects;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Interactions;
using System.Numerics;

namespace Murder.Editor.Systems.Sounds
{
    [SoundEditor]
    [Filter(typeof(SoundComponent), typeof(MusicComponent), typeof(SoundParameterComponent))]
    internal class SoundCreationEditorSystem : IMurderRenderSystem
    {
        public void Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            DrawAddEntity(hook);
        }

        private bool DrawAddEntity(EditorHook hook)
        {
            ImGui.PushID("Popup Sound!");

            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.Selectable("\uf2a2 Add sound trigger"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewSoundTriggerArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf04b Add start event"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewStartEventArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf04d Add stop event"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewStopEventArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf70c On enter/exit"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewOnEnterOnExitEventArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf70c Add play/stop"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewOnEnterOnExitPlayEventArea(hook, cursorWorldPosition);
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

            return true;
        }

        private void CreateNewSoundTriggerArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                new IComponent[]
                {
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new InteractiveComponent<SetSoundParameterOnInteraction>(new SetSoundParameterOnInteraction())
                },
                /* group */ "Sounds",
                /* name */ "Sound Trigger Area");
        }

        private void CreateNewStartEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                new IComponent[]
                {
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new InteractiveComponent<PlayEventInteraction>(new PlayEventInteraction())
                },
                /* group */ "Sounds",
                /* name */ "Event Trigger Area");
        }

        private void CreateNewStopEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                new IComponent[]
                {
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new InteractiveComponent<StopEventInteraction>(new StopEventInteraction())
                },
                /* group */ "Sounds",
                /* name */ "Event Trigger Area");
        }

        private void CreateNewOnEnterOnExitEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                new IComponent[]
                {
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true, sendMessageOnExit: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new OnEnterOnExitComponent(
                        new InteractiveComponent<SetSoundParameterOnInteraction>(new SetSoundParameterOnInteraction()),
                        new InteractiveComponent<SetSoundParameterOnInteraction>(new SetSoundParameterOnInteraction()))
                },
                /* group */ "Sounds",
                /* name */ "Event Trigger Area");
        }

        private void CreateNewOnEnterOnExitPlayEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                new IComponent[]
                {
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true, sendMessageOnExit: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new OnEnterOnExitComponent(
                        new InteractiveComponent<PlayEventInteraction>(new PlayEventInteraction()),
                        new InteractiveComponent<StopEventInteraction>(new StopEventInteraction()))
                },
                /* group */ "Sounds",
                /* name */ "Event Trigger Area");
        }
    }
}