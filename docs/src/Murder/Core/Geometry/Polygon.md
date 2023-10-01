# Polygon

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Polygon
```

### ⭐ Constructors
```csharp
public Polygon()
```

```csharp
public Polygon(IEnumerable<T> vertices, Vector2 position)
```

**Parameters** \
`vertices` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

```csharp
public Polygon(IEnumerable<T> vertices)
```

**Parameters** \
`vertices` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

### ⭐ Properties
#### DIAMOND
```csharp
public readonly static Polygon DIAMOND;
```

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \
#### EMPTY
```csharp
public readonly static Polygon EMPTY;
```

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \
#### Vertices
```csharp
public readonly ImmutableArray<T> Vertices;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### Contains(Point)
```csharp
public bool Contains(Point point)
```

**Parameters** \
`point` [Point](../../../Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Contains(Vector2)
```csharp
public bool Contains(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsClockwise(List<T>)
```csharp
public bool IsClockwise(List<T> vertices)
```

**Parameters** \
`vertices` [List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsConvex()
```csharp
public bool IsConvex()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsPointInTriangle(Vector2, Vector2, Vector2, Vector2)
```csharp
public bool IsPointInTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
```

**Parameters** \
`point` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`a` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`b` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`c` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryMerge(Polygon, Polygon, float, out Polygon&)
```csharp
public bool TryMerge(Polygon a, Polygon b, float minDistance, Polygon& result)
```

**Parameters** \
`a` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \
`b` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \
`minDistance` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`result` [Polygon&](../../../Murder/Core/Geometry/Polygon.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetNormals()
```csharp
public IEnumerable<T> GetNormals()
```

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### GetLines()
```csharp
public Line2[] GetLines()
```

**Returns** \
[Line2[]](../../../Murder/Core/Geometry/Line2.html) \

#### EarClippingTriangulation(Polygon)
```csharp
public List<T> EarClippingTriangulation(Polygon polygon)
```

**Parameters** \
`polygon` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### FindConcaveVertices(ImmutableArray<T>)
```csharp
public List<T> FindConcaveVertices(ImmutableArray<T> points)
```

**Parameters** \
`points` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### PartitionToConvex(Polygon)
```csharp
public List<T> PartitionToConvex(Polygon concave)
```

This doesn't work yet

**Parameters** \
`concave` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### ReorderVertices(List<T>)
```csharp
public List<T> ReorderVertices(List<T> vertices)
```

**Parameters** \
`vertices` [List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \

#### AddPosition(Vector2)
```csharp
public Polygon AddPosition(Vector2 add)
```

**Parameters** \
`add` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \

#### AtPosition(Point)
```csharp
public Polygon AtPosition(Point target)
```

Returns this polygon with a new position. The position is calculated using the vertice 0 as origin.

**Parameters** \
`target` [Point](../../../Murder/Core/Geometry/Point.html) \
\

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \
\

**Exceptions** \
[NotImplementedException](https://learn.microsoft.com/en-us/dotnet/api/System.NotImplementedException?view=net-7.0) \
\
#### FromRectangle(int, int, int, int)
```csharp
public Polygon FromRectangle(int x, int y, int width, int height)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \

#### RemoveVerticeAt(int)
```csharp
public Polygon RemoveVerticeAt(int index)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \

#### WithNewVerticeAt(int, Vector2)
```csharp
public Polygon WithNewVerticeAt(int index, Vector2 target)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`target` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \

#### WithVerticeAt(int, Vector2)
```csharp
public Polygon WithVerticeAt(int index, Vector2 target)
```

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`target` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \

#### GetBoundingBox()
```csharp
public Rectangle GetBoundingBox()
```

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \

#### Intersects(Polygon, Vector2, Vector2)
```csharp
public T? Intersects(Polygon other, Vector2 positionA, Vector2 positionB)
```

Check if a polygon is inside another, if they do, return the minimum translation vector to move the polygon out of the other.

**Parameters** \
`other` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \
\
`positionA` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`positionB` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
\

#### MergePolygons(Polygon, Polygon)
```csharp
public T? MergePolygons(Polygon a, Polygon b)
```

**Parameters** \
`a` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \
`b` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### ProjectOntoAxis(Vector2, Vector2)
```csharp
public ValueTuple<T1, T2> ProjectOntoAxis(Vector2 axis, Vector2 offset)
```

**Parameters** \
`axis` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`offset` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

#### Draw(Batch2D, Vector2, bool, Color)
```csharp
public void Draw(Batch2D batch, Vector2 position, bool flip, Color color)
```

**Parameters** \
`batch` [Batch2D](../../../Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`flip` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`color` [Color](../../../Murder/Core/Graphics/Color.html) \



⚡