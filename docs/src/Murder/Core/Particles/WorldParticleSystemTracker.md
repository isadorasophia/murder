# WorldParticleSystemTracker

**Namespace:** Murder.Core.Particles \
**Assembly:** Murder.dll

```csharp
public class WorldParticleSystemTracker
```

### ⭐ Constructors
```csharp
public WorldParticleSystemTracker(int seed)
```

**Parameters** \
`seed` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

### ⭐ Methods
#### Synchronize(Entity)
```csharp
public bool Synchronize(Entity particleEntity)
```

**Parameters** \
`particleEntity` [Entity](../..//Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Track(Entity)
```csharp
public bool Track(Entity particleEntity)
```

**Parameters** \
`particleEntity` [Entity](../..//Bang/Entities/Entity.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FetchActiveParticleTrackers()
```csharp
public ReadOnlySpan<T> FetchActiveParticleTrackers()
```

Fetch all the active particle trackers.

**Returns** \
[ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \

#### Activate(int)
```csharp
public void Activate(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Deactivate(int)
```csharp
public void Deactivate(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Step(World, float)
```csharp
public void Step(World world, float dt)
```

**Parameters** \
`world` [World](../..//Bang/World.html) \
`dt` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Untrack(Entity)
```csharp
public void Untrack(Entity particleEntity)
```

**Parameters** \
`particleEntity` [Entity](../..//Bang/Entities/Entity.html) \



⚡