# TileGrid

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public class TileGrid
```

### ⭐ Constructors
```csharp
public TileGrid(Point origin, int width, int height)
```

**Parameters** \
`origin` [Point](/Murder/Core/Geometry/Point.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Height
```csharp
public int Height { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Origin
```csharp
public Point Origin { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Width
```csharp
public int Width { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### HasFlagAtGridPosition(int, int, int)
```csharp
public bool HasFlagAtGridPosition(int x, int y, int value)
```

Checks whether is solid at a position <paramref name="x" /> and <paramref name="y" />.
            This will take a position from the grid (world) back to the local grid, using [TileGrid.Origin](/murder/core/tilegrid.html#origin).

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### At(Point)
```csharp
public int At(Point p)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### At(int, int)
```csharp
public int At(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### AtGridPosition(Point)
```csharp
public int AtGridPosition(Point p)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### HasFlagAt(int, int, int)
```csharp
public virtual bool HasFlagAt(int x, int y, int value)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Resize(IntRectangle)
```csharp
public void Resize(IntRectangle rectangle)
```

This supports resize the grid up to:
              _____      ______
             |     | -&gt; |      |
             |_____x    |      |
                        |______x
            or
              _____         _____
             |  x  | -&gt;    |  x  |
             |_____|       |_____|
            
            Where x is the bullet point.

**Parameters** \
`rectangle` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
\

#### Resize(int, int, Point)
```csharp
public void Resize(int width, int height, Point origin)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`origin` [Point](/Murder/Core/Geometry/Point.html) \

#### Set(Point, int)
```csharp
public void Set(Point p, int value)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Set(int, int, int)
```csharp
public void Set(int x, int y, int value)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### SetGridPosition(IntRectangle, int)
```csharp
public void SetGridPosition(IntRectangle rect, int value)
```

**Parameters** \
`rect` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### SetGridPosition(Point, int)
```csharp
public void SetGridPosition(Point p, int value)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Unset(Point, int)
```csharp
public void Unset(Point p, int value)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Unset(int, int, int)
```csharp
public void Unset(int x, int y, int value)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### UnsetAll(int)
```csharp
public void UnsetAll(int value)
```

Unset all the tiles according to the bitness of <paramref name="value" />.

**Parameters** \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### UnsetGridPosition(IntRectangle, int)
```csharp
public void UnsetGridPosition(IntRectangle rect, int value)
```

**Parameters** \
`rect` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### UnsetGridPosition(Point, int)
```csharp
public void UnsetGridPosition(Point p, int value)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡