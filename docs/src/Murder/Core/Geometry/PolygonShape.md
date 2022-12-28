# PolygonShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct PolygonShape : IShape
```

**Implements:** _[IShape](/Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public PolygonShape()
```

### ⭐ Properties
#### Polygon
```csharp
public readonly Polygon Polygon;
```

**Returns** \
[Polygon](/Murder/Core/Geometry/Polygon.html) \
#### Rect
```csharp
public Rectangle Rect { get; }
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
### ⭐ Methods
#### GetCenter()
```csharp
public Point GetCenter()
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### GetBoundingBox()
```csharp
public virtual Rectangle GetBoundingBox()
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### Cache()
```csharp
public void Cache()
```



⚡