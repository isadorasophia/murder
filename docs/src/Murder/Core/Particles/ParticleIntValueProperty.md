# ParticleIntValueProperty

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public sealed struct ParticleIntValueProperty
```

### ⭐ Constructors
```csharp
public ParticleIntValueProperty()
```

```csharp
public ParticleIntValueProperty(int constant)
```

**Parameters** \
`constant` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

```csharp
public ParticleIntValueProperty(int rangeStart, int rangeEnd)
```

**Parameters** \
`rangeStart` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`rangeEnd` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

```csharp
public ParticleIntValueProperty(int rangeStartMin, int rangeStartMax, int rangeEndMin, int rangeEndMax)
```

**Parameters** \
`rangeStartMin` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`rangeStartMax` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`rangeEndMin` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`rangeEndMax` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### Empty
```csharp
public static ParticleIntValueProperty Empty { get; }
```

**Returns** \
[ParticleIntValueProperty](../../../Murder/Core/Particles/ParticleIntValueProperty.html) \
#### Kind
```csharp
public readonly ParticleValuePropertyKind Kind;
```

**Returns** \
[ParticleValuePropertyKind](../../../Murder/Core/Particles/ParticleValuePropertyKind.html) \
### ⭐ Methods
#### CalculateMaxValue()
```csharp
public int CalculateMaxValue()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### GetValue(Random)
```csharp
public int GetValue(Random random)
```

**Parameters** \
`random` [Random](https://learn.microsoft.com/en-us/dotnet/api/System.Random?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡