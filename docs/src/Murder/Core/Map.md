# Map

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public class Map
```

### ⭐ Constructors
```csharp
public Map(int width, int height)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Height
```csharp
public readonly int Height;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Width
```csharp
public readonly int Width;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### HasAnyCollision(int, int)
```csharp
public bool HasAnyCollision(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasCarveCollision(int, int)
```csharp
public bool HasCarveCollision(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasCollision(int, int, GridCollisionType)
```csharp
public bool HasCollision(int x, int y, GridCollisionType type)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`type` [GridCollisionType](/Murder/Core/GridCollisionType.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasCollision(int, int, int, int, GridCollisionType)
```csharp
public bool HasCollision(int x, int y, int width, int height, GridCollisionType type)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`type` [GridCollisionType](/Murder/Core/GridCollisionType.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasLineOfSight(Point, Point, bool, GridCollisionType)
```csharp
public bool HasLineOfSight(Point start, Point end, bool excludeEdges, GridCollisionType blocking)
```

A fast Line of Sight check
            It is not exact by any means, just tries to draw A line of tiles between start and end.

**Parameters** \
`start` [Point](/Murder/Core/Geometry/Point.html) \
\
`end` [Point](/Murder/Core/Geometry/Point.html) \
\
`excludeEdges` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`blocking` [GridCollisionType](/Murder/Core/GridCollisionType.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### HasStaticCollision(int, int)
```csharp
public bool HasStaticCollision(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsInsideGrid(int, int)
```csharp
public bool IsInsideGrid(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsObstacle(Point)
```csharp
public bool IsObstacle(Point p)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetCollision(int, int)
```csharp
public GridCollisionType GetCollision(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[GridCollisionType](/Murder/Core/GridCollisionType.html) \

#### GetStaticCollisions(IntRectangle)
```csharp
public IEnumerable<T> GetStaticCollisions(IntRectangle rect)
```

**Parameters** \
`rect` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### GetVisionCollisions(IntRectangle)
```csharp
public IEnumerable<T> GetVisionCollisions(IntRectangle rect)
```

**Parameters** \
`rect` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### WeightAt(int, int)
```csharp
public int WeightAt(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### GetGridMap(int, int)
```csharp
public MapTile GetGridMap(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[MapTile](/Murder/Core/MapTile.html) \

#### HasCollisionAt(IntRectangle, GridCollisionType)
```csharp
public T? HasCollisionAt(IntRectangle rect, GridCollisionType type)
```

**Parameters** \
`rect` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
`type` [GridCollisionType](/Murder/Core/GridCollisionType.html) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### HasCollisionAt(int, int, int, int, GridCollisionType)
```csharp
public T? HasCollisionAt(int x, int y, int width, int height, GridCollisionType type)
```

Check for collision using tiles coordinates.

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`type` [GridCollisionType](/Murder/Core/GridCollisionType.html) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### HasStaticCollisionAt(int, int, int, int)
```csharp
public T? HasStaticCollisionAt(int x, int y, int width, int height)
```

Check for collision using tiles coordinates

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
\

#### Explore(int, int)
```csharp
public void Explore(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### SetOccupiedAsCarve(IntRectangle, bool, bool, int)
```csharp
public void SetOccupiedAsCarve(IntRectangle rect, bool blockVision, bool isObstacle, int weight)
```

**Parameters** \
`rect` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
`blockVision` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`isObstacle` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`weight` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### SetOccupiedAsStatic(int, int)
```csharp
public void SetOccupiedAsStatic(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### SetUnoccupiedCarve(IntRectangle, bool, bool, int)
```csharp
public void SetUnoccupiedCarve(IntRectangle rect, bool blockVision, bool isObstacle, int weight)
```

**Parameters** \
`rect` [IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
`blockVision` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`isObstacle` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`weight` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡