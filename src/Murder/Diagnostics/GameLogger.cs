using Microsoft.Xna.Framework.Input;
using Murder.Core.Graphics;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Murder.Diagnostics;

public class GameLogger
{
    protected const int _traceCount = 7;

    protected static GameLogger? _instance;

    protected bool _showDebug = false;
    protected readonly List<LogLine> _log = new();

    protected int _scrollToBottom = 1;
    protected bool _resetInputFocus = true;

    /// <summary>
    /// These are for supporting ^ functionality in console. Fancyy....
    /// </summary>
    protected readonly string[] _lastInputs = new string[1024];
    protected int _lastInputIndex = 0;

    private string _lastInput = string.Empty;
    public static bool IsShowing => _instance?._showDebug ?? false;

    /// <summary>
    /// This is a singleton.
    /// </summary>
    protected GameLogger() { }

    public void Initialize(bool diagnostic)
    {
        if (diagnostic)
        {
            // This enable us to watch for any first chance exception within our game.
            AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            {
                StringBuilder message = new();
                message.Append($"Exception was thrown! {e.Exception.Message}");

                // Ignore stacks for Newtonsoft.
                if (e.Exception.Source is not string source || !source.Contains("Newtonsoft"))
                {
                    message.Append($"\n{e.Exception.StackTrace}");
                }

                Warning(message.ToString());
            };
        }
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
    }

    /// <summary>
    /// Draws the console of the game.
    /// </summary>
    public virtual void DrawConsole(Func<string, string>? onInputAction = default) { }

    /// <summary>
    /// Shows the top bar of the console. Called when a console is displayed.
    /// </summary>
    protected virtual void TopBar(ref bool copy) { }

    /// <summary>
    /// Log text in the console display. Called when a console is displayed.
    /// </summary>
    protected virtual void LogText(bool copy) { }

    /// <summary>
    /// Receive input from the user. Called when a console is displayed.
    /// </summary>
    protected virtual void Input(Func<string, string>? onInputAction) { }
    public static void ClearAllGraphs() => Game.Instance.GraphLogger.ClearAllGraphs();
    public static void ClearGraph([CallerFilePath] string callerFilePath = "") => Game.Instance.GraphLogger.ClearGraph(callerFilePath);
    public static void PlotGraph(float point, [CallerFilePath] string callerFilePath = "") => Game.Instance.GraphLogger.PlotGraph(point, callerFilePath);
    public static void PlotGraph(float point, int max, [CallerFilePath] string callerFilePath = "") => Game.Instance.GraphLogger.PlotGraph(point, max, callerFilePath);
    public static void LogPerf(string v, Vector4? color = null) =>
        GetOrCreateInstance().LogPerfImpl(v, color ?? new Vector4(1, 1, 1, 1) /* white */);

    public static void LogDebug(string v)
    {
#if DEBUG
        GetOrCreateInstance().LogDebugImpl(v);
#endif
    }

    public static void Log(string v, Microsoft.Xna.Framework.Color? color = null)
        => GetOrCreateInstance().LogImpl(v, (color ?? Microsoft.Xna.Framework.Color.White).ToSysVector4());

    public static void Log(string v, Vector4 color) => GetOrCreateInstance().LogImpl(v, color);

    public static void Warning(string msg,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0) => GetOrCreateInstance().LogWarningImpl(msg, memberName, lineNumber);

    public static void Error(
        string msg, 
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0) => GetOrCreateInstance().LogErrorImpl(msg, memberName, lineNumber);

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
    /// This will verify a condition. If false, this will paste <paramref name="message"/> in the log.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming", "IL2026:Frame names might not be available.", Justification = "Optional message to debug.")]
    public static void Verify([DoesNotReturnIf(false)] bool condition, string message)
    {
#if DEBUG
        if (!condition)
        {
            if (Debugger.IsAttached)
            {
                // We only want to stop here if a debugger is actually attached.
                Debug.Fail(message);
            }
            else
            {
                Error(message);
            }
        }
#endif

        if (!condition)
        {
            StackTrace trace = new(true);

            int traceCount = _traceCount;
            StackFrame[] frames = trace.GetFrames();

            StringBuilder stack = new();
            for (int i = 1; i < frames.Length && i < _traceCount + 1; i++)
            {
                stack.AppendLine($"\t{frames[i].GetMethod()?.DeclaringType?.FullName ?? string.Empty}.{frames[i].GetMethod()?.Name ?? "Unknown"}:line {frames[i].GetFileLineNumber()}");

                if (traceCount-- <= 0)
                {
                    break;
                }
            }

            Warning($"Assert was fired: '{message}'. At:\n{stack}");
        }
    }

    private void LogPerfImpl(string rawMessage, Vector4 color)
    {
        if (CheckRepeat(rawMessage))
        {
            return;
        }

        string message = $"[PERF] 📈 {rawMessage}";
        Debug.WriteLine(message);

        OutputToLog($"\uf201 {rawMessage}", color);
        _scrollToBottom = 2;
    }

    private void LogDebugImpl(string rawMessage)
    {
        if (CheckRepeat(rawMessage))
            return;

        string message = $"[DEBUG] {rawMessage}";
        Debug.WriteLine(message);

        OutputToLog($"\uf188 {rawMessage}", new Vector4(1, 1, 1, 0.5f));
        _scrollToBottom = 2;
    }

    private void LogImpl(string rawMessage, Vector4 color)
    {
        if (CheckRepeat(rawMessage))
            return;

        string message = $"[LOG] {rawMessage}";
        Debug.WriteLine(message);

        OutputToLog(rawMessage, color);
        _scrollToBottom = 2;
    }

    private void LogWarningImpl(string rawMessage, string memberName, int lineNumber)
    {
        if (CheckRepeat(rawMessage))
        {
            return;
        }

        Debug.WriteLine($"[WRN] {rawMessage}");

        string outputMessage = rawMessage;
        if (!string.IsNullOrEmpty(memberName) && lineNumber != 0)
        {
            outputMessage = $"{memberName} (line {lineNumber}): {outputMessage}";
        }

        OutputToLog(outputMessage, new Vector4(1, 1, 0.5f, 1));
        _scrollToBottom = 2;
    }

    private void LogErrorImpl(string rawMessage, string memberName, int lineNumber)
    {
        if (CheckRepeat(rawMessage))
        {
            return;
        }

        Debug.WriteLine($"[ERR] {rawMessage}");

        string outputMessage = rawMessage;
        if (!string.IsNullOrEmpty(memberName) && lineNumber != 0)
        {
            outputMessage = $"{memberName} (line {lineNumber}): {outputMessage}";
        }

        OutputToLog(outputMessage, new Vector4(1, 0.25f, 0.5f, 1));
        _scrollToBottom = 2;
        _showDebug = true;
    }

    protected void LogCommand(string msg)
    {
        Debug.WriteLine($"[Input <<] {msg}");

        string message = $"> {msg}";
        OutputToLog(message, new Vector4(.5f, 1, .3f, 1));
        _scrollToBottom = 2;

        _lastInputs[_lastInputIndex++ % _lastInputs.Length] = msg;
    }

    protected void LogCommandOutput(string msg)
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
            _log.Add(new LogLine() { Message = line, Color = color, Repeats = 1 });
        }
    }

    private bool CheckRepeat(string rawMessage)
    {
        if (_lastInput == rawMessage && _log.Count > 0)
        {
            LogLine lastLog = _log.Last();
            _log[^1] = lastLog with { Repeats = lastLog.Repeats + 1 };
            return true;
        }

        _lastInput = rawMessage;
        return false;
    }

    protected void ClearLog() => _log.Clear();

    protected readonly struct LogLine
    {
        public string Message { init; get; }
        public Vector4 Color { init; get; }
        public int Repeats { init; get; }
    }

    /// <summary>
    /// Used to filter exceptions once a crash is yet to happen.
    /// </summary>
    public static bool CaptureCrash(string logFile = "crash.log")
    {
        string currentDirectory = Environment.CurrentDirectory;
        string logFilePath = Path.Join(currentDirectory, logFile);

        StringBuilder content = new(GetCurrentLog());

        File.WriteAllTextAsync(logFilePath, content.ToString());
        return false;
    }

    public static string GetCurrentLog()
    {
        StringBuilder content = new();
        foreach (string line in GameLogger.FetchLogs())
        {
            content.AppendLine(line);
        }

        return content.ToString();
    }
    public virtual void TrackImpl(string variableName, object value)
    {
        // Not implemented by game, only by editor.
    }
    public static void Track(string variableName, object value)
    {
        GameLogger._instance?.TrackImpl(variableName, value);
    }
}