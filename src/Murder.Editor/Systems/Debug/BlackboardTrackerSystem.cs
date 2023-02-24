using Bang.Contexts;
using Bang.Diagnostics;
using Bang.Systems;
using ImGuiNET;
using Murder.Core.Graphics;
using Murder.Core;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using System.Diagnostics.CodeAnalysis;
using Murder.Core.Dialogs;
using System.Reflection;
using System.Collections.Immutable;
using Murder.Utilities;
using Murder.Editor.CustomComponents;

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
        /// <summary>
        /// This is only used for rendering the entity components during the game (on debug mode).
        /// </summary>
        public void DrawGui(RenderContext render, Context context)
        {
            ImmutableDictionary<string, (Type t, IBlackboard blackboard)> blackboards =
                MurderSaveServices.CreateOrGetSave().BlackboardTracker.FetchBlackboards();

            if (blackboards.IsEmpty)
            {
                return;
            }

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
                using TableMultipleColumns table = new("blackboard_view", ImGuiTableFlags.Resizable, 100, -1);

                float height = ImGui.GetContentRegionAvail().Y - padding / 5f;

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.BeginChild("blackboard_child_view",
                    size: new System.Numerics.Vector2(-1, height), border: true, ImGuiWindowFlags.NoDocking);

                DrawTabs(blackboards.Keys);

                ImGui.EndChild();

                ImGui.TableNextColumn();

                var (_, blackboard) = blackboards[_targetBlackboard];
                bool changed = CustomComponent.ShowEditorOf(ref blackboard);
            }

            ImGui.EndChild();

            // Diagnostics tab .Begin()
            ImGui.End();
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
