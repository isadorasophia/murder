# PointShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct PointShape : IShape
```

**Implements:** _[IShape](../../../Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public PointShape(Point point)
```

**Parameters** \
`point` [Point](../../../Murder/Core/Geometry/Point.html) \

### ⭐ Properties
#### Point
```csharp
public readonly Point Point;
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
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