# Calculator

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public static class Calculator
```

Calculator helper class.

### ⭐ Properties
#### LayersCount
```csharp
public static int LayersCount;
```

Default layers count.
            TODO: Make this customizable.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### InRect(Vector2, Rectangle)
```csharp
public bool InRect(Vector2 xy, Rectangle rect)
```

Check for a point in a rectangle.

**Parameters** \
`xy` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
True if the point is in the rectangle.\

#### InRect(float, float, Rectangle)
```csharp
public bool InRect(float x, float y, Rectangle rect)
```

Check for a point in a rectangle.

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
True if the point is in the rectangle.\

#### InRect(float, float, float, float, float, float)
```csharp
public bool InRect(float x, float y, float rx, float ry, float rw, float rh)
```

Check for a point in a rectangle.

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`ry` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rw` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rh` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
True if the point is in the rectangle.\

#### IntersectsCircle(Rectangle, Vector2, float)
```csharp
public bool IntersectsCircle(Rectangle rectangle, Vector2 circleCenter, float circleRadiusSquared)
```

**Parameters** \
`rectangle` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`circleCenter` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`circleRadiusSquared` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### WithAlpha(Color, float)
```csharp
public Color WithAlpha(Color color, float alpha)
```

**Parameters** \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`alpha` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \

#### Approach(float, float, float)
```csharp
public float Approach(float from, float target, float amount)
```

**Parameters** \
`from` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`target` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`amount` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Clamp01(float)
```csharp
public float Clamp01(float v)
```

**Parameters** \
`v` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Clamp01(int)
```csharp
public float Clamp01(int v)
```

**Parameters** \
`v` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### ClampTime(float, float)
```csharp
public float ClampTime(float elapsed, float maxTime)
```

Takes an elapsed time and coverts it to a 0-1 range

**Parameters** \
`elapsed` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`maxTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Exceptions** \
[NotImplementedException](https://learn.microsoft.com/en-us/dotnet/api/System.NotImplementedException?view=net-7.0) \
\
#### ConvertLayerToLayerDepth(int)
```csharp
public float ConvertLayerToLayerDepth(int layer)
```

**Parameters** \
`layer` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Distance(float, float, float, float)
```csharp
public float Distance(float x1, float y1, float x2, float y2)
```

Distance check.

**Parameters** \
`x1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`x2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
The distance between the two points.\

#### DistanceLinePoint(float, float, float, float, float, float)
```csharp
public float DistanceLinePoint(float x, float y, float x1, float y1, float x2, float y2)
```

Distance between a line and a point.

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`x1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y1` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`x2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y2` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
The distance from the point to the line.\

#### DistanceRectPoint(float, float, float, float, float, float)
```csharp
public float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh)
```

Find the distance between a point and a rectangle.

**Parameters** \
`px` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`py` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rx` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`ry` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rw` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rh` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
The distance.  Returns 0 if the point is within the rectangle.\

#### Lerp(float, float, float)
```csharp
public float Lerp(float origin, float target, float factor)
```

**Parameters** \
`origin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`target` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`factor` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Remap(float, float, float, float, float)
```csharp
public float Remap(float input, float inputMin, float inputMax, float min, float max)
```

**Parameters** \
`input` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`inputMin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`inputMax` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`min` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`max` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### CeilToInt(float)
```csharp
public int CeilToInt(float v)
```

**Parameters** \
`v` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ConvertLayerDepthToLayer(float)
```csharp
public int ConvertLayerDepthToLayer(float layerDepth)
```

**Parameters** \
`layerDepth` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### FloorToInt(float)
```csharp
public int FloorToInt(float v)
```

**Parameters** \
`v` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### OneD(Point, int)
```csharp
public int OneD(Point p, int width)
```

**Parameters** \
`p` [Point](/Murder/Core/Geometry/Point.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### OneD(int, int, int)
```csharp
public int OneD(int x, int y, int width)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### RoundToEven(float)
```csharp
public int RoundToEven(float v)
```

**Parameters** \
`v` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### RoundToInt(float)
```csharp
public int RoundToInt(float v)
```

Rounds and converts a number to integer with [MathF.Round(System.Single)](https://learn.microsoft.com/en-us/dotnet/api/System.MathF?view=net-7.0).

**Parameters** \
`v` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### WrapAround(int, Int32&, Int32&)
```csharp
public int WrapAround(int value, Int32& min, Int32& max)
```

**Parameters** \
`value` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`min` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`max` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ToPoint(Vector2)
```csharp
public Point ToPoint(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \

#### Shrink(Rectangle, int)
```csharp
public Rectangle Shrink(Rectangle rectangle, int amount)
```

**Parameters** \
`rectangle` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`amount` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Rectangle](/Murder/Core/Geometry/Rectangle.html) \

#### AddOnce(IList<T>, T)
```csharp
public T AddOnce(IList<T> list, T item)
```

**Parameters** \
`list` [IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \
`item` [T]() \

**Returns** \
[T]() \

#### RepeatingArray(T, int)
```csharp
public T[] RepeatingArray(T value, int size)
```

**Parameters** \
`value` [T]() \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[T[]]() \

#### Approach(Vector2&, Vector2&, float)
```csharp
public Vector2 Approach(Vector2& from, Vector2& target, float amount)
```

**Parameters** \
`from` [Vector2&](/Murder/Core/Geometry/Vector2.html) \
`target` [Vector2&](/Murder/Core/Geometry/Vector2.html) \
`amount` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### Normalized(Vector2&)
```csharp
public Vector2 Normalized(Vector2& vector2)
```

**Parameters** \
`vector2` [Vector2&](/Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### PointInCircleEdge(float)
```csharp
public Vector2 PointInCircleEdge(float percent)
```

**Parameters** \
`percent` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### RandomPointInCircleEdge()
```csharp
public Vector2 RandomPointInCircleEdge()
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### RandomPointInsideCircle()
```csharp
public Vector2 RandomPointInsideCircle()
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### ToCore(Vector2)
```csharp
public Vector2 ToCore(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### ToSysVector2(Point)
```csharp
public Vector2 ToSysVector2(Point point)
```

**Parameters** \
`point` [Point](https://docs.monogame.net/api/Microsoft.Xna.Framework.Point.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToSysVector2(Vector2)
```csharp
public Vector2 ToSysVector2(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToXnaVector2(Vector2)
```csharp
public Vector2 ToXnaVector2(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### WithAlpha(Vector4, float)
```csharp
public Vector4 WithAlpha(Vector4 color, float alpha)
```

**Parameters** \
`color` [Vector4](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector4.html) \
`alpha` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector4](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector4.html) \

#### WithAlpha(Vector4, float)
```csharp
public Vector4 WithAlpha(Vector4 color, float alpha)
```

**Parameters** \
`color` [Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
`alpha` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \

#### Populate(T[], T)
```csharp
public void Populate(T[] arr, T value)
```

**Parameters** \
`arr` [T[]]() \
`value` [T]() \



⚡