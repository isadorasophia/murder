# Grid

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public static class Grid
```

### ⭐ Properties
#### CellDimensions
```csharp
public readonly static Point CellDimensions;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### CellSize
```csharp
public static const int CellSize;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### HalfCell
```csharp
public static const int HalfCell;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### HalfCellDimensions
```csharp
public readonly static Point HalfCellDimensions;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
### ⭐ Methods
#### HasFlag(int, int)
```csharp
public bool HasFlag(int value, int mask)
```

**Parameters** \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`mask` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CeilToGrid(float)
```csharp
public int CeilToGrid(float value)
```

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### FloorToGrid(float)
```csharp
public int FloorToGrid(float value)
```

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### RoundToGrid(float)
```csharp
public int RoundToGrid(float value)
```

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ToMask(int)
```csharp
public int ToMask(int value)
```

**Parameters** \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡