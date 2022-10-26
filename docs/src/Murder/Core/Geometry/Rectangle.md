# Rectangle

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Rectangle : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public Rectangle(Point position, Point size)
```

**Parameters** \
`position` [Point](/Murder/Core/Geometry/Point.html) \
`size` [Point](/Murder/Core/Geometry/Point.html) \

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
#### BottomLeft
```csharp
public Vector2 BottomLeft { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### BottomRight
```csharp
public Vector2 BottomRight { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### Center
```csharp
public Vector2 Center { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### CenterPoint
```csharp
public Vector2 CenterPoint { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### Empty
```csharp
public static Rectangle Empty { get; }
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
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
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
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
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### Top
```csharp
public float Top { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### TopLeft
```csharp
public Vector2 TopLeft { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### TopRight
```csharp
public Vector2 TopRight { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
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
public Rectangle AddPosition(Point position)
```

**Parameters** \
`position` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### AddPosition(Vector2)
```csharp
public Rectangle AddPosition(Vector2 position)
```

**Parameters** \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### Expand(int)
```csharp
public Rectangle Expand(int value)
```

**Parameters** \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### Equals(Rectangle)
```csharp
public virtual bool Equals(Rectangle other)
```

**Parameters** \
`other` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡