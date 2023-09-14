# IWorldAsset

**Namespace:** Murder.Assets \
**Assembly:** Murder.dll

```csharp
public abstract IWorldAsset
```

### ⭐ Properties
#### Instances
```csharp
public abstract virtual ImmutableArray<T> Instances { get; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### WorldGuid
```csharp
public abstract virtual Guid WorldGuid { get; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
### ⭐ Methods
#### TryGetInstance(Guid)
```csharp
public abstract EntityInstance TryGetInstance(Guid instanceGuid)
```

**Parameters** \
`instanceGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[EntityInstance](../../Murder/Prefabs/EntityInstance.html) \

#### TryCreateEntityInWorld(World, Guid)
```csharp
public virtual int TryCreateEntityInWorld(World world, Guid instance)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`instance` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡