using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Diagnostics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using System.Numerics;
using System.Text;

namespace Murder.Editor.Diagnostics
{
    public class EditorGameLogger : GameLogger
    {
        private string _input = string.Empty;

        private int _pressedBackCount = 0;

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

                ImGui.SameLine(ImGui.GetWindowWidth() - 30);

                if (ImGui.Button("\uf00d"))
                {
                    Toggle(false);
                }

                ImGuiHelpers.HelpTooltip("Close logs \uf256");

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
            if (ImGui.BeginChild("input_console", new(-1, ImGui.GetFontSize() * 1.5f)))
            {
                ImGui.PushItemWidth(ImGui.GetColumnWidth());

                if (_resetInputFocus)
                {
                    ImGui.SetKeyboardFocusHere();
                    _resetInputFocus = false;
                }

                if (Game.Input.Shortcut(Keys.Up) && _lastInputIndex != 0)
                {
                    _pressedBackCount++;

                    if (_lastInputIndex - _pressedBackCount < 0)
                    {
                        _pressedBackCount = 1;
                    }

                    // ImGui does some _weird_ caching and we need to do the input through a different
                    // id if we want the cached value to go through.
                    string cached = _lastInputs[_lastInputIndex - _pressedBackCount];
                    ImGui.InputText("##log_cached_console", ref cached, maxLength: 256);

                    _input = cached;
                    _resetInputFocus = true;
                }
                else
                {
                    ImGui.InputText("##log_console", ref _input, maxLength: 256);
                }

                if (Game.Input.Shortcut(Keys.Enter))
                {
                    LogCommand(_input);

                    if (onInputAction is not null)
                    {
                        string output = onInputAction.Invoke(_input);
                        LogCommandOutput(output);
                    }

                    _resetInputFocus = true;
                    _input = string.Empty;

                    _pressedBackCount = 0;
                }

                ImGui.PopItemWidth();

                ImGui.EndChild();
            }
        }
    }
}