using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using System.Numerics;

namespace Murder.Editor.Diagnostics
{
    public class EditorGameLogger : GameLogger
    {
        private string _input = string.Empty;

        private int _pressedBackCount = 0;

        public static Dictionary<string, object> TrackedVariables = new();

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
        public override void DrawConsole(Func<string, string>? onInputAction = default)
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

                ImGui.Separator();

                Input(onInputAction);

                ImGui.End();
            }

            Game.Input.ConsumeAll();
        }

        protected override void TopBar(ref bool copy)
        {
            if (ImGui.BeginChild("top_console", new(-1, ImGui.GetFontSize() * 1.75f)))
            {
                if (ImGui.Button("Clear")) ClearLog();
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

                ImGui.EndChild();
            }
        }

        protected override void LogText(bool copy)
        {
            if (ImGui.BeginChild("scrolling_console", new(-1, ImGui.GetFontSize() * 20)))
            {
                if (copy)
                {
                    ImGui.LogToClipboard();
                }

                ImGui.PushTextWrapPos();

                for (int i = 0; i < _log.Count; ++i)
                {
                    string msg = _log[i].Repeats <= 1 ? _log[i].Message : $"{_log[i].Message} ({_log[i].Repeats})";

                    ImGui.PushStyleColor(ImGuiCol.Text, _log[i].Color);
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

            if (ImGui.IsItemFocused())
            {
                _resetInputFocus = false;
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