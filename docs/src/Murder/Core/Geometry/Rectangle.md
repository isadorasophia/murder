# Rectangle

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Rectangle : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public Rectangle(Point p, int width, int height)
```

**Parameters** \
`p` [Point](../..//Murder/Core/Geometry/Point.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

```csharp
public Rectangle(Point position, Point size)
```

**Parameters** \
`position` [Point](../..//Murder/Core/Geometry/Point.html) \
`size` [Point](../..//Murder/Core/Geometry/Point.html) \

```csharp
public Rectangle(Vector2 position, Vector2 size)
```

**Parameters** \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`size` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

```csharp
public Rectangle(float x, float y, float width, float height)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`width` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`height` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public Rectangle(int x, int y, int width, int height)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Bottom
```csharp
public float Bottom { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### BottomCenter
```csharp
public Vector2 BottomCenter { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### BottomLeft
```csharp
public Vector2 BottomLeft { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### BottomRight
```csharp
public Vector2 BottomRight { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Center
```csharp
public Vector2 Center { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### CenterLeft
```csharp
public Vector2 CenterLeft { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### CenterPoint
```csharp
public Point CenterPoint { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### Empty
```csharp
public static Rectangle Empty { get; }
```

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \
#### Height
```csharp
public float Height;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### HeightRound
```csharp
public int HeightRound { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### IsEmpty
```csharp
public bool IsEmpty { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Left
```csharp
public float Left { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### One
```csharp
public static Rectangle One { get; }
```

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \
#### Right
```csharp
public float Right { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Size
```csharp
public Vector2 Size { get; public set; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Top
```csharp
public float Top { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### TopCenter
```csharp
public Vector2 TopCenter { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### TopLeft
```csharp
public Vector2 TopLeft { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### TopRight
```csharp
public Vector2 TopRight { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Width
```csharp
public float Width;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### WidthRound
```csharp
public int WidthRound { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### X
```csharp
public float X;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### XRound
```csharp
public int XRound { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Y
```csharp
public float Y;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### YRound
```csharp
public int YRound { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Contains(Point)
```csharp
public bool Contains(Point point)
```

**Parameters** \
`point` [Point](../..//Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Contains(Vector2)
```csharp
public bool Contains(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Contains(float, float)
```csharp
public bool Contains(float X, float Y)
```

**Parameters** \
`X` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`Y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Contains(int, int)
```csharp
public bool Contains(int X, int Y)
```

**Parameters** \
`X` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`Y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Touches(Rectangle)
```csharp
public bool Touches(Rectangle other)
```

Gets whether or not the other [Rectangle](../../..//Murder/Core/Geometry/Rectangle.html) intersects with this rectangle.

**Parameters** \
`other` [Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### TouchesWithMaxRotationCheck(Vector2, Vector2, Vector2)
```csharp
public bool TouchesWithMaxRotationCheck(Vector2 position, Vector2 size, Vector2 offset)
```

Whether an object within bounds intersects with this rectangle.
            This takes into account the "maximum" height and length given any rotation.

**Parameters** \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`size` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`offset` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### AddPadding(float, float, float, float)
```csharp
public Rectangle AddPadding(float left, float top, float right, float bottom)
```

**Parameters** \
`left` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`top` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`right` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`bottom` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### AddPosition(Point)
```csharp
public Rectangle AddPosition(Point position)
```

**Parameters** \
`position` [Point](../..//Murder/Core/Geometry/Point.html) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### AddPosition(Vector2)
```csharp
public Rectangle AddPosition(Vector2 position)
```

**Parameters** \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### CenterRectangle(Point, int, int)
```csharp
public Rectangle CenterRectangle(Point center, int width, int height)
```

**Parameters** \
`center` [Point](../..//Murder/Core/Geometry/Point.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### CenterRectangle(Vector2, float, float)
```csharp
public Rectangle CenterRectangle(Vector2 center, float width, float height)
```

**Parameters** \
`center` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`width` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`height` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### CenterRectangle(Vector2)
```csharp
public Rectangle CenterRectangle(Vector2 size)
```

**Parameters** \
`size` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### CenterRectangle(float, float)
```csharp
public Rectangle CenterRectangle(float x, float y)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### Expand(float)
```csharp
public Rectangle Expand(float value)
```

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### Expand(int)
```csharp
public Rectangle Expand(int value)
```

**Parameters** \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### GetIntersection(Rectangle, Rectangle)
```csharp
public Rectangle GetIntersection(Rectangle a, Rectangle b)
```

**Parameters** \
`a` [Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \
`b` [Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### Lerp(Rectangle, Rectangle, float)
```csharp
public Rectangle Lerp(Rectangle a, Rectangle b, float v)
```

**Parameters** \
`a` [Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \
`b` [Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \
`v` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### SetPosition(Vector2)
```csharp
public Rectangle SetPosition(Vector2 position)
```

**Parameters** \
`position` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### Equals(Rectangle)
```csharp
public virtual bool Equals(Rectangle other)
```

**Parameters** \
`other` [Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(Object)
```csharp
public virtual bool Equals(Object obj)
```

**Parameters** \
`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetHashCode()
```csharp
public virtual int GetHashCode()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡