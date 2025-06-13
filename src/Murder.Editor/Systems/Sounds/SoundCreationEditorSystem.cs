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
    [Filter(typeof(SoundComponent), typeof(SoundParameterComponent))]
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
                if (ImGui.Selectable("\uf2a2 Create event with distance tracking"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateSoundShape(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf2a2 Play event while on world"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewOnStartOnEndEventArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf2a2 Trigger when entering or exiting area"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewOnEnterOnExitPlayEventArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf2a2 Play ambience event while inside area"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateAmbienceArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf70c Set parameter when entering area"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewSoundTriggerArea(hook, cursorWorldPosition);
                }

                if (ImGui.Selectable("\uf70c Set parameter when entering or exiting area"))
                {
                    Point cursorWorldPosition = hook.LastCursorWorldPosition;
                    CreateNewOnEnterOnExitEventArea(hook, cursorWorldPosition);
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();

            return true;
        }

        private void CreateNewSoundTriggerArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new InteractiveComponent<SetSoundParameterOnInteraction>(new SetSoundParameterOnInteraction())
                ],
                /* group */ "Sounds",
                /* name */ "Set parameter when entering area");
        }

        private void CreateNewStartEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new InteractiveComponent<PlayEventInteraction>(new PlayEventInteraction())
                ],
                /* group */ "Sounds",
                /* name */ "Play event when entering area");
        }

        private void CreateNewStopEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new InteractOnCollisionComponent(playerOnly: true),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f)),
                    new InteractiveComponent<StopEventInteraction>(new StopEventInteraction())
                ],
                /* group */ "Sounds",
                /* name */ "Stop event when enter area");
        }

        private void CreateNewOnEnterOnExitEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
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
                ],
                /* group */ "Sounds",
                /* name */ "Set parameter when entering or exiting area");
        }

        private void CreateNewOnStartOnEndEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
                    new PositionComponent(position),
                    new InteractOnStartOnEndComponent(),
                    new InteractiveComponent<PlayEventInteraction>(),
                    new SoundParameterComponent()
                ],
                /* group */ "Sounds",
                /* name */ "Event while on world");
        }

        private void CreateNewOnEnterOnExitPlayEventArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
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
                ],
                /* group */ "Sounds",
                /* name */ "Trigger when entering or exiting area");
        }


        private void CreateAmbienceArea(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new AmbienceComponent(),
                    new ColliderComponent(
                        shape: new BoxShape(Vector2.Zero, Point.Zero, width: Grid.CellSize * 2, height: Grid.CellSize * 2),
                        layer: CollisionLayersBase.TRIGGER,
                        color: new Color(104 / 255f, 234 / 255f, 137 / 255f))
                ],
                /* group */ "Sounds",
                /* name */ "Play ambience while inside area");
        }

        private void CreateSoundShape(EditorHook hook, Vector2 position)
        {
            hook.AddEntityWithStage?.Invoke(
                [
                    new PositionComponent(position),
                    new SoundParameterComponent(),
                    new AmbienceComponent(),
                    new SoundShapeComponent([new Point(0, 0)]),
                ],
                /* group */ "Sounds",
                /* name */ "Event with distance tracking");
        }
    }
}