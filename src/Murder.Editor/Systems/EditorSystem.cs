using Bang.Contexts;
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
using Murder.Editor.Core;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems;

[EditorSystem]
[DoNotPause]
[OnlyShowOnDebugView]
[Filter(ContextAccessorFilter.None)]
public class EditorSystem : IUpdateSystem, IMurderRenderSystem, IGuiSystem, IStartupSystem
{
    public const int WINDOW_MAX_WIDTH = 1200;
    public const int WINDOW_MAX_HEIGHT = 1200;

    private const int DefaultSampleSize = 60;
    private readonly SmoothFpsCounter _frameRate = new(DefaultSampleSize);

    private bool _showRenderInspector = false;
    private int _inspectingRenderTarget = 0;
    private IntPtr _renderInspectorPtr;
    private RenderContext.BatchPreviewState _renderPreview = RenderContext.BatchPreviewState.None;


    private float _hovered;

    public void Start(Context context)
    {
        EditorHook hook;

        if (context.World.TryGetUnique<EditorComponent>() is EditorComponent editorComponent)
        {
            hook = editorComponent.EditorHook;
        }
        else
        {
            var component = new EditorComponent(true);
            hook = component.EditorHook;
            context.World.AddEntity(component);
        }

        hook.Offset = Point.Zero;
        _renderInspectorPtr = Architect.Instance.ImGuiRenderer.GetNextIntPtr();
    }

    public void DrawGui(RenderContext render, Context context)
    {
        if (!render.RenderToScreen)
            return;

        var hook = context.World.GetUnique<EditorComponent>().EditorHook;

        ImGui.BeginMainMenuBar();

        if (ImGui.BeginMenu("Show"))
        {
            ImGui.MenuItem("Render Inspector", "", ref _showRenderInspector);
            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();

        // FPS Window
        ImGui.SetNextWindowBgAlpha(Calculator.Lerp(0.5f, 1f, _hovered));
        if (ImGui.Begin("Insights", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if (ImGui.BeginTabBar("Insights Tabs"))
            {
                ImGui.SetWindowPos(new(0, 0), ImGuiCond.Appearing);

                if (ImGui.BeginTabItem("Overview"))
                {
                    ImGui.SeparatorText("Performance");
                    ImGui.Text($"FPS: {_frameRate.Value}");
                    ImGui.Text($"Update: {Game.Instance.UpdateTime:00.00} ({Game.Instance.LongestUpdateTime:00.00})");
                    ImGui.Text($"Render: {Game.Instance.RenderTime:00.00} ({Game.Instance.LongestRenderTime:00.00})");
                    ImGui.Text($"Entities: {context.World.EntityCount}");


                    ImGui.SeparatorText("Window Scale");
                    if (ImGui.Button("1x"))
                    {
                        ResizeWindow(1, render);
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("2x"))
                    {
                        ResizeWindow(2, render);
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("3x"))
                    {
                        ResizeWindow(3, render);
                    }


                    ImGui.SameLine();
                    if (ImGui.Button("4x"))
                    {
                        ResizeWindow(3, render);
                    }

                    ImGui.SeparatorText("Time");


                    float timeScale = Game.Instance.TimeScale;
                    ImGui.TextColored(Game.Profile.Theme.Faded, "Time Scale");
                    if (ImGui.SliderFloat("##Time Scale", ref timeScale, 0, 2))
                    {
                        Game.Instance.TimeScale = timeScale;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Reset"))
                    {
                        Game.Instance.TimeScale = 1;
                    }
                    ImGui.Spacing();

                    if (context.World.IsPaused)
                    {
                        ImGui.TextColored(Game.Profile.Theme.Yellow, "Paused");
                    }
                    else
                    {
                        ImGui.TextColored(Game.Profile.Theme.Faded, "Not Paused");
                    }
                    ImGui.Text($"Now: {Game.Now:0.0}");
                    ImGui.Text($"Now(Unscaled): {Game.NowUnscaled:0.0}");

                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Settings"))
                {
                    ImGui.SeparatorText("Gizmos");

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
                
                if (ImGui.BeginTabItem("Details"))
                {
                    ImGui.SeparatorText("Performance");

                    ImGui.Text($"FPS: {_frameRate.Value}");
                    ImGui.Text($"Update: {Game.Instance.UpdateTime:00.00} ({Game.Instance.LongestUpdateTime:00.00})");
                    ImGui.Text($"Render: {Game.Instance.RenderTime:00.00} ({Game.Instance.LongestRenderTime:00.00})");
                    ImGui.Text($"Entities: {context.World.EntityCount}");

                    ImGui.Separator();
                    PlotDeltaTimeGraph();

                    ImGui.SeparatorText("Display Details");

                    ImGui.Text($"{Game.GraphicsDevice.Adapter.Description:0}:");
                    ImGui.Text($"Display: [{Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width}px, {Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height}px]");

                    ImGui.Text($"Original Scale: {render.Viewport.OriginalScale}");
                    ImGui.Text($"Snapped Scale: {render.Viewport.Scale}");
                    ImGui.Text($"Viewport: {render.Viewport.Size}");
                    ImGui.Text($"NativeResolution: {render.Viewport.NativeResolution}");

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }

        if (ImGui.IsWindowHovered())
        {
            _hovered = Calculator.Approach(_hovered, 1, Game.DeltaTime * 5);
        }
        else
        {
            _hovered = Calculator.Approach(_hovered, 0, Game.DeltaTime * 5);
        }

        ImGui.End();

        if (hook.AllSelectedEntities.Count > 0)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            if (ImGui.Begin("##Inspector"))
            {
                ImGui.DockSpace(42);
                if (ImGui.IsWindowAppearing())
                {
                    var region = ImGui.GetMainViewport().Size;
                    var size = new Vector2(380, region.Y);
                    ImGui.SetWindowSize(size);
                    ImGui.SetWindowPos(region - size + new Vector2(-20, 14));
                }
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }

    }

    private static void ResizeWindow(float scale, RenderContext render)
    {
        Point windowSize = new(Game.Profile.GameWidth * scale, Game.Profile.GameHeight * scale);
        Game.Instance.SetWindowSize(windowSize);
        Game.Instance.GraphicsDeviceManager.ApplyChanges();
        render.RefreshWindow(Game.GraphicsDevice, windowSize, new Point(Game.Profile.GameWidth, Game.Profile.GameHeight), Game.Profile.ResizeStyle);
    }

    public void Update(Context context)
    {
        _frameRate.Update(Game.DeltaTime);

        MonoWorld world = (MonoWorld)context.World;
        EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

        // Currently this is where the hook.CursorPosition is being set.
        // We do not set a valid world position if hovering an ImGui window.
        hook.CursorScreenPosition = Game.Input.CursorPosition - hook.Offset;

        if (Game.Input.MouseConsumed)
        {
            hook.CursorWorldPosition = null;
            return;
        }

        Point cursorPosition = world.Camera.GetCursorWorldPosition(hook.Offset, new(hook.StageSize.X, hook.StageSize.Y));
        hook.Cursor = CursorStyle.Normal;
        hook.CursorWorldPosition = cursorPosition;

        if (!hook.IsPopupOpen)
        {
            hook.LastCursorWorldPosition = cursorPosition;
        }

        if (hook.CanSwitchModes && !hook.UsingGui && Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.Tab))
        {
            if (hook.EditorMode == EditorHook.EditorModes.EditMode)
            {
                hook.EditorMode = EditorHook.EditorModes.ObjectMode;
            }
            else if (hook.EditorMode == EditorHook.EditorModes.ObjectMode && hook.AllSelectedEntities.Count > 0)
            {
                hook.EditorMode = EditorHook.EditorModes.EditMode;
            }
            else
            {
                // Pressing TAB in play mode doesn't do anything
            }
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
            render.FloorBatch.DrawRectangle(new Rectangle(bounds.X, -Grid.HalfCellSize + rectangle.Top * Grid.CellSize, bounds.Width, 1), dimensionColor, 0.1f);
            render.FloorBatch.DrawRectangle(new Rectangle(bounds.X, -Grid.HalfCellSize + rectangle.Bottom * Grid.CellSize - 1, bounds.Width, 1), dimensionColor, 0.1f);

            render.FloorBatch.DrawRectangle(new Rectangle(-Grid.HalfCellSize + rectangle.Left * Grid.CellSize, bounds.Y, 1, bounds.Height), dimensionColor, 0.1f);
            render.FloorBatch.DrawRectangle(new Rectangle(-Grid.HalfCellSize + rectangle.Right * Grid.CellSize - 1, bounds.Y, 1, bounds.Height), dimensionColor, 0.1f);

            render.FloorBatch.DrawRectangle(rectangle * Grid.CellSize - Grid.HalfCellDimensions, Color.BrightGray, 0.2f);
            render.FloorBatch.DrawRectangle(new Rectangle(-Grid.HalfCellSize, -Grid.HalfCellSize, Grid.CellSize, Grid.CellSize), Color.Green, 0f);


            render.DebugBatch.DrawRectangleOutline(rectangle * Grid.CellSize - Grid.HalfCellDimensions, Color.White * 0.2f, 1, 1f);

            render.DebugBatch.DrawRectangle(new(rectangle.TopLeft * Grid.CellSize - Grid.HalfCellDimensions, Point.One), Color.White, 0f);
        }
    }

    private void PlotDeltaTimeGraph()
    {
        // TODO: Get more meaningful information from this...
        int gen1Count = GC.CollectionCount(generation: 1);
        int gen2Count = GC.CollectionCount(generation: 2);

        // ImGui.Text($"Gen 1/Gen 2 GC Count: {gen1Count}, {gen2Count}");
        UpdateTimeTracker tracker = Game.TimeTrackerDiagnoostics;
        if (tracker.Length >= 0)
        {
            ImGui.PlotHistogram("##fps_histogram", ref tracker.Sample[0], tracker.Length, 0, "Delta Time Tracker (ms)", 0, Game.FixedDeltaTime * 1.5f * 1000, new Vector2(ImGui.GetContentRegionAvail().X, 80));
        }
        else
        {
            ImGui.Text($"Graph is empty. Is DIAGNOSTICS_MODE enabled?");
        }

    }
}