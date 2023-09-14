# Emitter

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public sealed struct Emitter
```

### ⭐ Constructors
```csharp
public Emitter()
```

```csharp
public Emitter(int maxParticles, EmitterShape shape, ParticleValueProperty angle, ParticleValueProperty particlesPerSecond, ParticleIntValueProperty burst, ParticleValueProperty speed)
```

**Parameters** \
`maxParticles` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`shape` [EmitterShape](../..//Murder/Core/Particles/EmitterShape.html) \
`angle` [ParticleValueProperty](../..//Murder/Core/Particles/ParticleValueProperty.html) \
`particlesPerSecond` [ParticleValueProperty](../..//Murder/Core/Particles/ParticleValueProperty.html) \
`burst` [ParticleIntValueProperty](../..//Murder/Core/Particles/ParticleIntValueProperty.html) \
`speed` [ParticleValueProperty](../..//Murder/Core/Particles/ParticleValueProperty.html) \

### ⭐ Properties
#### Angle
```csharp
public readonly ParticleValueProperty Angle;
```

**Returns** \
[ParticleValueProperty](../..//Murder/Core/Particles/ParticleValueProperty.html) \
#### Burst
```csharp
public readonly ParticleIntValueProperty Burst;
```

**Returns** \
[ParticleIntValueProperty](../..//Murder/Core/Particles/ParticleIntValueProperty.html) \
#### MaxParticlesPool
```csharp
public readonly int MaxParticlesPool;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### ParticlesPerSecond
```csharp
public readonly ParticleValueProperty ParticlesPerSecond;
```

**Returns** \
[ParticleValueProperty](../..//Murder/Core/Particles/ParticleValueProperty.html) \
#### Shape
```csharp
public readonly EmitterShape Shape;
```

**Returns** \
[EmitterShape](../..//Murder/Core/Particles/EmitterShape.html) \
#### Speed
```csharp
public readonly ParticleValueProperty Speed;
```

**Returns** \
[ParticleValueProperty](../..//Murder/Core/Particles/ParticleValueProperty.html) \
### ⭐ Methods
#### WithShape(EmitterShape)
```csharp
public Emitter WithShape(EmitterShape shape)
```

**Parameters** \
`shape` [EmitterShape](../..//Murder/Core/Particles/EmitterShape.html) \

**Returns** \
[Emitter](../..//Murder/Core/Particles/Emitter.html) \



⚡