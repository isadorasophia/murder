using Bang.Contexts;
using Bang.Diagnostics;
using Bang.Systems;
using ImGuiNET;
using Murder.Core;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Utilities;

namespace Murder.Editor.Systems
{
    /// <summary>
    /// This system will draw selected entities and drag them through the map.
    /// </summary>
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    public class SystemsDiagnosticsSystem : IGuiSystem
    {
        /// <summary>
        /// This is only used for rendering the entity components during the game (on debug mode).
        /// </summary>
        public void DrawGui(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;
            MonoWorld world = (MonoWorld)context.World;

            int maxWidth = 710.WithDpi();

            // Graphics
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                size_min: new System.Numerics.Vector2(500.WithDpi(), 250.WithDpi()), 
                size_max: new System.Numerics.Vector2(maxWidth, 600.WithDpi()));

            int padding = 25.WithDpi();
            ImGui.SetWindowPos(new(x: ImGui.GetWindowWidth() - maxWidth, y: padding), ImGuiCond.Appearing);

            if (!ImGui.Begin("Diagnostics"))
            {
                // Window is closed, so just go way...
                return;
            }

            {
                using TableMultipleColumns table = new("dianogstics_view", ImGuiTableFlags.NoBordersInBody, 150, -1);

                float height = ImGui.GetContentRegionAvail().Y - padding / 5f;

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.BeginChild("systems_diagnostics",
                    size: new System.Numerics.Vector2(-1, height), border: true, ImGuiWindowFlags.NoDocking);

                CalculateAllOverallTime(world);
                
                DrawTabs();

                ImGui.EndChild();

                ImGui.TableNextColumn();

                Dictionary<int, SmoothCounter>? stats = default;
                switch (_targetView)
                {
                    case TargetView.None:
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

                    case TargetView.Render:
                        stats = world.RenderCounters;
                        break;
                        
                    case TargetView.GuiRender:
                        stats = world.GuiCounters;
                        break;
                }

                ImGui.Text($"a: {world.OverallUpdateTime.AverageTime}, p: {world.OverallUpdateTime.MaximumTime}");

                if (stats is not null)
                {
                    Dictionary<int, (string label, double size)> statistics = CalculateStatistics(world, _timePerSystems[(int)_targetView], stats);
                    
                    DrawTab(world, stats, statistics);
                    ImGuiHelpers.DrawHistogram(statistics.Values);
                }
            }

            // Diagnostics tab .Begin()
            ImGui.End();
        }

        private enum TargetView
        {
            Update = 0,
            FixedUpdate = 1,
            Reactive = 2,
            Render = 3,
            GuiRender = 4,
            None = 5
        }

        /// <summary>
        /// This is the overall time reported per each system.
        /// </summary>
        private readonly double[] _timePerSystems = new double[5];

        private TargetView _targetView = TargetView.None;

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

            if (ImGui.Selectable($"Render ({PrintTime(TargetView.Render)})###render", _targetView == TargetView.Render))
            {
                _targetView = TargetView.Render;
            }
            
            if (ImGui.Selectable($"GuiRender ({PrintTime(TargetView.GuiRender)})###guirender", _targetView == TargetView.GuiRender))
            {
                _targetView = TargetView.GuiRender;
            }
        }

        private void DrawTab(MonoWorld world, IDictionary<int, SmoothCounter> stats, Dictionary<int, (string label, double size)> statistics)
        {
            using TableMultipleColumns systemsTable = new("systems_view", ImGuiTableFlags.Borders, 
                20.WithDpi(), 250.WithDpi(), 80.WithDpi(), 80.WithDpi(), 100.WithDpi());

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
            _timePerSystems[(int)TargetView.Render] = world.RenderCounters.Sum(k => k.Value.MaximumTime);
            _timePerSystems[(int)TargetView.GuiRender] = world.GuiCounters.Sum(k => k.Value.MaximumTime);
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
}
