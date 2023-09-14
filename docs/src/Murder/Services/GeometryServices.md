# GeometryServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class GeometryServices
```

### ⭐ Methods
#### CheckOverlap(float, float, float, float)
```csharp
public bool CheckOverlap(float minA, float maxA, float minB, float maxB)
```

Check if two ranges overlap at any point.

**Parameters** \
`minA` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`maxA` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`minB` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`maxB` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CheckOverlap(ValueTuple<T1, T2>, ValueTuple<T1, T2>)
```csharp
public bool CheckOverlap(ValueTuple<T1, T2> a, ValueTuple<T1, T2> b)
```

Check if two ranges overlap at any point.

**Parameters** \
`a` [ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \
`b` [ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### InRect(Vector2, Rectangle)
```csharp
public bool InRect(Vector2 xy, Rectangle rect)
```

Check for a point in a rectangle.

**Parameters** \
`xy` [Vector2](../../Murder/Core/Geometry/Vector2.html) \
\
`rect` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### InRect(float, float, Rectangle)
```csharp
public bool InRect(float x, float y, Rectangle rect)
```

Check for a point in a rectangle.

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rect` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### InRect(float, float, float, float, float, float)
```csharp
public bool InRect(float x, float y, float rx, float ry, float rw, float rh)
```

Check for a point in a rectangle.

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`ry` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rw` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rh` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### IntersectsCircle(Rectangle, Vector2, float)
```csharp
public bool IntersectsCircle(Rectangle rectangle, Vector2 circleCenter, float circleRadiusSquared)
```

**Parameters** \
`rectangle` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`circleCenter` [Vector2](../../Murder/Core/Geometry/Vector2.html) \
`circleRadiusSquared` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsConvex(Vector2[], bool)
```csharp
public bool IsConvex(Vector2[] vertices, bool isClockwise)
```

Determines if a polygon is convex or not.

**Parameters** \
`vertices` [Vector2[]](../../Murder/Core/Geometry/Vector2.html) \
\
`isClockwise` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### IsValidPosition(IntRectangle[], Vector2, Point, Point)
```csharp
public bool IsValidPosition(IntRectangle[] area, Vector2 startPosition, Point endPosition, Point size)
```

Checks whether <paramref name="startPosition" />, with <paramref name="size" />, 
            fits in <paramref name="area" /> given an <paramref name="endPosition" />.

**Parameters** \
`area` [IntRectangle[]](../../Murder/Core/Geometry/IntRectangle.html) \
`startPosition` [Vector2](../../Murder/Core/Geometry/Vector2.html) \
`endPosition` [Point](../../Murder/Core/Geometry/Point.html) \
`size` [Point](../../Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Decimals(float)
```csharp
public float Decimals(float x)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Distance(float, float, float, float)
```csharp
public float Distance(float x1, float y1, float x2, float y2)
```

Distance check.

**Parameters** \
`x1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`x2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### DistanceLinePoint(float, float, float, float, float, float)
```csharp
public float DistanceLinePoint(float x, float y, float x1, float y1, float x2, float y2)
```

Distance between a line and a point.

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`x1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`x2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### DistanceRectPoint(float, float, float, float, float, float)
```csharp
public float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh)
```

Find the distance between a point and a rectangle.

**Parameters** \
`px` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`py` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`ry` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rw` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rh` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### RoundedDecimals(float)
```csharp
public float RoundedDecimals(float x)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### SignedPolygonArea(Vector2[])
```csharp
public float SignedPolygonArea(Vector2[] vertices)
```

Calculates the signed area of a polygon.
            The signed area is positive if the vertices are in clockwise order,
            and negative if the vertices are in counterclockwise order.

**Parameters** \
`vertices` [Vector2[]](../../Murder/Core/Geometry/Vector2.html) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### GetOuterIntersection(Rectangle, Rectangle)
```csharp
public IList<T> GetOuterIntersection(Rectangle a, Rectangle b)
```

Returns the area of <paramref name="b" /> that does not interlap with <paramref name="a" />.

**Parameters** \
`a` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`b` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \

#### Shrink(Rectangle, int)
```csharp
public Rectangle Shrink(Rectangle rectangle, int amount)
```

**Parameters** \
`rectangle` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`amount` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Rectangle](../../Murder/Core/Geometry/Rectangle.html) \

#### PointInCircleEdge(float)
```csharp
public Vector2 PointInCircleEdge(float percent)
```

**Parameters** \
`percent` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](../../Murder/Core/Geometry/Vector2.html) \

#### CreateCircle(double, int)
```csharp
public Vector2[] CreateCircle(double radius, int sides)
```

Creates a list of vectors that represents a circle

**Parameters** \
`radius` [double](https://learn.microsoft.com/en-us/dotnet/api/System.Double?view=net-7.0) \
\
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

**Returns** \
[Vector2[]](../../Murder/Core/Geometry/Vector2.html) \
\

#### CreateOrGetFlatenedCircle(float, float, int)
```csharp
public Vector2[] CreateOrGetFlatenedCircle(float radius, float scaleY, int sides)
```

**Parameters** \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`scaleY` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Vector2[]](../../Murder/Core/Geometry/Vector2.html) \



⚡