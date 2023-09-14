# Vector2

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Vector2 : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public Vector2(float v)
```

**Parameters** \
`v` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public Vector2(float x, float y)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Center
```csharp
public static Vector2 Center { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Down
```csharp
public static Vector2 Down { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### HasValue
```csharp
public bool HasValue { get; }
```

Checks if this vector has any value other than zero

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Height
```csharp
public float Height { get; }
```

A quick shorthand for when using a vector as a "size"

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Left
```csharp
public static Vector2 Left { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### One
```csharp
public static Vector2 One { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Point
```csharp
public Point Point { get; }
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### Right
```csharp
public static Vector2 Right { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Up
```csharp
public static Vector2 Up { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### Width
```csharp
public float Width { get; }
```

A quick shorthand for when using a vector as a "size"

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### X
```csharp
public float X;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### XY
```csharp
public ValueTuple<T1, T2> XY { get; }
```

**Returns** \
[ValueTuple\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.ValueTuple-2?view=net-7.0) \
#### Y
```csharp
public float Y;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Zero
```csharp
public static Vector2 Zero { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
### ⭐ Methods
#### Angle()
```csharp
public float Angle()
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### CalculateAngle(Vector2, Vector2, Vector2)
```csharp
public float CalculateAngle(Vector2 a, Vector2 b, Vector2 c)
```

Calculates the internal angle of a triangle.

**Parameters** \
`a` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
\
`b` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
\
`c` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### Deviation(Vector2, Vector2)
```csharp
public float Deviation(Vector2 vec1, Vector2 vec2)
```

**Parameters** \
`vec1` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`vec2` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Distance(Vector2, Vector2)
```csharp
public float Distance(Vector2 a, Vector2 b)
```

**Parameters** \
`a` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`b` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Dot(Vector2, Vector2)
```csharp
public float Dot(Vector2 a, Vector2 b)
```

**Parameters** \
`a` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`b` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Dot(Vector2)
```csharp
public float Dot(Vector2 other)
```

**Parameters** \
`other` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

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

Cheaper than checking [Vector2.Length](../../../Murder/Core/Geometry/Vector2.html#Length), useful when comparing distances.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### Manhattan()
```csharp
public float Manhattan()
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Ceil()
```csharp
public Point Ceil()
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### Floor()
```csharp
public Point Floor()
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### Round()
```csharp
public Point Round()
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### ToGridPoint()
```csharp
public Point ToGridPoint()
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \

#### ToPosition()
```csharp
public PositionComponent ToPosition()
```

**Returns** \
[PositionComponent](../..//Murder/Components/PositionComponent.html) \

#### Clamp(Vector2, Vector2, Vector2)
```csharp
public Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
```

**Parameters** \
`value1` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`min` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`max` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### FromAngle(float)
```csharp
public Vector2 FromAngle(float angle)
```

Creates a vector from an angle in radians.

**Parameters** \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
\

#### Lerp(Vector2, Vector2, float)
```csharp
public Vector2 Lerp(Vector2 origin, Vector2 target, float factor)
```

**Parameters** \
`origin` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`target` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`factor` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### LerpSnap(Vector2, Vector2, double, float)
```csharp
public Vector2 LerpSnap(Vector2 origin, Vector2 target, double factor, float threshold)
```

**Parameters** \
`origin` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`target` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`factor` [double](https://learn.microsoft.com/en-us/dotnet/api/System.Double?view=net-7.0) \
`threshold` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### LerpSnap(Vector2, Vector2, float, float)
```csharp
public Vector2 LerpSnap(Vector2 origin, Vector2 target, float factor, float threshold)
```

**Parameters** \
`origin` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`target` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \
`factor` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`threshold` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### Mirror(Vector2)
```csharp
public Vector2 Mirror(Vector2 center)
```

**Parameters** \
`center` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### Normalized()
```csharp
public Vector2 Normalized()
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### Parse(string)
```csharp
public Vector2 Parse(string str)
```

**Parameters** \
`str` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### PerpendicularClockwise()
```csharp
public Vector2 PerpendicularClockwise()
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### PerpendicularCounterClockwise()
```csharp
public Vector2 PerpendicularCounterClockwise()
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### Reverse()
```csharp
public Vector2 Reverse()
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### Rotate(float)
```csharp
public Vector2 Rotate(float angle)
```

Returns a new vector, rotated by the given angle. In radians.

**Parameters** \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
\

#### Round(Vector2)
```csharp
public Vector2 Round(Vector2 vector)
```

**Parameters** \
`vector` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \

#### ToSys()
```csharp
public Vector2 ToSys()
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToXna()
```csharp
public Vector2 ToXna()
```

**Returns** \
[Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \

#### ToVector3()
```csharp
public Vector3 ToVector3()
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \

#### Equals(Vector2)
```csharp
public virtual bool Equals(Vector2 other)
```

**Parameters** \
`other` [Vector2](../..//Murder/Core/Geometry/Vector2.html) \

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