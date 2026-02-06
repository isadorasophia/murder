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
public class SystemsDiagnosticsSystem : IGuiSystem, IUpdateSystem
{
    private bool _showDiagnostics = false;

    private string _systemsFilter = "";
    private Dictionary<int, PerfSmoothCounter>? _stats;

    private enum IncidentTypes
    {
        SystemPeak,
        General
    }
    private readonly struct IncidentReport()
    {
        public readonly IncidentTypes IncidentType { get; init; }
        public readonly int SystemId { get; init; }
        public readonly float StallTime { get; init; }
        public readonly TargetView Where { get; init; }
        public int SampleIndex { get; init; }
        public int Repeat { get; init; } = 0;
    }

    private readonly List<IncidentReport> _incidentReports = new List<IncidentReport>(200);
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

                _stats = default;
                switch (_targetView)
                {
                    case TargetView.Total:
                        break;

                    case TargetView.Update:
                        _stats = world.UpdateCounters;
                        break;

                    case TargetView.FixedUpdate:
                        _stats = world.FixedUpdateCounters;
                        break;

                    case TargetView.Reactive:
                        _stats = world.ReactiveCounters;
                        break;

                    case TargetView.PreRender:
                        _stats = world.PreRenderCounters;
                        break;

                    case TargetView.Render:
                        _stats = world.RenderCounters;
                        break;

                    case TargetView.GuiRender:
                        _stats = world.GuiCounters;
                        break;

                    case TargetView.Startup:
                        _stats = world.StartCounters;
                        break;
                }

                if (_stats is not null)
                {
                    Dictionary<int, (string label, double size)> statistics = CalculateStatistics(world, _timePerSystems[(int)_targetView], _stats);

                    // Histogram is 25px tall
                    ImGui.BeginChild("target", new System.Numerics.Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 2 - 35));

                    DrawTab(world, _stats, statistics);

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

                    ImGui.TextColored(Game.Profile.Theme.Faded, "Max texture swaps");
                    ImGui.SameLine();
                    ImGui.Text(batch.MaxTextureSwaps.ToString());

                    ImGui.Separator();
                }
            }
            ImGui.EndChild();
            ImGui.EndTabItem();
        }

        if (ImGui.BeginTabItem("Diagnostics Log"))
        {
            if (ImGui.Button("Clear"))
            {
                _incidentReports.Clear();
            }
            // Draw a little box with all incident reports.
            ImGui.BeginChild("diagnostic_logs");
            {
                for (int i = 0; i < _incidentReports.Count; i++)
                {
                    IncidentReport report = _incidentReports[i];

                    if (report.IncidentType == IncidentTypes.SystemPeak)
                    {
                        Type tSystem = world.IdToSystem[report.SystemId].GetType();
                        Vector4 color;

                        switch (report.Where)
                        {
                            case TargetView.Update:
                                color = new Vector4(0.4f, 0.6f, 1f, 1f);
                                break;
                            case TargetView.FixedUpdate:
                                color = new Vector4(0.4f, 1f, 0.6f, 1f);
                                break;
                            case TargetView.Reactive:
                                color = new Vector4(1f, 0.6f, 0.4f, 1f);
                                break;
                            case TargetView.PreRender:
                                color = new Vector4(1f, 0.4f, 0.6f, 1f);
                                break;
                            case TargetView.Render:
                                color = new Vector4(0.6f, 1f, 0.4f, 1f);
                                break;
                        }

                        ImGui.TextColored(Game.Profile.Theme.Accent, $"{tSystem.Name} stalled for {report.StallTime:0.00} ms during {report.Where}");
                    }
                    else if (report.IncidentType == IncidentTypes.General)
                    {
                        if (report.Repeat > 0)
                        {
                            ImGui.TextColored(Game.Profile.Theme.Red, $"Frame stalled {report.Repeat} times with a max of {report.StallTime:0.00} ms");
                        }
                        else
                        {
                            ImGui.TextColored(Game.Profile.Theme.Red, $"Frame stalled for {report.StallTime:0.00} ms");
                        }
                    }

                    if (i == _incidentReports.Count - 1)
                    {
                        ImGui.SetScrollHereY();
                    }
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

    private void DrawTab(MonoWorld world, IDictionary<int, PerfSmoothCounter> stats, Dictionary<int, (string label, double size)> statistics)
    {
        ImGui.InputTextWithHint("", "Filter", ref _systemsFilter, 256);

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

            if (!string.IsNullOrWhiteSpace(_systemsFilter))
            {
                var systemName = tSystem.Name.ToLowerInvariant();
                if (!StringHelper.FuzzyMatch(_systemsFilter.ToLowerInvariant(), systemName))
                {
                    continue;
                }
            }

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

    private Dictionary<int, (string name, double size)> CalculateStatistics(MonoWorld world, double overallTime, IDictionary<int, PerfSmoothCounter> stats)
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

    public void Update(Context context)
    {
        MonoWorld world = (MonoWorld)context.World;

        float stallTreshold = 10f * 1000; // milliseconds
        foreach (var (systemId, counter) in world.UpdateCounters)
        {
            if (counter.CurrentIndex <= 2) // Ignore first few frames.
            {
                continue;
            }

            if (_incidentReports.Count > 0 && _incidentReports[^1].SampleIndex == counter.CurrentIndex &&
                _incidentReports[^1].SystemId == systemId &&
                _incidentReports[^1].Where == TargetView.Update)
            {
                // Already reported on another phase.
                continue;
            }

            if (counter.CurrentTime > stallTreshold)
            {
                _incidentReports.Add(new IncidentReport
                {
                    IncidentType = IncidentTypes.SystemPeak,
                    SystemId = systemId,
                    StallTime = (float)counter.CurrentTime / 1000f,
                    Where = TargetView.Update,
                    SampleIndex = counter.CurrentIndex
                });
            }
        }

        foreach (var (systemId, counter) in world.FixedUpdateCounters)
        {
            if (counter.CurrentIndex <= 2) // Ignore first few frames.
            {
                continue;
            }

            if (_incidentReports.Count > 0 && _incidentReports[^1].SampleIndex == counter.CurrentIndex &&
                _incidentReports[^1].SystemId == systemId &&
                _incidentReports[^1].Where == TargetView.FixedUpdate)
            {
                // Already reported on another phase.
                continue;
            }

            if (counter.CurrentTime > stallTreshold)
            {
                _incidentReports.Add(new IncidentReport
                {
                    IncidentType = IncidentTypes.SystemPeak,
                    SystemId = systemId,
                    StallTime = (float)counter.CurrentTime / 1000f,
                    Where = TargetView.FixedUpdate,
                    SampleIndex = counter.CurrentIndex
                });
            }
        }

        foreach (var (systemId, counter) in world.ReactiveCounters)
        {
            if (counter.CurrentIndex <= 2) // Ignore first few frames.
            {
                continue;
            }

            if (_incidentReports.Count > 0 && _incidentReports[^1].SampleIndex == counter.CurrentIndex &&
                _incidentReports[^1].SystemId == systemId &&
                _incidentReports[^1].Where == TargetView.Reactive)
            {
                // Already reported on another phase.
                continue;
            }

            if (counter.CurrentTime > stallTreshold)
            {
                _incidentReports.Add(new IncidentReport
                {
                    IncidentType = IncidentTypes.SystemPeak,
                    SystemId = systemId,
                    StallTime = (float)counter.CurrentTime / 1000f,
                    Where = TargetView.Reactive,
                    SampleIndex = counter.CurrentIndex
                });
            }
        }

        foreach (var (systemId, counter) in world.PreRenderCounters)
        {
            if (counter.CurrentIndex <= 2) // Ignore first few frames.
            {
                continue;
            }
            if (_incidentReports.Count > 0 && _incidentReports[^1].SampleIndex == counter.CurrentIndex &&
                _incidentReports[^1].SystemId == systemId &&
                _incidentReports[^1].Where == TargetView.PreRender)
            {
                // Already reported on another phase.
                continue;
            }
            if (counter.CurrentTime > stallTreshold)
            {
                _incidentReports.Add(new IncidentReport
                {
                    IncidentType = IncidentTypes.SystemPeak,
                    SystemId = systemId,
                    StallTime = (float)counter.CurrentTime / 1000f,
                    Where = TargetView.PreRender,
                    SampleIndex = counter.CurrentIndex
                });
            }
        }

        foreach (var (systemId, counter) in world.RenderCounters)
        {
            if (counter.CurrentIndex <= 2) // Ignore first few frames.
            {
                continue;
            }

            if (_incidentReports.Count > 0 && _incidentReports[^1].SampleIndex == counter.CurrentIndex &&
                _incidentReports[^1].SystemId == systemId &&
                _incidentReports[^1].Where == TargetView.Render)
            {
                // Already reported on another phase.
                continue;
            }

            if (counter.CurrentTime > stallTreshold)
            {
                _incidentReports.Add(new IncidentReport
                {
                    IncidentType = IncidentTypes.SystemPeak,
                    SystemId = systemId,
                    StallTime = (float)counter.CurrentTime / 1000f,
                    Where = TargetView.Render,
                    SampleIndex = counter.CurrentIndex
                });
            }
        }

        foreach (var (systemId, counter) in world.GuiCounters)
        {
            if (counter.CurrentIndex <= 2) // Ignore first few frames.
            {
                continue;
            }
            if (_incidentReports.Count > 0 && _incidentReports[^1].SampleIndex == counter.CurrentIndex &&
                _incidentReports[^1].SystemId == systemId &&
                _incidentReports[^1].Where == TargetView.GuiRender)
            {
                // Already reported on another phase.
                continue;
            }
            if (counter.CurrentTime > stallTreshold)
            {
                _incidentReports.Add(new IncidentReport
                {
                    IncidentType = IncidentTypes.SystemPeak,
                    SystemId = systemId,
                    StallTime = (float)counter.CurrentTime / 1000f,
                    Where = TargetView.GuiRender,
                    SampleIndex = counter.CurrentIndex
                });
            }
        }

        // Check for last frame duration
        if (Game.DeltaTime >= 1 / 30f)
        {
            if (_incidentReports.Count > 0 && _incidentReports[^1].SampleIndex == -1 &&
                _incidentReports[^1].IncidentType == IncidentTypes.General)
            {
                // Already reported on another phase. Incement
                var lastIncident = _incidentReports[^1];
                _incidentReports[^1] = lastIncident with
                {
                    Repeat = lastIncident.Repeat + 1,
                    StallTime = Math.Max(lastIncident.StallTime, Game.DeltaTime * 1000f)
                };
                return;
            }

            _incidentReports.Add(new IncidentReport
            {
                IncidentType = IncidentTypes.General,
                StallTime = Game.DeltaTime * 1000f,
                Where = TargetView.Total,
                SampleIndex = -1
            });
        }
    }
}
