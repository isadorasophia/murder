using Bang.Contexts;
using Bang.Diagnostics;
using Bang.Systems;
using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Save;
using Murder.Services;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.Systems.Debug
{
    /// <summary>
    /// This system will draw selected entities and drag them through the map.
    /// </summary>
    [DoNotPause]
    [OnlyShowOnDebugView]
    [Filter(ContextAccessorFilter.None)]
    public class BlackboardTrackerSystem : IGuiSystem
    {
        private const string NoBlackboardName = "(Other)";
        private bool _showBlackboard = false;

        /// <summary>
        /// This is only used for rendering the entity components during the game (on debug mode).
        /// </summary>
        public void DrawGui(RenderContext render, Context context)
        {
            ImGui.BeginMainMenuBar();

            if (ImGui.BeginMenu("Show"))
            {
                ImGui.MenuItem("Blackboard", "", ref _showBlackboard);
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();

            if (!_showBlackboard)
                return;

            int maxWidth = 1200;

            // Graphics
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                size_min: new System.Numerics.Vector2(500, 500),
                size_max: new System.Numerics.Vector2(maxWidth, 800));

            int padding = 25;
            ImGui.SetWindowPos(new(x: render.Viewport.Size.X - maxWidth, y: padding), ImGuiCond.Appearing);

            if (!ImGui.Begin("Blackboards"))
            {
                // Window is closed, so just go way...
                return;
            }

            ImGui.BeginChild("blackboard_child");
            {
                DrawBlackboardsTable(padding);
            }

            ImGui.EndChild();

            // Diagnostics tab .Begin()
            ImGui.End();
        }

        private void DrawBlackboardsTable(int padding)
        {
            using TableMultipleColumns table = new("blackboard_view", ImGuiTableFlags.Resizable, 100, -1);
            if (!table.Opened)
            {
                return;
            }

            BlackboardTracker tracker = MurderSaveServices.CreateOrGetSave().BlackboardTracker;
            ImmutableDictionary<string, BlackboardInfo> blackboards = tracker.FetchBlackboards();

            if (blackboards.IsEmpty)
            {
                return;
            }

            float height = ImGui.GetContentRegionAvail().Y - padding / 5f;

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            ImGui.BeginChild("blackboard_child_view",
                size: new System.Numerics.Vector2(-1, height), ImGuiChildFlags.Border, ImGuiWindowFlags.NoDocking);

            DrawTabs(blackboards.Keys.Append(NoBlackboardName));

            ImGui.EndChild();

            ImGui.TableNextColumn();

            if (string.Equals(_targetBlackboard, NoBlackboardName))
            {
                if (CustomField.DrawValue(ref tracker, "_variablesWithoutBlackboard"))
                {
                    // Don't do anything when modifying these dictionary variables.
                }
            }
            else
            {
                IBlackboard blackboard = blackboards[_targetBlackboard].Blackboard;

                bool changed = CustomComponent.ShowEditorOf(ref blackboard);
                if (changed)
                {
                    MurderSaveServices.CreateOrGetSave().BlackboardTracker.OnModified(blackboard.Kind);
                }
            }
        }

        private string? _targetBlackboard;

        [MemberNotNull(nameof(_targetBlackboard))]
        private void DrawTabs(IEnumerable<string> blackboards)
        {
            _targetBlackboard ??= blackboards.First();

            foreach (string name in blackboards)
            {
                if (ImGui.Selectable(name, _targetBlackboard == name))
                {
                    _targetBlackboard = name;
                }
            }
        }
    }
}