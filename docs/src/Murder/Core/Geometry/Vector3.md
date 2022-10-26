# Vector3

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Vector3 : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public Vector3(float x, float y, float z)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`z` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Absolute
```csharp
public Vector3 Absolute { get; }
```

**Returns** \
[Vector3](/Murder/Core/Geometry/Vector3.html) \
#### Center
```csharp
public static Vector3 Center { get; }
```

**Returns** \
[Vector3](/Murder/Core/Geometry/Vector3.html) \
#### One
```csharp
public static Vector3 One { get; }
```

**Returns** \
[Vector3](/Murder/Core/Geometry/Vector3.html) \
#### X
```csharp
public float X;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Y
```csharp
public float Y;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Z
```csharp
public float Z;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Zero
```csharp
public static Vector3 Zero { get; }
```

**Returns** \
[Vector3](/Murder/Core/Geometry/Vector3.html) \
### ⭐ Methods
#### Distance(Vector3, Vector3)
```csharp
public float Distance(Vector3 a, Vector3 b)
```

**Parameters** \
`a` [Vector3](/Murder/Core/Geometry/Vector3.html) \
`b` [Vector3](/Murder/Core/Geometry/Vector3.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Dot(Vector3, Vector3)
```csharp
public float Dot(Vector3 a, Vector3 b)
```

**Parameters** \
`a` [Vector3](/Murder/Core/Geometry/Vector3.html) \
`b` [Vector3](/Murder/Core/Geometry/Vector3.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Dot(Vector3)
```csharp
public float Dot(Vector3 other)
```

**Parameters** \
`other` [Vector3](/Murder/Core/Geometry/Vector3.html) \

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

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### ToPosition()
```csharp
public PositionComponent ToPosition()
```

**Returns** \
[PositionComponent](/Murder/Components/PositionComponent.html) \

#### Lerp(Vector3, Vector3, float)
```csharp
public Vector3 Lerp(Vector3 origin, Vector3 target, float factor)
```

**Parameters** \
`origin` [Vector3](/Murder/Core/Geometry/Vector3.html) \
`target` [Vector3](/Murder/Core/Geometry/Vector3.html) \
`factor` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Vector3](/Murder/Core/Geometry/Vector3.html) \

#### Parse(string)
```csharp
public Vector3 Parse(string str)
```

**Parameters** \
`str` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Vector3](/Murder/Core/Geometry/Vector3.html) \

#### Round(Vector3)
```csharp
public Vector3 Round(Vector3 vector)
```

**Parameters** \
`vector` [Vector3](/Murder/Core/Geometry/Vector3.html) \

**Returns** \
[Vector3](/Murder/Core/Geometry/Vector3.html) \

#### ToSys()
```csharp
public Vector3 ToSys()
```

**Returns** \
[Vector3](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector3?view=net-7.0) \

#### ToVector3()
```csharp
public Vector3 ToVector3()
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \

#### ToXna()
```csharp
public Vector3 ToXna()
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \

#### Equals(Vector3)
```csharp
public virtual bool Equals(Vector3 other)
```

**Parameters** \
`other` [Vector3](/Murder/Core/Geometry/Vector3.html) \

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