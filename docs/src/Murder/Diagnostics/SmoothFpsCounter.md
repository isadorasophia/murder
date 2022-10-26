# SmoothFpsCounter

**Namespace:** Murder.Diagnostics \
**Assembly:** Murder.dll

```csharp
public class SmoothFpsCounter
```

This will smooth the average FPS of the game.

### ⭐ Constructors
```csharp
public SmoothFpsCounter(int size)
```

**Parameters** \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Value
```csharp
public double Value { get; }
```

Latest FPS value.

**Returns** \
[double](https://learn.microsoft.com/en-us/dotnet/api/System.Double?view=net-7.0) \
### ⭐ Methods
#### Update(double)
```csharp
public void Update(double dt)
```

**Parameters** \
`dt` [double](https://learn.microsoft.com/en-us/dotnet/api/System.Double?view=net-7.0) \



⚡