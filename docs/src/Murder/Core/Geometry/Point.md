# Point

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Point : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public Point(float x, float y)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public Point(int v)
```

**Parameters** \
`v` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

```csharp
public Point(int x, int y)
```

**Parameters** \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Down
```csharp
public static Point Down { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### Flipped
```csharp
public static Point Flipped { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### HalfCell
```csharp
public static Point HalfCell { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### One
```csharp
public static Point One { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### X
```csharp
public int X;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### XY
```csharp
public ValueTuple<T1, T2> XY { get; }
```

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \
#### Y
```csharp
public int Y;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Zero
```csharp
public static Point Zero { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
### ⭐ Methods
#### Length()
```csharp
public float Length()
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### LengthSquared()
```csharp
public int LengthSquared()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Mirror(Point)
```csharp
public Point Mirror(Point center)
```

**Parameters** \
`center` [Point](../..//Murder/Core/Geometry/Point.html) \

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### Scale(Point)
```csharp
public Point Scale(Point other)
```

**Parameters** \
`other` [Point](../..//Murder/Core/Geometry/Point.html) \

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### ToWorldPosition()
```csharp
public Point ToWorldPosition()
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### BreakInTwo()
```csharp
public ValueTuple<T1, T2> BreakInTwo()
```

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \

#### ToVector2()
```csharp
public Vector2 ToVector2()
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### ToVector3()
```csharp
public Vector3 ToVector3()
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \

#### Equals(Point)
```csharp
public virtual bool Equals(Point other)
```

**Parameters** \
`other` [Point](../..//Murder/Core/Geometry/Point.html) \

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

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡