# BoxShape

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct BoxShape : IShape
```

**Implements:** _[IShape](../../../Murder/Core/Geometry/IShape.html)_

### ⭐ Constructors
```csharp
public BoxShape()
```

```csharp
public BoxShape(Vector2 origin, Point offset, int width, int height)
```

**Parameters** \
`origin` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`offset` [Point](../../../Murder/Core/Geometry/Point.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Height
```csharp
public readonly int Height;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Offset
```csharp
public readonly Point Offset;
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### Origin
```csharp
public readonly Vector2 Origin;
```

**Returns** \
[Vector2](../../../Murder/Core/Geometry/Vector2.html) \
#### Rectangle
```csharp
public Rectangle Rectangle { get; }
```

Simple shape getter

**Returns** \
[Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
#### Size
```csharp
public Point Size { get; }
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### Width
```csharp
public readonly int Width;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### ResizeBottomRight(Vector2)
```csharp
public BoxShape ResizeBottomRight(Vector2 newBottomRight)
```

**Parameters** \
`newBottomRight` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \

**Returns** \
[BoxShape](../../../Murder/Core/Geometry/BoxShape.html) \

#### ResizeTopLeft(Vector2)
```csharp
public BoxShape ResizeTopLeft(Vector2 newTopLeft)
```

**Parameters** \
`newTopLeft` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \

**Returns** \
[BoxShape](../../../Murder/Core/Geometry/BoxShape.html) \

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