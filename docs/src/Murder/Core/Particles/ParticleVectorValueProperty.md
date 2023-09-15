# ParticleVectorValueProperty

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public sealed struct ParticleVectorValueProperty
```

### ⭐ Constructors
```csharp
public ParticleVectorValueProperty()
```

```csharp
public ParticleVectorValueProperty(Vector2 constant)
```

**Parameters** \
`constant` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \

```csharp
public ParticleVectorValueProperty(Vector2 rangeStart, Vector2 rangeEnd)
```

**Parameters** \
`rangeStart` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`rangeEnd` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \

```csharp
public ParticleVectorValueProperty(Vector2 rangeStartMin, Vector2 rangeStartMax, Vector2 rangeEndMin, Vector2 rangeEndMax)
```

**Parameters** \
`rangeStartMin` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`rangeStartMax` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`rangeEndMin` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
`rangeEndMax` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \

### ⭐ Properties
#### Empty
```csharp
public static ParticleVectorValueProperty Empty { get; }
```

**Returns** \
[ParticleVectorValueProperty](../../../Murder/Core/Particles/ParticleVectorValueProperty.html) \
#### Kind
```csharp
public readonly ParticleValuePropertyKind Kind;
```

**Returns** \
[ParticleValuePropertyKind](../../../Murder/Core/Particles/ParticleValuePropertyKind.html) \
### ⭐ Methods
#### GetRandomValue(Random)
```csharp
public Vector2 GetRandomValue(Random random)
```

**Parameters** \
`random` [Random](https://learn.microsoft.com/en-us/dotnet/api/System.Random?view=net-7.0) \

**Returns** \
[Vector2](../../../Murder/Core/Geometry/Vector2.html) \

#### GetValueAt(float)
```csharp
public Vector2 GetValueAt(float delta)
```

Get the value of this property over a delta lifetime.

**Parameters** \
`delta` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[Vector2](../../../Murder/Core/Geometry/Vector2.html) \



⚡