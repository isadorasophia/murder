# Color

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public sealed struct Color : IEquatable<T>
```

**Implements:** _[IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public Color(float r, float g, float b, float a)
```

**Parameters** \
`r` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`g` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`b` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`a` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public Color(float r, float g, float b)
```

**Parameters** \
`r` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`g` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`b` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### A
```csharp
public float A;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### B
```csharp
public float B;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Black
```csharp
public static Color Black { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### Blue
```csharp
public static Color Blue { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### BrightGray
```csharp
public static Color BrightGray { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### ColdGray
```csharp
public static Color ColdGray { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### G
```csharp
public float G;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Gray
```csharp
public static Color Gray { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### Green
```csharp
public static Color Green { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### Magenta
```csharp
public static Color Magenta { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### Orange
```csharp
public static Color Orange { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### R
```csharp
public float R;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Red
```csharp
public static Color Red { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### Transparent
```csharp
public static Color Transparent { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### WarmGray
```csharp
public static Color WarmGray { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### White
```csharp
public static Color White { get; }
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
### ⭐ Methods
#### CreateFrom256(int, int, int)
```csharp
public Color CreateFrom256(int r, int g, int b)
```

**Parameters** \
`r` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`g` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`b` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \

#### Darken(float)
```csharp
public Color Darken(float r)
```

**Parameters** \
`r` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \

#### FadeAlpha(float)
```csharp
public Color FadeAlpha(float alpha)
```

**Parameters** \
`alpha` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \

#### FromHex(string)
```csharp
public Color FromHex(string hex)
```

Parses a string <paramref name="hex" /> to [Color](../../..//Murder/Core/Graphics/Color.html).

**Parameters** \
`hex` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
\

#### Lerp(Color, Color, float)
```csharp
public Color Lerp(Color a, Color b, float factor)
```

**Parameters** \
`a` [Color](../..//Murder/Core/Graphics/Color.html) \
`b` [Color](../..//Murder/Core/Graphics/Color.html) \
`factor` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \

#### Parse(string)
```csharp
public Color Parse(string str)
```

**Parameters** \
`str` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \

#### Premultiply()
```csharp
public Color Premultiply()
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \

#### ToSysVector4()
```csharp
public Vector4 ToSysVector4()
```

**Returns** \
[Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \

#### Equals(Color)
```csharp
public virtual bool Equals(Color other)
```

**Parameters** \
`other` [Color](../..//Murder/Core/Graphics/Color.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡