using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Diagnostics;
using Murder.Editor.ImGuiExtended;

namespace Murder.Editor.Systems.Debug;

[DoNotPause]
[OnlyShowOnDebugView]
[Filter(ContextAccessorFilter.None)]
public class DebugTrackerSystemm : IGuiSystem   
{
    private bool _showHierarchy = false;
    public void DrawGui(RenderContext render, Context context)
    {

        ImGui.BeginMainMenuBar();

        if (ImGui.BeginMenu("Show"))
        {
            ImGuiHelpers.MenuItem("Variable Tracker", "", ref _showHierarchy);
            ImGui.EndMenu();
        }
        ImGui.EndMainMenuBar();


        if (_showHierarchy)
        {
            if (ImGui.Begin("Tracker", ref _showHierarchy, ImGuiWindowFlags.AlwaysAutoResize))
            {
                if (ImGuiHelpers.Button("Clear"))
                {
                    EditorGameLogger.TrackedVariables.Clear();
                }
                foreach (var item in EditorGameLogger.TrackedVariables)
                {
                    ImGui.Text(item.Key);
                    ImGui.SameLine();
                    ImGui.TextColored(Game.Profile.Theme.Faded, item.Value.ToString());
                }
            }
            ImGui.End();
        }
    }
}