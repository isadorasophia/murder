using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Murder.Core.Graphics;
using Murder.Diagnostics;
using Murder.Editor.Attributes;
using System.Numerics;

namespace Murder.Editor.Systems.Debug
{
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    internal class DebugPlotterSystem : IGuiSystem
    {
        private bool _showHierarchy = false;
        public void DrawGui(RenderContext render, Context context)
        {

            ImGui.BeginMainMenuBar();

            if (ImGui.BeginMenu("Show"))
            {
                ImGui.MenuItem("Graph Debugger", "", ref _showHierarchy);
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();

            if (_showHierarchy)
            {
                ImGui.SetNextWindowSize(new Vector2(350, 150), ImGuiCond.Appearing);
                if (ImGui.Begin("Graph", ref _showHierarchy, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    EditorGraphLogger logger = Architect.EditorGraphLogger;
                    foreach (var entry in logger.Graphs)
                    {
                        if (entry.Value.Values.Length > 0)
                        {
                            ImGui.PlotHistogram("", ref entry.Value.Values[0], entry.Value.Values.Length, 0, $"{entry.Key} ({entry.Value.Values.Length})", 0, entry.Value.Max, new Vector2(310, 75));
                        }
                        else
                        {
                            ImGui.Text($"Graph {entry.Key} is empty");
                        }
                    }
                    if (logger.Graphs.Count == 0)
                    {
                        ImGui.Text("Use GameLogger.PlotGraph(float value) to start plottting!");
                    }
                    else if (ImGui.Button("Clear All"))
                    {
                        GameLogger.ClearAllGraphs();
                    }
                }
                ImGui.End();
            }
        }
    }
}
