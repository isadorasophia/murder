# GameLogger

**Namespace:** Murder.Diagnostics \
**Assembly:** Murder.dll

```csharp
public class GameLogger
```

### ⭐ Methods
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
public void DrawConsole(Func<T, TResult> onInputAction)
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

This will verify a condition. If true, this will paste <paramref name="message" /> in the log.

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