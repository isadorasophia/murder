# GameLogger

**Namespace:** Murder.Diagnostics \
**Assembly:** Murder.dll

```csharp
public class GameLogger
```

### ⭐ Constructors
```csharp
protected GameLogger()
```

This is a singleton.

### ⭐ Properties
#### _instance
```csharp
protected static GameLogger _instance;
```

**Returns** \
[GameLogger](/Murder/Diagnostics/GameLogger.html) \
#### _lastInputIndex
```csharp
protected int _lastInputIndex;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### _lastInputs
```csharp
protected readonly String[] _lastInputs;
```

These are for supporting ^ functionality in console. Fancyy....

**Returns** \
[string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### _log
```csharp
protected readonly List<T> _log;
```

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \
#### _resetInputFocus
```csharp
protected bool _resetInputFocus;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### _scrollToBottom
```csharp
protected int _scrollToBottom;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### _showDebug
```csharp
protected bool _showDebug;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### _traceCount
```csharp
protected static const int _traceCount;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Input(Func<T, TResult>)
```csharp
protected virtual void Input(Func<T, TResult> onInputAction)
```

Receive input from the user. Called when a console is displayed.

**Parameters** \
`onInputAction` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-2?view=net-7.0) \

#### LogText(bool)
```csharp
protected virtual void LogText(bool copy)
```

Log text in the console display. Called when a console is displayed.

**Parameters** \
`copy` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TopBar(Boolean&)
```csharp
protected virtual void TopBar(Boolean& copy)
```

**Parameters** \
`copy` [bool&](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ClearLog()
```csharp
protected void ClearLog()
```

#### LogCommand(string)
```csharp
protected void LogCommand(string msg)
```

**Parameters** \
`msg` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### LogCommandOutput(string)
```csharp
protected void LogCommandOutput(string msg)
```

**Parameters** \
`msg` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### CaptureCrash(Exception, string)
```csharp
public bool CaptureCrash(Exception ex, string logFile)
```

Used to filter exceptions once a crash is yet to happen.

**Parameters** \
`ex` [Exception](https://learn.microsoft.com/en-us/dotnet/api/System.Exception?view=net-7.0) \
`logFile` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetOrCreateInstance()
```csharp
public GameLogger GetOrCreateInstance()
```

**Returns** \
[GameLogger](/Murder/Diagnostics/GameLogger.html) \

#### FetchLogs()
```csharp
public ImmutableArray<T> FetchLogs()
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### DrawConsole(Func<T, TResult>)
```csharp
public virtual void DrawConsole(Func<T, TResult> onInputAction)
```

Draws the console of the game.

**Parameters** \
`onInputAction` [Func\<T, TResult\>](https://learn.microsoft.com/en-us/dotnet/api/System.Func-2?view=net-7.0) \

#### Error(string)
```csharp
public void Error(string msg)
```

**Parameters** \
`msg` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Fail(string)
```csharp
public void Fail(string message)
```

This will fail a given message and paste it in the log.

**Parameters** \
`message` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Initialize()
```csharp
public void Initialize()
```

#### Log(string, Vector4)
```csharp
public void Log(string v, Vector4 color)
```

**Parameters** \
`v` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`color` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \

#### Log(string, T?)
```csharp
public void Log(string v, T? color)
```

**Parameters** \
`v` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`color` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Toggle(bool)
```csharp
public void Toggle(bool value)
```

**Parameters** \
`value` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ToggleDebugWindow()
```csharp
public void ToggleDebugWindow()
```

#### Verify(bool, string)
```csharp
public void Verify(bool condition, string message)
```

This will verify a condition. If false, this will paste <paramref name="message" /> in the log.

**Parameters** \
`condition` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`message` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Verify(bool)
```csharp
public void Verify(bool condition)
```

This will verify a condition. If false, this will paste in the log.

**Parameters** \
`condition` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Warning(string)
```csharp
public void Warning(string msg)
```

**Parameters** \
`msg` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡