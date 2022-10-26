# Camera2D

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class Camera2D
```

Creates a camera 2D world view for our game.

### ⭐ Constructors
```csharp
public Camera2D(int width, int height, int scale)
```

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`scale` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Aspect
```csharp
public float Aspect { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Bounds
```csharp
public Rectangle Bounds { get; private set; }
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
#### HalfWidth
```csharp
public int HalfWidth { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Height
```csharp
public int Height { get; private set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Position
```csharp
public Vector2 Position { get; public set; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
#### SafeBounds
```csharp
public Rectangle SafeBounds { get; private set; }
```

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \
#### Size
```csharp
public Point Size { get; }
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### Width
```csharp
public int Width { get; private set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### WorldViewProjection
```csharp
public Matrix WorldViewProjection { get; }
```

**Returns** \
[Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
#### Zoom
```csharp
public float Zoom { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### IsInCamera(Point)
```csharp
public bool IsInCamera(Point rectangle)
```

**Parameters** \
`rectangle` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsInCamera(Rectangle)
```csharp
public bool IsInCamera(Rectangle rectangle)
```

**Parameters** \
`rectangle` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetCursorWorldPosition(Point, Point)
```csharp
public Point GetCursorWorldPosition(Point screenOffset, Point viewportSize)
```

Get coordinates of the cursor in the world.

**Parameters** \
`screenOffset` [Point](/Murder/Core/Geometry/Point.html) \
`viewportSize` [Point](/Murder/Core/Geometry/Point.html) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### ScreenToWorldPosition(Vector2)
```csharp
public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
```

**Parameters** \
`screenPosition` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### WorldToScreenPosition(Vector2)
```csharp
public Vector2 WorldToScreenPosition(Vector2 screenPosition)
```

**Parameters** \
`screenPosition` [Vector2](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### Rotate(float)
```csharp
public void Rotate(float degrees)
```

**Parameters** \
`degrees` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡