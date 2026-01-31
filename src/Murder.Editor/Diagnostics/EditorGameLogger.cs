using Bang;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Utilities;
using System.Numerics;

namespace Murder.Editor.Diagnostics
{
    public class EditorGameLogger : GameLogger
    {
        private string _input = string.Empty;
        private string _filterText = string.Empty;

        private int _pressedBackCount = 0;
        private float _logHeight = 300f;
        private const float MinLogHeight = 100f;
        private const float MaxLogHeight = 800f;

        public static Dictionary<string, object> TrackedVariables = new();

        [Flags]
        private enum LogFilter
        {
            None = 0,
            Log = 1 << 0,
            Debug = 1 << 1,
            Perf = 1 << 2,
            Warning = 1 << 3,
            Error = 1 << 4,
            Command = 1 << 5,
            All = Log | Debug | Perf | Warning | Error | Command
        }

        private LogFilter _activeFilters = LogFilter.All;

        public override void TrackImpl(string variableName, object value)
        {
            TrackedVariables[variableName] = value;
        }

        public static EditorGameLogger OverrideInstanceWithEditor()
        {
            EditorGameLogger logger = new EditorGameLogger();
            _instance = logger;

            return logger;
        }

        /// <summary>
        /// Draws the console of the game.
        /// </summary>
        public override void DrawConsole(Func<string, string>? onInputAction = default, World? world = null)
        {
            if (!_showDebug) return;

            ImGui.SetNextWindowBgAlpha(.8f);
            ImGuiWindowFlags winStyle = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar;
            float width = ImGui.GetWindowViewport().Size.X;

            if (ImGui.Begin("Console", winStyle))
            {
                ImGui.SetWindowFocus();

                ImGui.SetWindowSize(new Vector2(width, -1));
                ImGui.SetWindowPos(new Vector2(0, 0));

                bool copy = false;
                ImGui.SetNextWindowBgAlpha(.1f);
                TopBar(ref copy);

                ImGui.Separator();

                ImGui.SetNextWindowBgAlpha(.1f);
                LogText(copy);

                // Resize handle
                if (world?.TryGetUnique<Components.EditorComponent>() is Components.EditorComponent editorComponent)
                {
                    ResizeHandle(editorComponent.EditorHook);
                }
                else
                {
                    ResizeHandle(null);
                }

                Input(onInputAction);
                ImGui.End();
            }
        }

        private void FilterButton(string label, LogFilter filter, Vector4 color)
        {
            bool isActive = _activeFilters.HasFlag(filter);

            if (isActive)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, color * 0.6f);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, color * 0.8f);
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, color);
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 1));
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.2f, 0.2f, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.4f, 0.4f, 0.4f, 1));
                ImGui.PushStyleColor(ImGuiCol.Text, color * 0.7f);
            }

            if (ImGui.Button(label))
            {
                _activeFilters ^= filter; // Toggle
            }

            ImGui.PopStyleColor(4);
        }
        protected override void TopBar(ref bool copy)
        {
            if (ImGui.BeginChild("top_console", new(-1, ImGui.GetFontSize() * 1.75f)))
            {
                if (ImGui.Button("Clear"))
                {
                    ClearLog();
                }

                ImGui.SameLine();
                ImGuiHelpers.HelpTooltip("Clear all the log output!");

                copy = ImGui.Button("Copy");
                ImGui.SameLine();
                ImGuiHelpers.HelpTooltip("Copy the whole log!");

                if (ImGui.Button("Close"))
                {
                    Toggle(false);
                }

                ImGuiHelpers.HelpTooltip("Close logs!");
                ImGui.SameLine();

                // Text filter
                ImGui.SetNextItemWidth(200);
                if (ImGui.InputTextWithHint("##filter", "Filter logs...", ref _filterText, 256))
                {
                    _scrollToBottom = 2;
                }

                ImGui.SameLine();
                if (ImGui.Button("×##clearfilter"))
                {
                    _filterText = string.Empty;
                }

                ImGui.SameLine();
                ImGui.Spacing();
                ImGui.SameLine();

                // Filter toggle buttons
                FilterButton("Log", LogFilter.Log, new Vector4(1, 1, 1, 1));
                ImGui.SameLine();
                FilterButton("Debug", LogFilter.Debug, new Vector4(1, 1, 1, 0.5f));
                ImGui.SameLine();
                FilterButton("\uf201", LogFilter.Perf, new Vector4(0.5f, 1, 0.5f, 1));
                ImGui.SameLine();
                FilterButton("\uf071", LogFilter.Warning, new Vector4(1, 1, 0.5f, 1));
                ImGui.SameLine();
                FilterButton("\uf714", LogFilter.Error, new Vector4(1, 0.25f, 0.5f, 1));
                ImGui.SameLine();
                FilterButton("\uf120", LogFilter.Command, new Vector4(.5f, 1, .3f, 1));

                ImGui.SameLine();
                ImGui.Spacing();
                ImGui.SameLine();

                if (ImGui.Button("All"))
                {
                    _activeFilters = LogFilter.All;
                }
                ImGui.SameLine();
                if (ImGui.Button("None"))
                {
                    _activeFilters = LogFilter.None;
                }
                ImGui.EndChild();
            }
        }

        private void ResizeHandle(EditorHook? hook)
        {
            // Invisible button as resize handle
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.5f, 0.5f, 0.5f, 0.3f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.7f, 0.7f, 0.7f, 0.5f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.9f, 0.9f, 0.9f, 0.7f));

            ImGui.Button("##resize", new Vector2(-1, 6));

            if (ImGui.IsItemActive())
            {
                _logHeight += ImGui.GetIO().MouseDelta.Y;
                _logHeight = Math.Clamp(_logHeight, MinLogHeight, MaxLogHeight);
            }

            if (hook != null)
            {
                if (!hook.CursorIsBusy.Any())
                {
                    if (ImGui.IsItemHovered() || ImGui.IsItemActive())
                    {
                        hook.Cursor = Core.CursorStyle.Hand;
                        hook.CursorIsBusy.Add(typeof(EditorGameLogger));
                    }
                    else
                    {
                        hook.Cursor = Core.CursorStyle.Normal;
                        hook.CursorIsBusy.Remove(typeof(EditorGameLogger));
                    }
                }
            }
            else
            {
                if (ImGui.IsItemHovered() || ImGui.IsItemActive())
                {
                    Architect.Instance.Cursor = Core.CursorStyle.Hand;
                }
                else
                {
                    Architect.Instance.Cursor = Core.CursorStyle.Normal;
                }
            }
            ImGui.PopStyleColor(3);
        }

        protected override void LogText(bool copy)
        {
            if (ImGui.BeginChild("scrolling_console", new(-1, _logHeight)))
            {
                if (copy)
                {
                    string allText = string.Empty;
                    for (int i = 0; i < _log.Count; ++i)
                    {
                        var logLine = _log[i];
                        // Check type filter
                        if (!PassesTypeFilter(logLine.Type))
                            continue;
                        // Check text filter
                        if (!string.IsNullOrEmpty(_filterText) &&
                            !StringHelper.FuzzyMatch(_filterText, logLine.Message))
                            continue;
                        string msg = logLine.Repeats <= 1 ? logLine.Message : $"{logLine.Message} ({logLine.Repeats})";
                        allText += msg + "\n";
                    }
                    ImGui.SetClipboardText(allText);
                }

                ImGui.PushTextWrapPos();

                for (int i = 0; i < _log.Count; ++i)
                {
                    var logLine = _log[i];

                    // Check type filter
                    if (!PassesTypeFilter(logLine.Type))
                        continue;

                    // Check text filter
                    if (!string.IsNullOrEmpty(_filterText) &&
                        !StringHelper.FuzzyMatch(_filterText, logLine.Message))
                        continue;

                    string msg = logLine.Repeats <= 1 ? logLine.Message : $"{logLine.Message} ({logLine.Repeats})";

                    ImGui.PushStyleColor(ImGuiCol.Text, logLine.Color);
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 0, 0));
                    ImGui.PushItemWidth(ImGui.GetColumnWidth());

                    ImGui.InputText($"##log{i}", ref msg, 256, ImGuiInputTextFlags.ReadOnly);

                    ImGui.PopStyleColor(2);
                    ImGui.PopItemWidth();
                }

                ImGui.PopTextWrapPos();

                if (_scrollToBottom > 0)
                {
                    ImGui.SetScrollHereY(1f);
                    _scrollToBottom--;
                }

                ImGui.EndChild();
            }
        }

        private bool PassesTypeFilter(LogType logType)
        {
            return logType switch
            {
                LogType.Log => _activeFilters.HasFlag(LogFilter.Log),
                LogType.Debug => _activeFilters.HasFlag(LogFilter.Debug),
                LogType.Perf => _activeFilters.HasFlag(LogFilter.Perf),
                LogType.Warning => _activeFilters.HasFlag(LogFilter.Warning),
                LogType.Error => _activeFilters.HasFlag(LogFilter.Error),
                LogType.Command => _activeFilters.HasFlag(LogFilter.Command),
                _ => true
            };
        }

        protected override void Input(Func<string, string>? onInputAction)
        {
            string current = _input;
            string id = "log_console";
            if (Game.Input.Shortcut(Keys.Up) && _lastInputIndex != 0)
            {
                _pressedBackCount++;

                if (_lastInputIndex - _pressedBackCount < 0)
                {
                    _pressedBackCount = 1;
                }

                // ImGui does some _weird_ caching and we need to do the input through a different
                // id if we want the cached value to go through.
                id = "log_console_reset";
                current = _lastInputs[_lastInputIndex - _pressedBackCount];
                _resetInputFocus = true;
            }

            if (Game.Input.Shortcut(Keys.Tab))
            {
                _resetInputFocus = true;
                _input = GetClosest(current);
                current = _input;
                id = "log_console_reset";
            }

            if (_resetInputFocus)
            {
                ImGui.SetKeyboardFocusHere(0);
                _resetInputFocus = false;
            }
            ImGui.PushItemWidth(-1);
            if (ImGui.InputTextWithHint($"##{id}", "help", ref current, maxLength: 256, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                LogCommand(_input);

                if (onInputAction is not null)
                {
                    string output = onInputAction.Invoke(_input);
                    LogCommandOutput(output);
                }

                _resetInputFocus = true;
                current = string.Empty;

                _pressedBackCount = 0;
            }
            ImGui.PopItemWidth();


            _input = current;
        }

        private string GetClosest(string current)
        {
            if (current == string.Empty)
            {
                return "help";
            }

            foreach (var command in CommandServices.AllCommands)
            {
                string commandName = command.Key;
                if (commandName.StartsWith(current))
                {
                    return commandName;
                }
            }

            return string.Empty;
        }
    }
}