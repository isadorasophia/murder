# PolygonShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct PolygonShape : IShape
```

**Implements:** _[IShape](../../../Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public PolygonShape()
```

```csharp
public PolygonShape(Polygon polygon)
```

**Parameters** \
`polygon` [Polygon](../../../Murder/Core/Geometry/Polygon.html) \

### ⭐ Properties
#### Polygon
```csharp
public readonly Polygon Polygon;
```

**Returns** \
[Polygon](../../../Murder/Core/Geometry/Polygon.html) \
#### Rect
```csharp
public Rectangle Rect { get; }
```

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
### ⭐ Methods
#### GetCenter()
```csharp
public Point GetCenter()
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \

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

#### Cache()
```csharp
public void Cache()
```



⚡