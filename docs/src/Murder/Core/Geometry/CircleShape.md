# CircleShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct CircleShape : IShape
```

**Implements:** _[IShape](../../../Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public CircleShape(float radius, Point offset)
```

**Parameters** \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`offset` [Point](../../../Murder/Core/Geometry/Point.html) \

### ⭐ Properties
#### Circle
```csharp
public Circle Circle { get; }
```

**Returns** \
[Circle](../../../Murder/Core/Geometry/Circle.html) \
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
### ⭐ Methods
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