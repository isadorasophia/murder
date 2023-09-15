# ParticleSystemTracker

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public sealed struct ParticleSystemTracker
```

### ⭐ Constructors
```csharp
public ParticleSystemTracker(Emitter emitter, Particle particle, int seed)
```

**Parameters** \
`emitter` [Emitter](../../../Murder/Core/Particles/Emitter.html) \
`particle` [Particle](../../../Murder/Core/Particles/Particle.html) \
`seed` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Properties
#### CurrentTime
```csharp
public float CurrentTime { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Emitter
```csharp
public readonly Emitter Emitter;
```

**Returns** \
[Emitter](../../../Murder/Core/Particles/Emitter.html) \
#### LastEmitterPosition
```csharp
public Vector2 LastEmitterPosition { get; }
```

The last position of the emitter.

**Returns** \
[Vector2](../../../Murder/Core/Geometry/Vector2.html) \
#### Particle
```csharp
public readonly Particle Particle;
```

**Returns** \
[Particle](../../../Murder/Core/Particles/Particle.html) \
#### Particles
```csharp
public ReadOnlySpan<T> Particles { get; }
```

**Returns** \
[ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \
### ⭐ Methods
#### Step(float, bool, Vector2)
```csharp
public bool Step(float dt, bool allowSpawn, Vector2 emitterPosition)
```

Makes a "step" throughout the particle system.

**Parameters** \
`dt` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`allowSpawn` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`emitterPosition` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### Start(Vector2)
```csharp
public void Start(Vector2 emitterPosition)
```

**Parameters** \
`emitterPosition` [Vector2](../../../Murder/Core/Geometry/Vector2.html) \



⚡