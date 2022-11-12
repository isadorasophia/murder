using ImGuiNET;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Core;
using Murder.Core.Input;
using Murder.Components;
using Murder.Editor.Attributes;
using Murder.Core.Geometry;
using Murder.Editor.Utilities;
using Murder.Editor.Components;
using Murder.Core.Graphics;
using Murder.Assets;
using Murder.ImGuiExtended;
using Murder.Utilities;
using Murder.Services;
using Murder.Diagnostics;
using System.Diagnostics;
using Bang;
using Bang.Components;

namespace Murder.Editor.Systems
{
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.AllOf, ContextAccessorKind.Read, typeof(ITransformComponent))]
    public class EditorSystem : IUpdateSystem, IMonoRenderSystem, IGuiSystem, IStartupSystem
    {
        private const int DefaultSampleSize = 60;
        private readonly SmoothFpsCounter _frameRate = new(DefaultSampleSize);

        private Point _selectionBox = new Point(10, 10);
        private bool _showRenderInspector = false;
        private int _inspectingRenderTarget = 0;
        private IntPtr _renderInspectorPtr;

        public ValueTask Start(Context context)
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
            _renderInspectorPtr = Game.Instance.ImGuiRenderer.GetNextIntPtr();
            
            return default;
        }

        public ValueTask DrawGui(RenderContext render, Context context)
        {
            var hook = context.World.GetUnique<EditorComponent>().EditorHook;

            Game.Instance.IsMouseVisible = true;
            
            // FPS Window
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.Begin("FPS");

            ImGui.SetWindowPos(new(0, 0), ImGuiCond.Appearing);

            ImGui.Text($"FPS: {_frameRate.Value}");
            ImGui.Text($"Update: {Game.Instance.UpdateTime:00.00} ({Game.Instance.LongestUpdateTime:00.00})");
            ImGui.Text($"Render: {Game.Instance.RenderTime:00.00} ({Game.Instance.LongestRenderTime:00.00})");
            
            ImGui.Separator();

            ImGui.Text($"Draw Calls: {Game.GraphicsDevice.Metrics.DrawCount}");
            ImGui.Text($"Primitives: {Game.GraphicsDevice.Metrics.PrimitiveCount}");
            ImGui.Text($"Loaded Textures: {Game.GraphicsDevice.Metrics.TextureCount}");

            ImGui.End();

            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                new System.Numerics.Vector2(300, 100),
                new System.Numerics.Vector2(600, 768)
            );
            ImGui.Begin("Options");

            ImGui.Checkbox("Collisions", ref hook.DrawCollisions);
            ImGui.Checkbox("Grid", ref hook.DrawGrid);
            ImGui.Checkbox("Pathfind", ref hook.DrawPathfind);
            ImGui.Checkbox("States", ref hook.ShowStates);
            ImGuiHelpers.DrawEnumField("Draw QuadTree", ref hook.DrawQuadTree);
            if (ImGui.Button("Recover Camera"))
            {
                hook.CurrentZoomLevel = EditorHook.STARTING_ZOOM;
                foreach (var e in context.World.GetEntitiesWith(typeof(CameraFollowComponent)))
                {
                    e.SetCameraFollow(true);
                }
            }
            ImGui.SameLine();
            if (ImGui.Button("Show Render Inspector"))
            {
                _showRenderInspector = true;
            }

            if (_showRenderInspector && ImGui.Begin("Render Inspector", ref _showRenderInspector, ImGuiWindowFlags.None))
            {
                Game.Instance.IsMouseVisible = true;

                (bool mod, _inspectingRenderTarget) = ImGuiHelpers.DrawEnumField("Render Target", typeof(RenderContext.RenderTargets), _inspectingRenderTarget);
                var image = render.GetRenderTargetFromEnum((RenderContext.RenderTargets)_inspectingRenderTarget);
                Game.Instance.ImGuiRenderer.BindTexture(_renderInspectorPtr, image, false);
                ImGui.Text($"{image.Width}x{image.Height}");
                var size = ImGui.GetContentRegionAvail();
                var aspect = (float)image.Height / image.Width;
                ImGui.Image(_renderInspectorPtr, new System.Numerics.Vector2(size.X, size.X * aspect));

                if (ImGui.SmallButton("Save As Png"))
                {
                    var folder = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "\\Screenshots\\");
                    var directory = Directory.CreateDirectory(folder);
                    Stream stream = File.Create(Path.Join(folder,"BufferOutput.png"));
                    image.SaveAsPng(stream, image.Width, image.Height);
                    stream.Dispose();

                    System.Diagnostics.Process.Start("explorer.exe", directory.FullName);
                }
                ImGui.End();
            }


            ImGui.End();

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGui.Begin("##Inspector");
            ImGui.DockSpace(42);
            if (ImGui.IsWindowAppearing())
            {
                var region = ImGui.GetMainViewport().Size;
                var size = new System.Numerics.Vector2(320, 400) * (Game.Instance.DPIScale / 100);
                ImGui.SetWindowSize(size);
                ImGui.SetWindowPos(region - size - new System.Numerics.Vector2(20, 14));
            }
            ImGui.End();
            ImGui.PopStyleVar();

            return default;
        }

        public ValueTask Update(Context context)
        {
            _frameRate.Update(Game.DeltaTime);

            MonoWorld world = (MonoWorld)context.World;
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            Point cursorPosition = world.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));

            hook.Cursor = EditorHook.CursorStyle.Normal;
            hook.CursorWorldPosition = cursorPosition;
            hook.CursorScreenPosition = Game.Input.CursorPosition - hook.Offset;

            if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                hook.RefreshAtlas?.Invoke();
            }

            return default;
        }

        public ValueTask Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            DrawEntityDimensions(render, hook);

            return default;
        }

        private void DrawEntityDimensions(RenderContext render, EditorHook hook)
        {
            var bounds = render.Camera.Bounds;
            var dimensionColor = Color.BrightGray;

            if (hook.Dimensions is null)
            {
                return;
            }

            foreach (var (_, rectangle) in hook.Dimensions)
            {
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(bounds.X, -Grid.HalfCell + rectangle.Top * Grid.CellSize, bounds.Width, 1), dimensionColor, 0.1f);
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(bounds.X, -Grid.HalfCell + rectangle.Bottom * Grid.CellSize - 1, bounds.Width, 1), dimensionColor, 0.1f);

                render.FloorSpriteBatch.DrawRectangle(new Rectangle(-Grid.HalfCell + rectangle.Left * Grid.CellSize, bounds.Y, 1, bounds.Height), dimensionColor, 0.1f);
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(-Grid.HalfCell + rectangle.Right * Grid.CellSize - 1, bounds.Y, 1, bounds.Height), dimensionColor, 0.1f);

                render.FloorSpriteBatch.DrawRectangle(rectangle * Grid.CellSize - Grid.HalfCellDimensions, Color.BrightGray, 0.2f);
                render.FloorSpriteBatch.DrawRectangle(new Rectangle(-Grid.HalfCell, -Grid.HalfCell, Grid.CellSize, Grid.CellSize), Color.Green, 0f);


                render.DebugSpriteBatch.DrawRectangleOutline(rectangle * Grid.CellSize - Grid.HalfCellDimensions, Color.White.WithAlpha(0.2f), 1, 1f);

                render.DebugSpriteBatch.DrawRectangle(new (rectangle.TopLeft * Grid.CellSize - Grid.HalfCellDimensions, Point.One), Color.White, 0f);
            }
        }
    }
}
