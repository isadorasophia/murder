# Matrix

**Namespace:** Murder.Core.Geometry \
**Assembly:** Murder.dll

```csharp
public sealed struct Matrix : IEquatable<T>
```

Implements a matrix within our engine. It can be converted to other matrix data types.

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Properties
#### Identity
```csharp
public static Matrix Identity { get; }
```

Just a shorthand for [Matrix.Identity](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) for when you don't want to import the whole XNA Framework Matrix Library

**Returns** \
[Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
#### M11
```csharp
public float M11;
```

A first row and first column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M12
```csharp
public float M12;
```

A first row and second column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M13
```csharp
public float M13;
```

A first row and third column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M14
```csharp
public float M14;
```

A first row and fourth column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M21
```csharp
public float M21;
```

A second row and first column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M22
```csharp
public float M22;
```

A second row and second column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M23
```csharp
public float M23;
```

A second row and third column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M24
```csharp
public float M24;
```

A second row and fourth column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M31
```csharp
public float M31;
```

A third row and first column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M32
```csharp
public float M32;
```

A third row and second column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M33
```csharp
public float M33;
```

A third row and third column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M34
```csharp
public float M34;
```

A third row and fourth column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M41
```csharp
public float M41;
```

A fourth row and first column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M42
```csharp
public float M42;
```

A fourth row and second column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M43
```csharp
public float M43;
```

A fourth row and third column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### M44
```csharp
public float M44;
```

A fourth row and fourth column value.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### ToXnaMatrix()
```csharp
public Matrix ToXnaMatrix()
```

**Returns** \
[Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \

#### Equals(Matrix)
```csharp
public virtual bool Equals(Matrix other)
```

**Parameters** \
`other` [Matrix](../../../Murder/Core/Geometry/Matrix.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡