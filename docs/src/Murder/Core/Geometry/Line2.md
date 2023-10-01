# Line2

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Line2
```

Class for a simple line with two points.
            This is based on a Otter2d class: https://github.com/kylepulver/Otter/blob/master/Otter/Utility/Line2.cs

### ⭐ Constructors
```csharp
public Line2(float x1, float y1, float x2, float y2)
```

Create a new Line2.

**Parameters** \
`x1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`x2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

```csharp
public Line2(Vector2 start, Vector2 end)
```

Create a new Line2.

**Parameters** \
`start` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`end` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\

### ⭐ Properties
#### Bottom
```csharp
public float Bottom { get; }
```

The bottom most Y position of the line.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### End
```csharp
public Vector2 End { get; }
```

The second point of a line as a vector2.

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Height
```csharp
public float Height { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Left
```csharp
public float Left { get; }
```

The left most X position of the line.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Right
```csharp
public float Right { get; }
```

The right most X position of the line.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Start
```csharp
public Vector2 Start { get; }
```

The first point of the line as a vector2.

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Top
```csharp
public float Top { get; }
```

The top most Y position of the line.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Width
```csharp
public float Width { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### X1
```csharp
public readonly float X1;
```

The X position for the first point.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### X2
```csharp
public readonly float X2;
```

The X position for the second point.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Y1
```csharp
public readonly float Y1;
```

The Y position for the first point.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Y2
```csharp
public readonly float Y2;
```

The Y position for the second point.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### GetClosestPoint(Vector2, float, out Vector2&)
```csharp
public bool GetClosestPoint(Vector2 point, float maxRange, Vector2& closest)
```

**Parameters** \
`point` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`maxRange` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`closest` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Intersects(Line2)
```csharp
public bool Intersects(Line2 other)
```

Intersection test on another line. (http://ideone.com/PnPJgb)

**Parameters** \
`other` [Line2](../../../Murder/Core/Geometry/Line2.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### IntersectsCircle(Circle)
```csharp
public bool IntersectsCircle(Circle circle)
```

Check the intersection against a circle.

**Parameters** \
`circle` [Circle](../../../Murder/Core/Geometry/Circle.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IntersectsRect(Rectangle)
```csharp
public bool IntersectsRect(Rectangle rect)
```

**Parameters** \
`rect` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IntersectsRect(float, float, float, float)
```csharp
public bool IntersectsRect(float x, float y, float width, float height)
```

Check intersection against a rectangle.

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`width` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`height` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### TryGetIntersectingPoint(Circle, out Vector2&)
```csharp
public bool TryGetIntersectingPoint(Circle circle, Vector2& hitPoint)
```

**Parameters** \
`circle` [Circle](../../../Murder/Core/Geometry/Circle.html) \
`hitPoint` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetIntersectingPoint(Line2, Line2, out Vector2&)
```csharp
public bool TryGetIntersectingPoint(Line2 line1, Line2 line2, Vector2& hitPoint)
```

**Parameters** \
`line1` [Line2](../../../Murder/Core/Geometry/Line2.html) \
`line2` [Line2](../../../Murder/Core/Geometry/Line2.html) \
`hitPoint` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetIntersectingPoint(Line2, out Vector2&)
```csharp
public bool TryGetIntersectingPoint(Line2 other, Vector2& hitPoint)
```

**Parameters** \
`other` [Line2](../../../Murder/Core/Geometry/Line2.html) \
`hitPoint` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetIntersectingPoint(Rectangle, out Vector2&)
```csharp
public bool TryGetIntersectingPoint(Rectangle rect, Vector2& hitPoint)
```

**Parameters** \
`rect` [Rectangle](../../../Murder/Core/Geometry/Rectangle.html) \
`hitPoint` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetIntersectingPoint(float, float, float, float, out Vector2&)
```csharp
public bool TryGetIntersectingPoint(float x, float y, float width, float height, Vector2& hitPoint)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`width` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`height` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`hitPoint` [Vector2&](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Length()
```csharp
public float Length()
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### LengthSquared()
```csharp
public float LengthSquared()
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡