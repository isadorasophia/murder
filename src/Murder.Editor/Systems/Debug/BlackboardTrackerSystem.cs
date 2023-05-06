using Bang.Contexts;
using Bang.Diagnostics;
using Bang.Systems;
using ImGuiNET;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Immutable;
using Murder.Editor.CustomComponents;
using Murder.Data;
using Murder.Services;
using Murder.Save;
using Murder.Core.Dialogs;
using Murder.Core.Geometry;
using Murder.Editor.CustomFields;
using Murder.Assets.Graphics;

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

        /// <summary>
        /// This is only used for rendering the entity components during the game (on debug mode).
        /// </summary>
        public void DrawGui(RenderContext render, Context context)
        {
            int maxWidth = 710;

            // Graphics
            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowSizeConstraints(
                size_min: new System.Numerics.Vector2(500, 500),
                size_max: new System.Numerics.Vector2(maxWidth, 800));

            int padding = 25;
            ImGui.SetWindowPos(new(x: render.ScreenSize.X - maxWidth, y: padding), ImGuiCond.Appearing);

            if (!ImGui.Begin("Blackboards"))
            {
                // Window is closed, so just go way...
                return;
            }

            ImGui.BeginChild("blackboard_child");
            {
                DrawBlackaboardsTable(padding);
            }

            ImGui.EndChild();

            // Diagnostics tab .Begin()
            ImGui.End();
        }

        private void DrawBlackaboardsTable(int padding)
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
                size: new System.Numerics.Vector2(-1, height), border: true, ImGuiWindowFlags.NoDocking);

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
                var (_, blackboard) = blackboards[_targetBlackboard];

                bool changed = CustomComponent.ShowEditorOf(ref blackboard);
                if (changed)
                {
                    MurderSaveServices.CreateOrGetSave().BlackboardTracker.OnModified();
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
