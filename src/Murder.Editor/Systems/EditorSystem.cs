using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Attributes;
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
    private int _windowWidth;
    private int _windowHeight;
    private string _windowSize = string.Empty;

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

        // Record slowdowns
        float totalFrameTime = Game.FixedDeltaTime;
        if (totalFrameTime > 0)
        {
            float renderRatio = Game.Instance.RenderTime / totalFrameTime;
            float updateRatio = Game.Instance.UpdateTime / totalFrameTime;
            float imguiRatio = Game.Instance.ImGuiRenderTime / totalFrameTime;
            if (renderRatio + updateRatio + imguiRatio > 1)
            {
                SlowDownTracker.Update(renderRatio + updateRatio + imguiRatio - 1f);
            }
            else
            {
                SlowDownTracker.Update(0f);
            }
        }

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
                    PlotSlowdowGraph();

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
                        ResizeWindow(4, render);
                    }

                    if (ImGui.Button("1080p"))
                    {
                        ResizeWindow(render, new Point(1920, 1080));
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("1440p"))
                    {
                        ResizeWindow(render, new Point(2560, 1440));
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("vertical"))
                    {
                        ResizeWindow(render, new Point(1080, 1920));
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
                    var dl = ImGui.GetWindowDrawList();

                    // To calculate FPS from LastFrameDuration (in seconds), use the formula: FPS = 1 / LastFrameDuration.
                    ImGui.Text($"FPS: {_frameRate.Value}");

                    ImGui.Text($"   Render: {Game.Instance.RenderTime * 1000:00.00} ({Game.Instance.LongestRenderTime * 1000:00.00})");
                    dl.AddCircleFilled(ImGui.GetItemRectMin() + new Vector2(10, 6), 5, Color.ToUint(Game.Profile.Theme.White), 12);

                    ImGui.Text($"   Update: {Game.Instance.UpdateTime * 1000:00.00} ({Game.Instance.LongestUpdateTime * 1000:00.00})");
                    dl.AddCircleFilled(ImGui.GetItemRectMin() + new Vector2(10, 6), 5, Color.ToUint(Game.Profile.Theme.HighAccent), 12);

                    ImGui.Text($"   ImGui: {Game.Instance.ImGuiRenderTime * 1000:00.00}");
                    dl.AddCircleFilled(ImGui.GetItemRectMin() + new Vector2(10, 6), 5, Color.ToUint(Game.Profile.Theme.Yellow), 12);

                    ImGui.Text($"   Sound: {Game.Instance.SoundUpdateTime * 1000:00.00}");
                    dl.AddCircleFilled(ImGui.GetItemRectMin() + new Vector2(10, 6), 5, Color.ToUint(Game.Profile.Theme.Green), 12);

                    ImGui.Text($"Total: {Game.Instance.LastFrameDuration * 1000:00.00}");

                    ImGui.Dummy(new Vector2(0, 24));
                    Vector2 start = ImGui.GetItemRectMin();
                    Vector2 end = ImGui.GetItemRectMax();
                    Vector2 availableSize = ImGui.GetContentRegionAvail();
                    // Draw a border around the frame insight area
                    dl.AddRect(start, new Vector2(start.X + availableSize.X, end.Y), Color.ToUint(Game.Profile.Theme.Faded), 0f, ImDrawFlags.None, 1f);

                    // Draw Update, Render in proportion to the total frame time
                    if (totalFrameTime > 0)
                    {
                        float renderRatio = Game.Instance.RenderTime / totalFrameTime;
                        float updateRatio = Game.Instance.UpdateTime / totalFrameTime;
                        float imguiRatio = Game.Instance.ImGuiRenderTime / totalFrameTime;
                        float soundRatio = Game.Instance.SoundUpdateTime / totalFrameTime;

                        float excess = renderRatio + updateRatio + imguiRatio + soundRatio - 1f;

                        if (excess > 0)
                        {
                            // If the ratios exceed 1, we need to adjust them to fit within the available space
                            float totalRatio = renderRatio + updateRatio + imguiRatio;
                            renderRatio /= totalRatio;
                            updateRatio /= totalRatio;
                            imguiRatio /= totalRatio;
                            soundRatio /= totalRatio;
                        }

                        float renderEnd = start.X + availableSize.X * renderRatio;
                        float updateEnd = start.X + availableSize.X * (renderRatio + updateRatio);
                        float imguiEnd = start.X + availableSize.X * (renderRatio + updateRatio + imguiRatio);
                        float soundEnd = start.X + availableSize.X * (renderRatio + updateRatio + imguiRatio + soundRatio);

                        if (renderEnd - start.X >= 1)
                        {
                            dl.AddRectFilled(new Vector2(start.X, start.Y), new Vector2(renderEnd, end.Y), Color.ToUint(Game.Profile.Theme.White), 4f);
                        }

                        if (updateEnd - renderEnd >= 1)
                        {
                            dl.AddRectFilled(new Vector2(renderEnd, start.Y), new Vector2(updateEnd, end.Y), Color.ToUint(Game.Profile.Theme.HighAccent), 4f);
                        }

                        if (imguiEnd - updateEnd >= 1)
                        {
                            dl.AddRectFilled(new Vector2(updateEnd, start.Y), new Vector2(imguiEnd, end.Y), Color.ToUint(Game.Profile.Theme.Yellow), 4f);
                        }
                        if (soundEnd - imguiEnd >= 1)
                        {
                            dl.AddRectFilled(new Vector2(imguiEnd, start.Y), new Vector2(soundEnd, end.Y), Color.ToUint(Game.Profile.Theme.Green), 4f);
                        }

                        if (excess > 0)
                        {
                            // If the render and update ratios exceed 1, draw the overflow in red as a small bar under the frame insight area
                            float overflowRatio = Calculator.Clamp01(excess);
                            dl.AddRectFilled(new Vector2(start.X, end.Y - 10), new Vector2(start.X + availableSize.X * overflowRatio, end.Y), Color.ToUint(Game.Profile.Theme.Red), 0);
                        }
                    }

                    ImGui.Separator();
                    PlotDeltaTimeGraph();

                    ImGui.SeparatorText("Display Details");

                    ImGui.Text($"{Game.GraphicsDevice.Adapter.Description:0}:");
                    ImGui.Text($"Display: [{Game.GraphicsDevice.Adapter.CurrentDisplayMode.Width}px, {Game.GraphicsDevice.Adapter.CurrentDisplayMode.Height}px]");

                    ImGui.Text($"Fullscreen? {Game.Instance.Fullscreen}");
                    ImGui.Text($"Original Scale: {render.Viewport.OriginalScale}");
                    ImGui.Text($"Snapped Scale: {render.Viewport.Scale}");
                    ImGui.Text($"Viewport: {render.Viewport.Size}");
                    ImGui.Text($"NativeResolution: {render.Viewport.NativeResolution}");

                    if (ImGui.IsWindowAppearing())
                    {
                        _windowWidth = Game.Instance.Window.ClientBounds.Width;
                        _windowHeight = Game.Instance.Window.ClientBounds.Height;
                        _windowSize = $"{_windowWidth}px, {_windowHeight}px";
                    }

                    ImGui.InputText("WindowSize##Window", ref _windowSize, 32);
                    ImGui.SameLine();
                    if (ImGui.Button("Resize Window")) // "Resize Window
                    {
                        Game.Instance.Fullscreen = false;
                        Point windowSize = Calculator.ParsePixelSize(_windowSize);
                        ResizeWindow(render, windowSize);
                    }

                    if (ImGui.Button("Refresh Resolution"))
                    {
                        _windowWidth = Game.Instance.Window.ClientBounds.Width;
                        _windowHeight = Game.Instance.Window.ClientBounds.Height;
                        Point windowSize = new Point(_windowWidth, _windowHeight);
                        render.RefreshWindow(Game.GraphicsDevice, windowSize, new Point(render.Viewport.NativeResolution.X, render.Viewport.NativeResolution.Y), Game.Profile.ResizeStyle);
                    }

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Snapshots"))
                {
                    if (ImGui.Button("Snapshot (Single)"))
                    {
                        DebugSnapshot.TakeSnapShot(1);
                    }

                    if (ImGui.Button("Snapshot (30)"))
                    {
                        DebugSnapshot.TakeSnapShot(30);
                    }

                    if (ImGui.Button("Snapshot (120)"))
                    {
                        DebugSnapshot.TakeSnapShot(120);
                    }

                    ImGui.TextColored(Game.Profile.Theme.White, $"{DebugSnapshot.GetTotalTime()}");

                    ImGuiHelpers.DrawBarsGraph(DebugSnapshot.GetAllEntries(), 20);
                }

                ImGui.EndTabBar();
            }
        }

        if (ImGui.IsWindowHovered())
        {
            _hovered = Calculator.Approach(_hovered, 1, Game.Instance.LastFrameDuration * 5);
        }
        else
        {
            _hovered = Calculator.Approach(_hovered, 0, Game.Instance.LastFrameDuration * 5);
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

    public static void ResizeWindow(RenderContext render, Point windowSize)
    {
        Game.Instance.Fullscreen = false;
        Game.Instance.SetWindowSize(windowSize, false);
        Game.Instance.GraphicsDeviceManager.ApplyChanges();
        render.RefreshWindow(Game.GraphicsDevice, windowSize, new Point(Game.Profile.GameWidth, Game.Profile.GameHeight), Game.Profile.ResizeStyle);
    }

    private static void ResizeWindow(float scale, RenderContext render)
    {
        Point windowSize = (new Vector2(render.Viewport.NativeResolution.X, render.Viewport.NativeResolution.Y) * scale).Point();
        Game.Instance.Fullscreen = false;
        Game.Instance.SetWindowSize(windowSize, false);
        Game.Instance.GraphicsDeviceManager.ApplyChanges();
        render.RefreshWindow(Game.GraphicsDevice, windowSize, new Point(render.Viewport.NativeResolution.X, render.Viewport.NativeResolution.Y), Game.Profile.ResizeStyle);
    }

    public void Update(Context context)
    {
        _frameRate.Update(Game.Instance.LastFrameDuration);

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

    private int _previousGCGen1Count = 0;
    private int _previousGCGen2Count = 0;
    /// <summary>
    /// Only updated byt the EditorSystem
    /// </summary>
    public static UpdateTimeTracker GcTracker = new();
    public static UpdateTimeTracker SlowDownTracker = new();
    private void PlotDeltaTimeGraph()
    {
        // TODO: Get more meaningful information from this...
        int gen1Count = GC.CollectionCount(generation: 1);
        int gen2Count = GC.CollectionCount(generation: 2);
        if (gen2Count != _previousGCGen2Count)
        {
            _previousGCGen2Count = gen2Count;
            GcTracker.Update(1f);
        }
        else if (gen1Count != _previousGCGen1Count)
        {
            _previousGCGen1Count = gen1Count;
            GcTracker.Update(0.5f);
        }
        else
        {
            GcTracker.Update(0f);
        }
        if (Game.IsRunningSlowly)
        {
            ImGui.TextColored(Game.Profile.Theme.Red, "Running slowly!");
        }

        PlotSlowdowGraph();

        ImGui.PlotHistogram("##GC_histogram", ref GcTracker.Sample[0], GcTracker.Length, 0, "GC Collection", 0, 1000, new Vector2(ImGui.GetContentRegionAvail().X, 20));

        // ImGui.Text($"Gen 1/Gen 2 GC Count: {gen1Count}, {gen2Count}");
        UpdateTimeTracker tracker = Game.TimeTrackerDiagnostics;
        UpdateTimeTracker renderTracker = Game.RenderTimeTrackerDiagnostics;
        UpdateTimeTracker gcTracker = Game.RenderTimeTrackerDiagnostics;
        if (tracker.Length >= 0)
        {
            ImGui.PlotHistogram("##fps_histogram", ref tracker.Sample[0], tracker.Length, 0, "Systems Time Tracker (ms)", 0, Game.FixedDeltaTime * 1.25f * 1000, new Vector2(ImGui.GetContentRegionAvail().X, 80));
            ImGui.PlotHistogram("##render_histogram", ref renderTracker.Sample[0], renderTracker.Length, 0, "Render Time Tracker (ms)", 0, Game.FixedDeltaTime * 0.5f * 1000, new Vector2(ImGui.GetContentRegionAvail().X, 80));
            (float low, float median, float high) = GetHighLowAndMedian(tracker);
            ImGui.Text($"Fastest Update Time: {low:0.00} ms");
            ImGui.Text($"Median Update Time: {median:0.00} ms");
            ImGui.Text($"Slowest Update Time: {high:0.00} ms");
        }
        else
        {
            ImGui.Text($"Graph is empty. Is DIAGNOSTICS_MODE enabled?");
        }
    }

    private static void PlotSlowdowGraph()
    {
        ImGui.PushStyleColor(ImGuiCol.PlotHistogram, Game.Profile.Theme.Red);
        ImGui.PlotHistogram("##Slowdown History", ref SlowDownTracker.Sample[0], SlowDownTracker.Length, 0, "Slowdowns", 0, 1000, new Vector2(ImGui.GetContentRegionAvail().X, 20));
        ImGui.PopStyleColor();
    }

    private (float low, float median, float high) GetHighLowAndMedian(UpdateTimeTracker tracker)
    {
        float[] samples = tracker.Sample;
        int length = tracker.Length;

        if (length <= 0 || samples == null || samples.Length < length)
            return (0f, 0f, 0f);

        // Copy only the valid samples
        float[] validSamples = new float[length];
        Array.Copy(samples, validSamples, length);

        // Sort to compute median and get min/max
        Array.Sort(validSamples);
        float low = validSamples[0];
        float high = validSamples[length - 1];
        float median;

        if (length % 2 == 0)
        {
            // Even number of elements
            median = (validSamples[length / 2 - 1] + validSamples[length / 2]) / 2f;
        }
        else
        {
            // Odd number of elements
            median = validSamples[length / 2];
        }

        return (low, median, high);
    }


}