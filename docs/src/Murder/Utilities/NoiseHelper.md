# NoiseHelper

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public sealed class NoiseHelper
```

### ⭐ Constructors
```csharp
public NoiseHelper()
```

### ⭐ Methods
#### CarmodyNoise(float, float, float, bool, bool)
```csharp
public float CarmodyNoise(float x, float y, float z, bool doReseed, bool doNormalize)
```

Carmody's implementation of a Simplex Noise generator

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`z` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`doReseed` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`doNormalize` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### GustavsonNoise(float, float, bool, bool)
```csharp
public float GustavsonNoise(float xin, float yin, bool doRecalculate, bool doNormalize)
```

Gustavson's implementation of a 2D Simplex Noise generator

**Parameters** \
`xin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`yin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`doRecalculate` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`doNormalize` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### GustavsonNoise(float, float, float, bool, bool)
```csharp
public float GustavsonNoise(float xin, float yin, float zin, bool doRecalculate, bool doNormalize)
```

Gustavson's implementation of a 3D Simplex Noise generator

**Parameters** \
`xin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`yin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`zin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`doRecalculate` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`doNormalize` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### Normalize(float, float, float)
```csharp
public float Normalize(float value, float min, float max)
```

Normalizes a float to the range of 0 to 1

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`min` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`max` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### NormalizeSigned(float, float, float)
```csharp
public float NormalizeSigned(float value, float min, float max)
```

Normalizes a float to the range of -1 to 1

**Parameters** \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`min` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`max` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### Simple01(float, float)
```csharp
public float Simple01(float point, float frequency)
```

**Parameters** \
`point` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`frequency` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Simple2D(float, float, float)
```csharp
public float Simple2D(float x, float y, float frequency)
```

**Parameters** \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`frequency` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### LCG(long, long, long, long)
```csharp
public long LCG(long seed, long a, long c, long m)
```

**Parameters** \
`seed` [long](https://learn.microsoft.com/en-us/dotnet/api/System.Int64?view=net-7.0) \
`a` [long](https://learn.microsoft.com/en-us/dotnet/api/System.Int64?view=net-7.0) \
`c` [long](https://learn.microsoft.com/en-us/dotnet/api/System.Int64?view=net-7.0) \
`m` [long](https://learn.microsoft.com/en-us/dotnet/api/System.Int64?view=net-7.0) \

**Returns** \
[long](https://learn.microsoft.com/en-us/dotnet/api/System.Int64?view=net-7.0) \



⚡