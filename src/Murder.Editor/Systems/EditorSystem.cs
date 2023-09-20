﻿using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.EditorCore;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Services;
using System.Numerics;

namespace Murder.Editor.Systems
{
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    public class EditorSystem : IUpdateSystem, IMurderRenderSystem, IGuiSystem, IStartupSystem
    {
        private const int DefaultSampleSize = 60;
        private readonly SmoothFpsCounter _frameRate = new(DefaultSampleSize);

        private bool _showRenderInspector = false;
        private int _inspectingRenderTarget = 0;
        private IntPtr _renderInspectorPtr;
        private RenderContext.BatchPreviewState _renderPreview = RenderContext.BatchPreviewState.None;

        public void Start(Context context)
        {
            EditorHook hook;

            if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editorComponent)
            {
                hook = editorComponent.EditorHook;
            }
            else
            {
                var component = new EditorComponent();
                hook = component.EditorHook;
                context.World.AddEntity(component);
            }

            hook.Offset =  Point.Zero;
            _renderInspectorPtr = Architect.Instance.ImGuiRenderer.GetNextIntPtr();
        }

        public void DrawGui(RenderContext render, Context context)
        {
            if (!render.RenderToScreen)
                return;

            var hook = context.World.GetUnique<EditorComponent>().EditorHook;
            
            // FPS Window
            ImGui.SetNextWindowBgAlpha(0.9f);
            if (ImGui.Begin("Insights"))
            {
                if (ImGui.BeginTabBar("Insights Tabs"))
                {
                    if (ImGui.BeginTabItem("Performance"))
                    {
                        ImGui.SetWindowPos(new(0, 0), ImGuiCond.Appearing);

                        ImGui.Text($"FPS: {_frameRate.Value}");
                        ImGui.Text($"Update: {Game.Instance.UpdateTime:00.00} ({Game.Instance.LongestUpdateTime:00.00})");
                        ImGui.Text($"Render: {Game.Instance.RenderTime:00.00} ({Game.Instance.LongestRenderTime:00.00})");

                        ImGui.Separator();

                        ImGui.Text($"{Game.GraphicsDevice.Adapter.Description:0}:");
                        ImGui.Text($"Loaded Textures: {Game.GraphicsDevice.Metrics.TextureCount:0}");
                        ImGui.Text($"Clear Count: {Game.GraphicsDevice.Metrics.ClearCount:0}");
                        ImGui.Text($"Display: [{Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width}px, {Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height}px]");
                        ImGui.Text($"Draw Calls: {Game.GraphicsDevice.Metrics.DrawCount}");
                        ImGui.Text($"Primitives: {Game.GraphicsDevice.Metrics.PrimitiveCount}");

                        ImGui.Separator();

                        ImGui.Text($"Entities: {context.World.EntityCount}");
                        ImGui.Text($"Now: {Game.Now:0.0}");
                        ImGui.Text($"Now(Unscaled): {Game.NowUnscaled:0.0}");
                        
                        ImGui.Separator();
                        ImGui.Text($"Custom Shaders:");
                        for (int i = 0; i < Game.Data.CustomGameShader.Length; i++)
                        {
                            var shader = Game.Data.CustomGameShader[i];
                            if (shader != null)
                            {
                                ImGui.Text($"{i}:{shader.Name}");
                            }
                        }
                        
                        ImGui.SetNextWindowBgAlpha(0.9f);
                        ImGui.SetNextWindowSizeConstraints(
                            new Vector2(300, 100),
                            new Vector2(600, 768)
                        );
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("View"))
                    {

                        ImGui.Checkbox("Collisions", ref hook.DrawCollisions);
                        ImGui.Checkbox("Grid", ref hook.DrawGrid);
                        ImGui.Checkbox("Pathfind", ref hook.DrawPathfind);
                        ImGui.Checkbox("States", ref hook.ShowStates);
                        ImGui.Checkbox("Interactions", ref hook.DrawTargetInteractions);
                        ImGui.Checkbox("AnimationEvents", ref hook.DrawAnimationEvents);

                        ImGuiHelpers.DrawEnumField("Draw QuadTree", ref hook.DrawQuadTree);
                        if (ImGui.Button("Recover Camera"))
                        {
                            hook.CurrentZoomLevel = EditorHook.STARTING_ZOOM;
                            foreach (var e in context.World.GetEntitiesWith(typeof(CameraFollowComponent)))
                            {
                                e.SetCameraFollow(true);
                            }
                        }

                        ImGui.NewLine();
                        if (ImGuiHelpers.DrawEnumField("RenderPreview", ref _renderPreview))
                        {
                            render.PreviewState = _renderPreview;
                        }
                        ImGui.Checkbox("Stretch", ref render.PreviewStretch);

                        if (ImGui.Button("Show Render Inspector"))
                        {
                            _showRenderInspector = true;
                        }

                        if (_showRenderInspector && ImGui.Begin("Render Inspector", ref _showRenderInspector, ImGuiWindowFlags.None))
                        {
                            (bool mod, _inspectingRenderTarget) = ImGuiHelpers.DrawEnumField("Render Target", typeof(RenderContext.RenderTargets), _inspectingRenderTarget);
                            var image = render.GetRenderTargetFromEnum((RenderContext.RenderTargets)_inspectingRenderTarget);
                            Architect.Instance.ImGuiRenderer.BindTexture(_renderInspectorPtr, image, false);
                            ImGui.Text($"{image.Width}x{image.Height}");
                            var size = ImGui.GetContentRegionAvail();
                            var aspect = (float)image.Height / image.Width;
                            ImGui.Image(_renderInspectorPtr, new Vector2(size.X, size.X * aspect));

                            if (ImGui.SmallButton("Save As Png"))
                            {
                                var folder = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "\\Screenshots\\");
                                var directory = Directory.CreateDirectory(folder);
                                Stream stream = File.Create(Path.Join(folder, "BufferOutput.png"));
                                image.SaveAsPng(stream, image.Width, image.Height);
                                stream.Dispose();

                                System.Diagnostics.Process.Start("explorer.exe", directory.FullName);
                            }
                            ImGui.End();
                        }

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }
            ImGui.End();
            }

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            if (ImGui.Begin("##Inspector"))
            {
                ImGui.DockSpace(42);
                if (ImGui.IsWindowAppearing())
                {
                    var region = ImGui.GetMainViewport().Size;
                    var size = new Vector2(320, 400);
                    ImGui.SetWindowSize(size);
                    ImGui.SetWindowPos(region - size - new Vector2(20, 14));
                }
                ImGui.End();
            }
            ImGui.PopStyleVar();
        }

        public void Update(Context context)
        {
            _frameRate.Update(Game.DeltaTime);

            MonoWorld world = (MonoWorld)context.World;
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            Point cursorPosition = world.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));

            hook.Cursor = CursorStyle.Normal;
            hook.CursorWorldPosition = cursorPosition;
            hook.CursorScreenPosition = Game.Input.CursorPosition - hook.Offset;

            if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                hook.RefreshAtlas?.Invoke();
            }
        }

        public void Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            DrawEntityDimensions(render, hook);
        }

        private static void DrawEntityDimensions(RenderContext render, EditorHook hook)
        {
            var bounds = render.Camera.Bounds;
            var dimensionColor = Color.BrightGray;

            if (hook.Dimensions is null)
            {
                return;
            }

            foreach (var (_, rectangle) in hook.Dimensions)
            {
                render.FloorBatch.DrawRectangle(new Rectangle(bounds.X, -Grid.HalfCell + rectangle.Top * Grid.CellSize, bounds.Width, 1), dimensionColor, 0.1f);
                render.FloorBatch.DrawRectangle(new Rectangle(bounds.X, -Grid.HalfCell + rectangle.Bottom * Grid.CellSize - 1, bounds.Width, 1), dimensionColor, 0.1f);

                render.FloorBatch.DrawRectangle(new Rectangle(-Grid.HalfCell + rectangle.Left * Grid.CellSize, bounds.Y, 1, bounds.Height), dimensionColor, 0.1f);
                render.FloorBatch.DrawRectangle(new Rectangle(-Grid.HalfCell + rectangle.Right * Grid.CellSize - 1, bounds.Y, 1, bounds.Height), dimensionColor, 0.1f);

                render.FloorBatch.DrawRectangle(rectangle * Grid.CellSize - Grid.HalfCellDimensions, Color.BrightGray, 0.2f);
                render.FloorBatch.DrawRectangle(new Rectangle(-Grid.HalfCell, -Grid.HalfCell, Grid.CellSize, Grid.CellSize), Color.Green, 0f);


                render.DebugBatch.DrawRectangleOutline(rectangle * Grid.CellSize - Grid.HalfCellDimensions, Color.White * 0.2f, 1, 1f);

                render.DebugBatch.DrawRectangle(new (rectangle.TopLeft * Grid.CellSize - Grid.HalfCellDimensions, Point.One), Color.White, 0f);
            }
        }
    }
}
