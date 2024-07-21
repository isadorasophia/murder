using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using System.Numerics;
using Murder.Utilities;
using Murder.Services;
using Murder.Core.Geometry;
using Murder.Editor.Utilities;
using Murder.Editor.Components;
using Murder.Core.Input;
using Murder.Editor.Messages;
using ImGuiNET;
using Murder.Core.Sounds;
using Murder.Systems.Sound;

namespace Murder.Editor.Systems;

[Filter(typeof(SoundShapeComponent))]
public class SoundShapeEditorSystem : IUpdateSystem, IMurderRenderSystem, IGuiSystem
{
    // Tuple to store the ID and index of the point being dragged
    private (int id, int index)? _dragging;

    private bool _isEditMode = false;
    private bool _isPlaying = false;

    public SoundShapeEditorSystem()
    {
    }

    public void Update(Context context)
    {
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
        if (!IsEnabled(context, hook))
        {
            return;
        }

        if (hook.CursorWorldPosition is not Point cursorPosition)
        {
            return;
        }

        if (hook.EditorMode != EditorHook.EditorModes.EditMode)
        {
            if (_isEditMode)
            {
                OnToggledMode(false);
            }

            return;
        }

        if (!_isEditMode)
        {
            OnToggledMode(true);
        }

        if (Game.Input.PressedAndConsume(MurderInputButtons.Submit))
        {
            TogglePlayEvents(context, hook);
        }

        // Pretend the mouse is the player...
        SoundServices.UpdateListenerPosition(cursorPosition);

        foreach (Entity e in context.Entities)
        {
            SoundShapeTrackerSystem.UpdateEmitterPosition(e, cursorPosition);
        }
    }

    private void TogglePlayEvents(Context context, EditorHook hook)
    {
        if (_isPlaying)
        {
            SoundServices.StopAll(fadeOut: false);

            _isPlaying = false;
        }
        else
        {
            foreach (Entity e in GetEligibleEntities(context, hook))
            {
                if (e.TryGetAmbience() is AmbienceComponent ambience)
                {
                    foreach (SoundEventIdInfo info in ambience.Events)
                    {
                        SoundServices.Play(info.Id, e, info.Layer, properties: SoundProperties.Persist);
                    }
                }
            }

            _isPlaying = true;
        }
    }

    public void Draw(RenderContext render, Context context)
    {
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
        if (!IsEnabled(context, hook))
        {
            return;
        }

        if (hook.CursorWorldPosition is not Point cursorPosition)
        {
            return;
        }

        foreach (Entity e in context.Entities)
        {
            SoundShapeComponent soundShape = e.GetSoundShape();
            Point position = e.GetGlobalTransform().Point;

            SoundPosition soundPosition = soundShape.GetSoundPosition(cursorPosition - position);
            Vector2 closestPoint = soundPosition.ClosestPoint + position;

            // Loop through the points of the sound shape
            for (int i = 0; i < soundShape.Points.Length; i++)
            {
                Vector2 point = soundShape.Points[i];
                // Calculate the distance from the cursor to the current point
                float distance = (cursorPosition - (position + point)).LengthSquared();

                // Draw a rectangle at each point
                RenderServices.DrawRectangle(render.DebugBatch, IntRectangle.CenterRectangle(position + point, 4, 4), Color.White, 0);

                // Draw lines between points if the shape style is OpenLines
                if (soundShape.ShapeStyle != ShapeStyle.Points)
                {
                    if (i > 0)
                    {
                        RenderServices.DrawLine(render.DebugBatch, position + soundShape.Points[i - 1], position + soundShape.Points[i], Color.White, 0.5f);
                    }

                    if ((soundShape.ShapeStyle == ShapeStyle.ClosedLines || soundShape.ShapeStyle == ShapeStyle.ClosedShape) && i == soundShape.Points.Length - 1)
                    {
                        RenderServices.DrawLine(render.DebugBatch, position + soundShape.Points[i], position + soundShape.Points[0], Color.White, 0.5f);
                    }
                }
            }

            ProcessEntity(hook, e);

            // Check if we are in edit mode and if the entity is selected
            if (hook.EditorMode == EditorHook.EditorModes.EditMode && IsSelected(hook, e))
            {
                Vector2 snapPoint;
                if (soundShape.Points.Length == 0 || soundPosition.ClosestIndex < 0)
                {
                    snapPoint = position;
                }
                else
                {
                    snapPoint = soundShape.Points[soundPosition.ClosestIndex] + position;
                }
                float distanceToSnappedPoint = (cursorPosition - snapPoint).Length();

                // If a closest point is found
                if (distanceToSnappedPoint > 8)
                {
                    // If left shift is held down, we can add points
                    if (Game.Input.Down(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    {
                        RenderServices.DrawRectangleOutline(render.DebugBatch, IntRectangle.CenterRectangle(cursorPosition, 8, 8), Color.Green);

                        RenderServices.DrawLine(render.DebugBatch, cursorPosition, position + soundShape.Points.LastOrDefault(), Color.Gray, 0.5f);

                        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                        {
                            e.SetSoundShape(soundShape with { Points = soundShape.Points.Add(cursorPosition - position) });
                            e.SendMessage(new AssetUpdatedMessage(typeof(SoundShapeComponent)));
                        }
                    }
                    else if (!hook.UsingGui)
                    {
                        // This is the main preview line
                        RenderServices.DrawLine(render.DebugBatch, cursorPosition, closestPoint, Color.Gray, 0.5f);

                        // Draw a circle, filled with the current volume.
                        RenderServices.DrawCircleOutline(render.DebugBatch, cursorPosition + new Point(16), 8, 24, Color.White, 0.5f);

                        float ratio = soundPosition.EasedDistance;

                        RenderServices.DrawPieChart(render.DebugBatch, cursorPosition + new Point(16), 8, 0, MathF.PI * 2 * ratio, 24, new DrawInfo(Color.White * ratio, 0.5f));
                    }
                }
                else
                {
                    // If left shift is held down and we are close enough, we can delete points
                    if (Game.Input.Down(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    {
                        RenderServices.DrawRectangleOutline(render.DebugBatch, IntRectangle.CenterRectangle(closestPoint, 10, 10), Color.Red);

                        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                        {
                            e.SetSoundShape(soundShape with { Points = soundShape.Points.RemoveAt(Math.Min(soundShape.Points.Length - 1, soundPosition.ClosestIndex)) });
                            e.SendMessage(new AssetUpdatedMessage(typeof(SoundShapeComponent)));
                        }
                    }
                    else
                    {
                        // Otherwise, we can drag points
                        RenderServices.DrawRectangleOutline(render.DebugBatch, IntRectangle.CenterRectangle(snapPoint, 8, 8), Color.Yellow);

                        // Start dragging if the left click is pressed
                        if (_dragging is null && Game.Input.Pressed(MurderInputButtons.LeftClick))
                        {
                            _dragging = (id: e.EntityId, index: soundPosition.ClosestIndex);
                        }
                    }
                }
            }

            // If a point is being dragged
            if (_dragging is not null && _dragging.Value.id == e.EntityId)
            {
                // Stop dragging if the left click is released
                if (!Game.Input.Down(MurderInputButtons.LeftClick))
                {
                    _dragging = null;
                }
                else
                {
                    // Update the position of the dragged point
                    e.SetSoundShape(soundShape with { Points = soundShape.Points.SetItem(_dragging.Value.index, cursorPosition - position) });
                    e.SendMessage(new AssetUpdatedMessage(typeof(SoundShapeComponent)));
                }
            }
        }
    }

    public void DrawGui(RenderContext render, Context context)
    {
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
        if (!IsEnabled(context, hook))
        {
            return;
        }

        // Check if we are in edit mode and if the entity is selected
        if (IsAnySelected)
        {
            Vector2 topLeft = new Vector2(ImGui.GetItemRectMax().X - 300, ImGui.GetItemRectMin().Y);

            ImGui.SetNextWindowPos(topLeft);
            ImGui.SetNextWindowBgAlpha(0.85f);
            if (ImGui.Begin("Sound editor help",
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoNav |
                ImGuiWindowFlags.NoDecoration))
            {
                ImGui.Text("Shift + Left Click: Add or remove points");
                ImGui.Text("Left Click: Drag points");
                ImGui.Text(_isPlaying ? "Enter: Stop playing events" : "Enter: Start playing events");

                ImGui.End();
            }
        }
    }

    protected virtual void ProcessEntity(EditorHook hook, Entity e) { }

    protected virtual IEnumerable<Entity> GetEligibleEntities(Context context, EditorHook hook) => context.Entities;

    protected virtual void OnToggledMode(bool mode) 
    {
        _isEditMode = mode;

        if (!_isEditMode)
        {
            SoundServices.StopAll(fadeOut: false);
            _isPlaying = false;
        }
    }

    protected virtual bool IsSelected(EditorHook hook, Entity e) => true;

    protected virtual bool IsAnySelected => true;

    protected virtual bool IsEnabled(Context context, EditorHook hook) => 
        context.HasAnyEntity && hook.StageSettings.HasFlag(Assets.StageSetting.ShowSound);
}
