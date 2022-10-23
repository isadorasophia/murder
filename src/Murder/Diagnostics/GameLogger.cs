using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Core.Input;
using Murder.ImGuiExtended;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace Murder.Diagnostics
{
    public class GameLogger
    {
        private const int _traceCount = 7;

        private static GameLogger? _instance;

        private bool _showDebug = false;
        private readonly List<LogLine> _log = new();

        private int _scrollToBottom = 1;

        private string _input = string.Empty;
        private bool _resetInputFocus = true;

        /// <summary>
        /// These are for supporting ^ functionality in console. Fancyy....
        /// </summary>
        private readonly string[] _lastInputs = new string[1024];
        private int _lastInputIndex = 0;

        private int _pressedBackCount = 0;

        /// <summary>
        /// This is a singleton.
        /// </summary>
        private GameLogger() { }

        public void Initialize()
        {
#if DEBUG
            // This enable us to watch for any first chance exception within our game.
            AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            {
                Warning($"Exception was thrown!\n{e.Exception.Message}\n{e.Exception.StackTrace}.");
            };
#endif
        }

        public static GameLogger GetOrCreateInstance()
        {
            _instance ??= new GameLogger();
            return _instance;
        }

        public static ImmutableArray<string> FetchLogs() =>
            GetOrCreateInstance()._log.Select(l => l.Message).ToImmutableArray();

        public static void Toggle(bool value)
        {
            GameLogger instance = GetOrCreateInstance();
            if (value == instance._showDebug) return;

            instance.ToggleDebugWindow();
        }

        public void ToggleDebugWindow()
        {
            _showDebug = !_showDebug;

            if (_showDebug)
            {
                _scrollToBottom = 2;
                _resetInputFocus = true;
            }

            Game.Input.Lock(_showDebug);
        }

        /// <summary>
        /// Draws the console of the game.
        /// </summary>
        public void DrawConsole(Func<string, string>? onInputAction = default)
        {
            if (!_showDebug) return;

            ImGuiWindowFlags winStyle = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar;
            float width = ImGui.GetWindowViewport().Size.X;

            if (ImGui.Begin("Console", winStyle))
            {
                ImGui.SetWindowFocus();

                ImGui.SetWindowSize(new Vector2(width, -1));
                ImGui.SetWindowPos(new Vector2(0, 0));

                bool copy = false;
                TopBar(ref copy);

                ImGui.Separator();

                LogText(copy);

                ImGui.Separator();

                Input(onInputAction);

                ImGui.End();
            }

            foreach (var value in Enum.GetValues(typeof(InputButtons)))
            {
                Game.Input.Consume((InputButtons)value);
            }
        }

        private void TopBar(ref bool copy)
        {
            if (ImGui.BeginChild("top_console", new(-1, ImGui.GetFontSize() * 1.75f)))
            {
                if (ImGui.Button("Clear")) ClearLog();
                ImGui.SameLine();
                ImGuiHelpers.HelpTooltip("Clear all the log output!");

                copy = ImGui.Button("Copy");
                ImGui.SameLine();
                ImGuiHelpers.HelpTooltip("Copy the whole log!");

                ImGui.Dummy(new Vector2(20, 0) * Game.Instance.GameScale);
                ImGui.SameLine();

                if (ImGui.Button("Close"))
                {
                    Toggle(false);
                }

                ImGuiHelpers.HelpTooltip("Bye :-(");

                ImGui.EndChild();
            }
        }

        private void LogText(bool copy)
        {
            if (ImGui.BeginChild("scrolling_console", new(-1, ImGui.GetFontSize() * 20)))
            {
                if (copy) ImGui.LogToClipboard();

                ImGui.PushTextWrapPos();

                for (int i = 0; i < _log.Count; ++i)
                {
                    string msg = _log[i].Message;

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

        private void Input(Func<string, string>? onInputAction)
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

        public static void Log(string v, Microsoft.Xna.Framework.Color? color = null)
            => GetOrCreateInstance().LogImpl(v, (color ?? Microsoft.Xna.Framework.Color.White).ToSysVector4());

        public static void Log(string v, Vector4 color) => GetOrCreateInstance().LogImpl(v, color);

        public static void Warning(string msg) => GetOrCreateInstance().LogWarningImpl(msg);

        public static void Error(string msg) => GetOrCreateInstance().LogErrorImpl(msg);

        /// <summary>
        /// This will fail a given message and paste it in the log.
        /// </summary>
        public static void Fail(string message)
        {
            Verify(false, message);
        }

        /// <summary>
        /// This will verify a condition. If false, this will paste in the log.
        /// </summary>
        public static void Verify([DoesNotReturnIf(false)] bool condition)
        {
            Verify(condition, "An expected condition was not true.");
        }

        /// <summary>
        /// This will verify a condition. If true, this will paste <paramref name="message"/> in the log.
        /// </summary>
        public static void Verify([DoesNotReturnIf(false)] bool condition, string message)
        {
            Debug.Assert(condition, message);

            if (!condition)
            {
                StackTrace trace = new(true);

                int traceCount = _traceCount;
                StackFrame[] frames = trace.GetFrames();

                StringBuilder stack = new();
                for (int i = 1; i < frames.Length && i < _traceCount + 1; i++)
                {
                    stack.AppendLine($"\t{frames[i].GetMethod()?.DeclaringType?.FullName ?? string.Empty}{frames[i].GetMethod()?.Name ?? "Unknown"}:line {frames[i].GetFileLineNumber()}");

                    if (traceCount-- <= 0)
                    {
                        break;
                    }
                }

                Warning($"Assert was fired: '{message}'. At:\n{stack}");
            }
        }

        private void LogImpl(string rawMessage, Vector4 color)
        {
            var message = $"[LOG] {rawMessage}";
            Debug.WriteLine(message);

            OutputToLog(rawMessage, color);
            _scrollToBottom = 2;
        }

        private void LogWarningImpl(string rawMessage)
        {
            var message = $"[WRN] {rawMessage}";
            Debug.WriteLine(message);

            if (_log.LastOrDefault().Message == rawMessage)
            {
                // If warning has already been showed, skip showing it multiple times.
                return;
            }

            OutputToLog(rawMessage, new Vector4(1, 1, 0.5f, 1));
            _scrollToBottom = 2;
            _showDebug = true;
        }

        private void LogErrorImpl(string rawMessage)
        {
            var message = $"[ERR] {rawMessage}";
            Debug.WriteLine(message);

            OutputToLog(rawMessage, new Vector4(1, 0.25f, 0.5f, 1));
            _scrollToBottom = 2;
            _showDebug = true;
        }

        private void LogCommand(string msg)
        {
            Debug.WriteLine($"[Input <<] {msg}");

            string message = $"> {msg}";
            OutputToLog(message, new Vector4(.5f, 1, .3f, 1));
            _scrollToBottom = 2;

            _lastInputs[_lastInputIndex++ % _lastInputs.Length] = msg;
        }

        private void LogCommandOutput(string msg)
        {
            Debug.WriteLine($"[Output >>] {msg}");

            OutputToLog(msg, new Vector4(1, 1, 1, 1), includeTime: false);
            _scrollToBottom = 2;
        }

        private void OutputToLog(string message, Vector4 color, bool includeTime = true)
        {
            string time = includeTime ? $"[{DateTime.Now:HH:mm:ss}] " : string.Empty;
            
            using StringReader reader = new(time + message);
            for (string? line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                _log.Add(new LogLine() { Message = line, Color = color });
            }
        }

        private void ClearLog() => _log.Clear();

        private readonly struct LogLine
        {
            public string Message { init; get; }
            public Vector4 Color { init; get; }
        }
    }
}