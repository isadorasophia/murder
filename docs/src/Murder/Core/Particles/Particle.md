# Particle

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public sealed struct Particle
```

### ⭐ Constructors
```csharp
public Particle()
```

```csharp
public Particle(ParticleTexture texture, ImmutableArray<T> colors, ImmutableArray<T> scale, ParticleValueProperty alpha, ParticleValueProperty acceleration, ParticleValueProperty friction, ParticleValueProperty startVelocity, ParticleValueProperty rotationSpeed, ParticleValueProperty rotation, ParticleValueProperty lifeTime, bool rotateWithVelocity, float sortOffset)
```

**Parameters** \
`texture` [ParticleTexture](../../../Murder/Core/Particles/ParticleTexture.html) \
`colors` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`scale` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`alpha` [ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
`acceleration` [ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
`friction` [ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
`startVelocity` [ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
`rotationSpeed` [ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
`rotation` [ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
`lifeTime` [ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
`rotateWithVelocity` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`sortOffset` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Acceleration
```csharp
public readonly ParticleValueProperty Acceleration;
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### Alpha
```csharp
public readonly ParticleValueProperty Alpha;
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### Colors
```csharp
public readonly ImmutableArray<T> Colors;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### FollowEntityPosition
```csharp
public readonly bool FollowEntityPosition;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Friction
```csharp
public readonly ParticleValueProperty Friction;
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### Gravity
```csharp
public readonly ParticleVectorValueProperty Gravity;
```

**Returns** \
[ParticleVectorValueProperty](../../../Murder/Core/Particles/ParticleVectorValueProperty.html) \
#### LifeTime
```csharp
public readonly ParticleValueProperty LifeTime;
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### RotateWithVelocity
```csharp
public readonly bool RotateWithVelocity;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Rotation
```csharp
public readonly ParticleValueProperty Rotation;
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### RotationSpeed
```csharp
public readonly ParticleValueProperty RotationSpeed;
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### Scale
```csharp
public readonly ImmutableArray<T> Scale;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### SortOffset
```csharp
public readonly float SortOffset;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### SpriteBatch
```csharp
public readonly int SpriteBatch;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### StartVelocity
```csharp
public readonly ParticleValueProperty StartVelocity;
```

**Returns** \
[ParticleValueProperty](../../../Murder/Core/Particles/ParticleValueProperty.html) \
#### Texture
```csharp
public readonly ParticleTexture Texture;
```

**Returns** \
[ParticleTexture](../../../Murder/Core/Particles/ParticleTexture.html) \
### ⭐ Methods
#### CalculateColor(float)
```csharp
public Color CalculateColor(float delta)
```

Calculate the color of a particle in a <paramref name="delta" /> with internal {0, 1}.

**Parameters** \
`delta` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[Color](../../../Murder/Core/Graphics/Color.html) \

#### WithRotation(float)
```csharp
public Particle WithRotation(float rotation)
```

**Parameters** \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[Particle](../../../Murder/Core/Particles/Particle.html) \

#### WithTexture(ParticleTexture)
```csharp
public Particle WithTexture(ParticleTexture texture)
```

**Parameters** \
`texture` [ParticleTexture](../../../Murder/Core/Particles/ParticleTexture.html) \

**Returns** \
[Particle](../../../Murder/Core/Particles/Particle.html) \

#### CalculateScale(float)
```csharp
public Vector2 CalculateScale(float delta)
```

Calculate the scale of a particle in a <paramref name="delta" /> with internal {0, 1}.

**Parameters** \
`delta` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \



⚡