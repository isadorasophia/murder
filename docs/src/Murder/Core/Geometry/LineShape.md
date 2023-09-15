# LineShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct LineShape : IShape
```

**Implements:** _[IShape](../../../Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public LineShape(Point start, Point end)
```

**Parameters** \
`start` [Point](../../../Murder/Core/Geometry/Point.html) \
`end` [Point](../../../Murder/Core/Geometry/Point.html) \

### ⭐ Properties
#### End
```csharp
public readonly Point End;
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### Line
```csharp
public Line2 Line { get; }
```

**Returns** \
[Line2](../../../Murder/Core/Geometry/Line2.html) \
#### Start
```csharp
public readonly Point Start;
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
### ⭐ Methods
#### LineAtPosition(Point)
```csharp
public Line2 LineAtPosition(Point position)
```

**Parameters** \
`position` [Point](../../../Murder/Core/Geometry/Point.html) \

**Returns** \
[Line2](../../../Murder/Core/Geometry/Line2.html) \

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