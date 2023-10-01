# SmoothCounter

**Namespace:** Bang.Diagnostics \
**Assembly:** Bang.dll

```csharp
public class SmoothCounter
```

Class used to smooth the counter of performance ticks.

### ⭐ Constructors
```csharp
public SmoothCounter(int size)
```

Creates a new [SmoothCounter](../../Bang/Diagnostics/SmoothCounter.html).

**Parameters** \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

### ⭐ Properties
#### AverageEntities
```csharp
public int AverageEntities { get; }
```

Average of entities over the sample size.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### AverageTime
```csharp
public int AverageTime { get; }
```

Average of counter time value over the sample size.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### MaximumTime
```csharp
public double MaximumTime { get; }
```

Maximum value over the sample size.

**Returns** \
[double](https://learn.microsoft.com/en-us/dotnet/api/System.Double?view=net-7.0) \
### ⭐ Methods
#### Clear()
```csharp
public void Clear()
```

Clear the counter track.

#### Update(double, int)
```csharp
public void Update(double ms, int totalEntities)
```

Update the smooth counter for the FPS report.

**Parameters** \
`ms` [double](https://learn.microsoft.com/en-us/dotnet/api/System.Double?view=net-7.0) \
\
`totalEntities` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\



⚡