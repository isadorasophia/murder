# ParticleRuntime

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public sealed struct ParticleRuntime
```

### ⭐ Constructors
```csharp
public ParticleRuntime(float startTime, float lifetime, Vector2 position, Vector2 fromPosition, Vector2 gravity, float startAlpha, float startVelocity, float startRotation, float startAcceleration, float startFriction, float startRotationSpeed)
```

**Parameters** \
`startTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`lifetime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`fromPosition` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`gravity` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`startAlpha` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`startVelocity` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`startRotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`startAcceleration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`startFriction` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`startRotationSpeed` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Acceleration
```csharp
public float Acceleration;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Alpha
```csharp
public float Alpha;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Delta
```csharp
public float Delta { get; private set; }
```

This is the lifetime of the particle over 0 to 1.

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Friction
```csharp
public float Friction;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Gravity
```csharp
public Vector2 Gravity;
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Lifetime
```csharp
public readonly float Lifetime;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Position
```csharp
public Vector2 Position { get; }
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### Rotation
```csharp
public float Rotation;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### RotationSpeed
```csharp
public float RotationSpeed;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### StartRotation
```csharp
public float StartRotation;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Velocity
```csharp
public float Velocity;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### Step(Particle&, float, float)
```csharp
public void Step(Particle& particle, float currentTime, float dt)
```

**Parameters** \
`particle` [Particle&](../../../Murder/Core/Particles/Particle.html) \
`currentTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`dt` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### UpdateFromPosition(Vector2)
```csharp
public void UpdateFromPosition(Vector2 from)
```

**Parameters** \
`from` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \



⚡