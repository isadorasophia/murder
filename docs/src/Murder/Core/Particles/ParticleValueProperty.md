# ParticleValueProperty

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public sealed struct ParticleValueProperty
```

### ⭐ Constructors
```csharp
public ParticleValueProperty()
```

```csharp
public ParticleValueProperty(float constant)
```

**Parameters** \
`constant` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public ParticleValueProperty(float rangeStart, float rangeEnd)
```

**Parameters** \
`rangeStart` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`rangeEnd` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public ParticleValueProperty(float rangeStartMin, float rangeStartMax, float rangeEndMin, float rangeEndMax)
```

**Parameters** \
`rangeStartMin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`rangeStartMax` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`rangeEndMin` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`rangeEndMax` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Empty
```csharp
public static ParticleValueProperty Empty { get; }
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### Kind
```csharp
public readonly ParticleValuePropertyKind Kind;
```

**Returns** \
[ParticleValuePropertyKind](../../../Murder/Core/Particles/ParticleValuePropertyKind.html) \
### ⭐ Methods
#### GetRandomValue(Random)
```csharp
public float GetRandomValue(Random random)
```

**Parameters** \
`random` [Random](https://learn.microsoft.com/en-us/dotnet/api/System.Random?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### GetValueAt(float)
```csharp
public float GetValueAt(float delta)
```

Get the value of this property over a delta lifetime.

**Parameters** \
`delta` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡