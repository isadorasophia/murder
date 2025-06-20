using Bang.Contexts;
using Bang.Diagnostics;
using Bang.Systems;
using ImGuiNET;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Systems.Debug;
using Murder.Editor.Utilities;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Systems;

/// <summary>
/// This system will draw selected entities and drag them through the map.
/// </summary>
[DoNotPause]
[OnlyShowOnDebugView]
[Filter(ContextAccessorFilter.None)]
public class SystemsDiagnosticsSystem : IGuiSystem
{
    private bool _showDiagnostics = false;

    /// <summary>
    /// This is only used for rendering the entity components during the game (on debug mode).
    /// </summary>
    public void DrawGui(RenderContext render, Context context)
    {
        ImGui.BeginMainMenuBar();

        if (ImGui.BeginMenu("Show"))
        {
            ImGui.MenuItem("Diagnostics", "", ref _showDiagnostics);
            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();

        if (!_showDiagnostics)
        {
            return;
        }

        MonoWorld world = (MonoWorld)context.World;

        int maxWidth = 1200;

        // Graphics
        ImGui.SetNextWindowBgAlpha(0.9f);
        ImGui.SetNextWindowSizeConstraints(
            size_min: new Vector2(500, 350),
            size_max: new Vector2(EditorSystem.WINDOW_MAX_WIDTH, EditorSystem.WINDOW_MAX_HEIGHT));

        int padding = 25;
        ImGui.SetWindowPos(new(x: render.Viewport.Size.X - maxWidth, y: padding), ImGuiCond.Appearing);

        if (!ImGui.Begin("Diagnostics", ref _showDiagnostics))
        {
            ImGui.End();
            // Window is closed, so just go way...
            return;
        }

        ImGui.BeginTabBar("diagnostics_tabs", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.TabListPopupButton);
        if (ImGui.BeginTabItem("Systems"))
        {
            ImGui.BeginChild("diagnostic_systems");
            {
                using TableMultipleColumns table = new("dianogstics_view", ImGuiTableFlags.Resizable, 0, -1);

                float height = ImGui.GetContentRegionAvail().Y - padding / 5f;

                ImGui.TableNextColumn();

                ImGui.BeginChild("systems_diagnostics",
                    size: new Vector2(-1, height), ImGuiChildFlags.None, ImGuiWindowFlags.NoDocking);

                CalculateAllOverallTime(world);

                DrawTabs();

                ImGui.EndChild();

                ImGui.TableNextColumn();

                Dictionary<int, SmoothCounter>? stats = default;
                switch (_targetView)
                {
                    case TargetView.Total:
                        break;

                    case TargetView.Update:
                        stats = world.UpdateCounters;
                        break;

                    case TargetView.FixedUpdate:
                        stats = world.FixedUpdateCounters;
                        break;

                    case TargetView.Reactive:
                        stats = world.ReactiveCounters;
                        break;

                    case TargetView.PreRender:
                        stats = world.PreRenderCounters;
                        break;

                    case TargetView.Render:
                        stats = world.RenderCounters;
                        break;

                    case TargetView.GuiRender:
                        stats = world.GuiCounters;
                        break;

                    case TargetView.Startup:
                        stats = world.StartCounters;
                        break;
                }

                if (stats is not null)
                {
                    Dictionary<int, (string label, double size)> statistics = CalculateStatistics(world, _timePerSystems[(int)_targetView], stats);

                    // Histogram is 25px tall
                    ImGui.BeginChild("target", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 2 - 35));

                    DrawTab(world, stats, statistics);

                    ImGui.EndChild();

                    ImGuiHelpers.DrawHistogram(statistics.Values);
                }
            }
            ImGui.EndChild();

            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Input"))
        {
            DrawInputDiagnostics();

            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("2D Batches"))
        {
            if (ImGui.BeginChild("diagnostic_batch"))
            {
                foreach (var b in AssetsFilter.SpriteBatches)
                {
                    var batch = render.GetBatch(b.id);
                    ImGui.TextColored(Game.Profile.Theme.Accent, (b.name).ToString());

                    ImGui.TextColored(Game.Profile.Theme.Faded, "Max Items");
                    ImGui.SameLine();
                    ImGui.Text(batch.TotalItemCount.ToString());

                    ImGui.TextColored(Game.Profile.Theme.Faded, "Items");
                    ImGui.SameLine();
                    ImGui.Text(batch.ItemsQueued.ToString());

                    ImGui.Separator();
                }
            }
            ImGui.EndChild();
            ImGui.EndTabItem();
        }

        // Diagnostics tab .Begin()
        ImGui.End();
    }

    private void DrawInputDiagnostics()
    {
        ImGui.Separator();
        ImGui.Text("Buttons");
        ImGui.Separator();
        foreach (var button in Game.Input.AllButtons)
        {
            var b = Game.Input.GetOrCreateButton(button);
            ImGui.Text($"{button}: {(Game.Input.Down(button) ? "Down" : "Up")} {(b.Consumed ? "Consumed" : "")}");
            ImGui.TextColored(Game.Profile.Theme.Faded, $"{b.GetDescriptor()}");
        }
        ImGui.Spacing();
        ImGui.Spacing();

        ImGui.Separator();
        ImGui.Text("Axis");
        ImGui.Separator();
        foreach (var button in Game.Input.AllAxis)
        {
            var b = Game.Input.GetAxis(button);

            ImGui.Text($"{button}: {Game.Input.GetAxis(button).Value.X:0.00},{b.Value.Y:0.00} {b.IntValue} {b.Down}");
            ImGui.TextColored(Game.Profile.Theme.Faded, $"{string.Join(',', b.GetActiveButtonDescriptions())}");
        }
    }

    private enum TargetView
    {
        Update = 0,
        FixedUpdate = 1,
        Reactive = 2,
        PreRender = 3,
        Render = 4,
        GuiRender = 5,
        Startup = 6,
        Total = 7
    }

    /// <summary>
    /// This is the overall time reported per each system.
    /// </summary>
    private readonly double[] _timePerSystems = new double[(int)TargetView.Total];

    private TargetView _targetView = TargetView.Update;

    private void DrawTabs()
    {
        if (ImGui.Selectable($"Update ({PrintTime(TargetView.Update)})###update", _targetView == TargetView.Update))
        {
            _targetView = TargetView.Update;
        }

        if (ImGui.Selectable($"FixedUpdate ({PrintTime(TargetView.FixedUpdate)})###fixedupdate", _targetView == TargetView.FixedUpdate))
        {
            _targetView = TargetView.FixedUpdate;
        }

        if (ImGui.Selectable($"Reactive ({PrintTime(TargetView.Reactive)})###reactive", _targetView == TargetView.Reactive))
        {
            _targetView = TargetView.Reactive;
        }

        if (ImGui.Selectable($"Pre-Render ({PrintTime(TargetView.PreRender)})###prerender", _targetView == TargetView.PreRender))
        {
            _targetView = TargetView.PreRender;
        }

        if (ImGui.Selectable($"Render ({PrintTime(TargetView.Render)})###render", _targetView == TargetView.Render))
        {
            _targetView = TargetView.Render;
        }

        if (ImGui.Selectable($"GuiRender ({PrintTime(TargetView.GuiRender)})###guirender", _targetView == TargetView.GuiRender))
        {
            _targetView = TargetView.GuiRender;
        }

        if (ImGui.Selectable($"Startup ({PrintTime(TargetView.Startup)})###startup", _targetView == TargetView.Startup))
        {
            _targetView = TargetView.Startup;
        }

        ImGui.Separator();
    }

    private void DrawTab(MonoWorld world, IDictionary<int, SmoothCounter> stats, Dictionary<int, (string label, double size)> statistics)
    {
        using TableMultipleColumns systemsTable = new("systems_view", ImGuiTableFlags.Borders,
            20, -1, 50, 50, 50);

        ImGui.TableNextRow();
        ImGui.TableNextColumn();

        /* checkbox column */
        ImGui.TableNextColumn();

        ImGui.TextColored(Game.Profile.Theme.Accent, "Name");

        ImGui.TableNextColumn();
        ImGui.TextColored(Game.Profile.Theme.Accent, "Time (ms)");

        ImGui.TableNextColumn();
        ImGui.TextColored(Game.Profile.Theme.Accent, "Peak (ms)");

        ImGui.TableNextColumn();
        ImGui.TextColored(Game.Profile.Theme.Accent, "Entities");

        ImGui.TableNextRow();
        ImGui.TableNextColumn();

        foreach (var (systemId, counter) in stats)
        {
            Type tSystem = world.IdToSystem[systemId].GetType();

            bool isSelected = world.IsSystemActive(tSystem);
            if (ImGui.Checkbox($"##{systemId}", ref isSelected))
            {
                if (isSelected) world.ActivateSystem(tSystem);
                else world.DeactivateSystem(tSystem);
            }

            ImGui.TableNextColumn();

            ImGui.Text(tSystem.Name);
            ImGui.TableNextColumn();

            ImGui.Text(PrintTime(counter.AverageTime));

            ImGui.TableNextColumn();

            bool isHotPath = statistics[systemId].size > 20;
            if (isHotPath)
            {
                ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGuiHelpers.MakeColor32(Game.Profile.Theme.BgFaded));
            }

            ImGui.Text(PrintTime(counter.MaximumTime));

            ImGui.TableNextColumn();

            ImGui.Text(counter.AverageEntities.ToString());

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
        }
    }

    private void CalculateAllOverallTime(MonoWorld world)
    {
        _timePerSystems[(int)TargetView.Update] = world.UpdateCounters.Sum(k => k.Value.MaximumTime);
        _timePerSystems[(int)TargetView.FixedUpdate] = world.FixedUpdateCounters.Sum(k => k.Value.MaximumTime);
        _timePerSystems[(int)TargetView.Reactive] = world.ReactiveCounters.Sum(k => k.Value.MaximumTime);
        _timePerSystems[(int)TargetView.PreRender] = world.PreRenderCounters.Sum(k => k.Value.MaximumTime);
        _timePerSystems[(int)TargetView.Render] = world.RenderCounters.Sum(k => k.Value.MaximumTime);
        _timePerSystems[(int)TargetView.GuiRender] = world.GuiCounters.Sum(k => k.Value.MaximumTime);
        _timePerSystems[(int)TargetView.Startup] = world.StartCounters.Sum(k => k.Value.MaximumTime);
    }

    private Dictionary<int, (string name, double size)> CalculateStatistics(MonoWorld world, double overallTime, IDictionary<int, SmoothCounter> stats)
    {
        Dictionary<int, (string name, double size)> statistics = new();
        foreach (var (systemId, counter) in stats)
        {
            float size = (float)(counter.MaximumTime / overallTime * 100);

            statistics[systemId] = (
                name: $"{world.IdToSystem[systemId].GetType().Name} ({Calculator.RoundToInt(size)}%%)",
                size);
        }

        return statistics;
    }

    private string PrintTime(TargetView target) => PrintTime(_timePerSystems[(int)target]);

    private static string PrintTime(double microsseconds) => (microsseconds / 1000f).ToString("0.00");
}
