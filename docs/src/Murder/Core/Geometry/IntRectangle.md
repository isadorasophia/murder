# IntRectangle

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct IntRectangle : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public IntRectangle(Point position, Point size)
```

**Parameters** \
`position` [Point](/Murder/Core/Geometry/Point.html) \
`size` [Point](/Murder/Core/Geometry/Point.html) \

```csharp
public IntRectangle(float x, float y, float width, float height)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`width` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`height` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public IntRectangle(int x, int y, int width, int height)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Bottom
```csharp
public int Bottom { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### BottomLeft
```csharp
public Point BottomLeft { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### BottomRight
```csharp
public Point BottomRight { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Center
```csharp
public Vector2 Center { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### CenterPoint
```csharp
public Point CenterPoint { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Empty
```csharp
public static IntRectangle Empty { get; }
```

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
#### Height
```csharp
public int Height;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Left
```csharp
public int Left { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### One
```csharp
public static IntRectangle One { get; }
```

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \
#### Right
```csharp
public int Right { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Size
```csharp
public Point Size { get; public set; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Top
```csharp
public int Top { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### TopLeft
```csharp
public Point TopLeft { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### TopRight
```csharp
public Point TopRight { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Width
```csharp
public int Width;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### X
```csharp
public int X;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Y
```csharp
public int Y;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Contains(Point)
```csharp
public bool Contains(Point point)
```

**Parameters** \
`point` [Point](/Murder/Core/Geometry/Point.html) \

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

Gets whether or not the other [Rectangle](/Murder/Core/Geometry/Rectangle.html) intersects with this rectangle.

**Parameters** \
`other` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
true if other  intersects with this rectangle; false otherwise.\

#### AddPosition(Point)
```csharp
public IntRectangle AddPosition(Point position)
```

**Parameters** \
`position` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### AddPosition(Vector2)
```csharp
public IntRectangle AddPosition(Vector2 position)
```

**Parameters** \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### Expand(float)
```csharp
public IntRectangle Expand(float value)
```

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### Expand(int)
```csharp
public IntRectangle Expand(int value)
```

**Parameters** \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[IntRectangle](/Murder/Core/Geometry/IntRectangle.html) \

#### Equals(Rectangle)
```csharp
public virtual bool Equals(Rectangle other)
```

**Parameters** \
`other` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡