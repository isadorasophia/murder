# GridHelper

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public static class GridHelper
```

### ⭐ Methods
#### Circle(int, int, int)
```csharp
public IEnumerable<T> Circle(int cx, int cy, int radius)
```

**Parameters** \
`cx` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`cy` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`radius` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### Line(Point, Point)
```csharp
public IEnumerable<T> Line(Point start, Point end)
```

**Parameters** \
`start` [Point](/Murder/Core/Geometry/Point.html) \
`end` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### Neighbours(Point, int, int, bool)
```csharp
public IEnumerable<T> Neighbours(Point p, int width, int height, bool includeDiagonals)
```

Returns all the neighbours of a position.

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`includeDiagonals` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### Neighbours(Point, int, int, int, int, bool)
```csharp
public IEnumerable<T> Neighbours(Point p, int x, int y, int edgeX, int edgeY, bool includeDiagonals)
```

Returns all the neighbours of a position.

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`edgeX` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`edgeY` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`includeDiagonals` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### Reverse(IDictionary<TKey, TValue>, Point, Point)
```csharp
public ImmutableDictionary<TKey, TValue> Reverse(IDictionary<TKey, TValue> input, Point initial, Point target)
```

**Parameters** \
`input` [IDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IDictionary-2?view=net-7.0) \
`initial` [Point](/Murder/Core/Geometry/Point.html) \
`target` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### GetBoundingBox(Rectangle)
```csharp
public IntRectangle GetBoundingBox(Rectangle rect)
```

**Parameters** \
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### mWARClampToGrid(int, int)
```csharp
public Point mWARClampToGrid(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### ToGrid(Vector2)
```csharp
public Point ToGrid(Vector2 position)
```

**Parameters** \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \



⚡