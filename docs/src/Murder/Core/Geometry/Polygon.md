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
public Polygon(IEnumerable<T> vertices, Point position)
```

**Parameters** \
`vertices` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \
`position` [Point](/Murder/Core/Geometry/Point.html) \

```csharp
public Polygon(IEnumerable<T> vertices)
```

**Parameters** \
`vertices` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

### ⭐ Properties
#### Vertices
```csharp
public readonly ImmutableArray<T> Vertices;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### GetBoundingBox()
```csharp
public Rectangle GetBoundingBox()
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### Draw(Batch2D, Vector2, bool, Color)
```csharp
public void Draw(Batch2D batch, Vector2 position, bool flip, Color color)
```

**Parameters** \
`batch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`flip` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \



⚡