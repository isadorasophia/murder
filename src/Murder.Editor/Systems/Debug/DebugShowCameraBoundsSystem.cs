using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Editor.Components;
using Murder.Editor.Core;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Serialization;
using Murder.Services;
using System.Numerics;

namespace Murder.Editor.Systems.Debug
{
    [EditorSystem]
    public class DebugShowCameraBoundsSystem : IMurderRenderSystem, IUpdateSystem, IGuiSystem
    {
        private readonly static int _hash = typeof(DebugShowCameraBoundsSystem).GetHashCode();

        private bool _takeScreenshot = false;
        private Point _resolution = Point.Zero;

        private Point? _draggingStart = null;
        private Point? _draggingStartSize = null;
        private Point _cameraSize = Point.Zero;
        public void Draw(RenderContext render, Context context)
        {
            EditorHook? editorHook = context.World.TryGetUnique<EditorComponent>()?.EditorHook;
            if (editorHook is null || editorHook.DrawCameraBounds is not EditorHook.CameraBoundsInfo info)
            {
                return;
            }
            if (_cameraSize == Point.Zero)
            {
                _cameraSize = new Point(Game.DefaultWidth, Game.DefaultHeight);
            }

            Rectangle cameraRect = new(
                render.Camera.Position.X + render.Camera.HalfWidth - Game.DefaultWidth / 2f + info.Offset.X,
                render.Camera.Position.Y + render.Camera.Height / 2f - Game.DefaultHeight / 2f + info.Offset.Y,
                _cameraSize.X,
                _cameraSize.Y
                );

            render.GameUiBatch.DrawRectangleOutline(cameraRect + new Point(0, 1), Game.Profile.Theme.Bg, 3, 0.5f);
            render.GameUiBatch.DrawRectangleOutline(cameraRect, Game.Profile.Theme.HighAccent, 2, 0.45f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                render.Camera.Bounds.X, render.Camera.Bounds.Y,
                render.Camera.Bounds.Width + 2,
                cameraRect.Y - render.Camera.Bounds.Y), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                render.Camera.Bounds.X, cameraRect.Y,
                cameraRect.X - render.Camera.Bounds.X,
                cameraRect.Height), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                cameraRect.X + cameraRect.Width, cameraRect.Y,
                render.Camera.Bounds.Width,
                cameraRect.Height), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            render.GameUiBatch.DrawRectangle(new Rectangle(
                render.Camera.Bounds.X, cameraRect.Y + cameraRect.Height,
                render.Camera.Bounds.Width + 2,
                render.Camera.Bounds.Height - cameraRect.Height - cameraRect.Y + render.Camera.Bounds.Y + 2), Game.Profile.Theme.Bg * 0.4f, 0.995f);

            Point handleSize = new Point(98, 12);
            info.HandleArea = new(cameraRect.TopLeft - new Point(0, handleSize.Y), handleSize);
            render.GameUiBatch.DrawRectangle(info.HandleArea.Value, Game.Profile.Theme.HighAccent, 0.45f);
            RenderServices.DrawText(render.GameUiBatch, MurderFonts.PixelFont, "CAMERA REAL SIZE", cameraRect.TopLeft - new Point(-4, handleSize.Y - 4),
                new DrawInfo(0.44f) { Color = Game.Profile.Theme.Bg });

            info.ScreenshotButtonArea = new Rectangle(cameraRect.BottomRight + new Vector2(-handleSize.Y / 2f), new Vector2(handleSize.Y));
            render.GameUiBatch.DrawRectangle(info.ScreenshotButtonArea.Value, Game.Profile.Theme.HighAccent, 0.45f);

            if (_takeScreenshot)
            {
                _takeScreenshot = false;
                render.SaveScreenShotArea(cameraRect);
            }
        }

        public void DrawGui(RenderContext render, Context context)
        {

            EditorHook? editorHook = context.World.TryGetUnique<EditorComponent>()?.EditorHook;
            if (editorHook is null || editorHook.DrawCameraBounds is not EditorHook.CameraBoundsInfo info)
            {
                return;
            }

            if (ImGui.Begin("Camera Bounds", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking))
            {
                if (info.ScreenshotButtonArea.HasValue)
                {
                    if (ImGuiHelpers.Button("Take Screenshot"))
                    {
                        editorHook.CursorIsBusy.Add(typeof(DebugShowCameraBoundsSystem));
                        _takeScreenshot = true;
                    }

                    if (ImGuiHelpers.Button("Reset Size"))
                    {
                        _cameraSize = Point.Zero;
                    }

                    if (ImGuiHelpers.Button("Open In Explorer"))
                    {
                        string filePath = FileHelper.GetScreenshotFolder();
                        if (Directory.Exists(filePath))
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                            {
                                FileName = filePath,
                                UseShellExecute = true,
                                Verb = "open"
                            });
                        }
                    }
                }
                ImGui.End();
            }
        }

        public void Update(Context context)
        {
            EditorHook? editorHook = context.World.TryGetUnique<EditorComponent>()?.EditorHook;
            if (editorHook is null || editorHook.DrawCameraBounds is not EditorHook.CameraBoundsInfo info)
            {
                return;
            }

            if (info.HandleArea.HasValue)
            {
                if (info.ResetCameraBounds)
                {
                    info.ResetCameraBounds = false;
                    info.CenterOffset = Point.Zero;
                    info.Offset = Point.Zero;
                }

                if (editorHook.CursorWorldPosition is Point cursorWorldPosition)
                {
                    if (info.HandleArea.Value.Contains(cursorWorldPosition))
                    {
                        editorHook.Cursor = CursorStyle.Hand;
                        if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                        {
                            info.CenterOffset = cursorWorldPosition - info.Offset;
                            info.Dragging = true;
                            editorHook.CursorIsBusy.Add(typeof(DebugShowCameraBoundsSystem));
                        }
                    }

                    if (!Game.Input.Down(MurderInputButtons.LeftClick))
                    {
                        editorHook.CursorIsBusy.Remove(typeof(DebugShowCameraBoundsSystem));
                        info.Dragging = false;
                    }

                    if (info.Dragging)
                    {
                        editorHook.CursorIsBusy.Add(typeof(DebugShowCameraBoundsSystem));
                        info.Offset = cursorWorldPosition - info.CenterOffset;
                    }
                }
            }

            if (info.ScreenshotButtonArea.HasValue)
            {
                if (editorHook.CursorWorldPosition != null && info.ScreenshotButtonArea.Value.Contains(editorHook.CursorWorldPosition.Value))
                {
                    editorHook.Cursor = CursorStyle.Point;
                    if (Game.Input.Pressed(MurderInputButtons.LeftClick))
                    {
                        editorHook.CursorIsBusy.Add(typeof(DebugShowCameraBoundsSystem));
                        _draggingStart = editorHook.CursorWorldPosition;
                        _draggingStartSize = _cameraSize;
                    }
                }
            }

            if (Game.Input.Down(MurderInputButtons.LeftClick) && _draggingStart != null && _draggingStartSize != null && editorHook.CursorWorldPosition != null)
            {
                _cameraSize = _draggingStartSize.Value + (editorHook.CursorWorldPosition.Value - _draggingStart.Value);
            }
            else
            {
                _draggingStart = null;
            }
        }
    }
}