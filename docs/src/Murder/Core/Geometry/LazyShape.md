# LazyShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct LazyShape : IShape
```

**Implements:** _[IShape](../../../Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public LazyShape(float radius, Point offset)
```

**Parameters** \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`offset` [Point](../../../Murder/Core/Geometry/Point.html) \

### ⭐ Properties
#### Offset
```csharp
public readonly Point Offset;
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### Radius
```csharp
public readonly float Radius;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### SQUARE_ROOT_OF_TWO
```csharp
public static const float SQUARE_ROOT_OF_TWO;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### Rectangle(Vector2)
```csharp
public Rectangle Rectangle(Vector2 addPosition)
```

**Parameters** \
`addPosition` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \

#### GetPolygon()
```csharp
public virtual PolygonShape GetPolygon()
```

**Returns** \
[PolygonShape](../../../Murder/Core/Geometry/PolygonShape.html) \

#### GetBoundingBox()
```csharp
public virtual Rectangle GetBoundingBox()
```

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \



⚡