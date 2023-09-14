# IReactiveSystem

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public abstract IReactiveSystem : ISystem
```

A reactive system that reacts to changes of certain components.

**Implements:** _[ISystem](../../Bang/Systems/ISystem.html)_

### ⭐ Methods
#### OnAdded(World, ImmutableArray<T>)
```csharp
public abstract void OnAdded(World world, ImmutableArray<T> entities)
```

This is called at the end of the frame for all entities that were added one of the target.
            components.
            This is not called if the entity died.

**Parameters** \
`world` [World](../../Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### OnModified(World, ImmutableArray<T>)
```csharp
public abstract void OnModified(World world, ImmutableArray<T> entities)
```

This is called at the end of the frame for all entities that modified one of the target.
            components.
            This is not called if the entity died.

**Parameters** \
`world` [World](../../Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### OnRemoved(World, ImmutableArray<T>)
```csharp
public abstract void OnRemoved(World world, ImmutableArray<T> entities)
```

This is called at the end of the frame for all entities that removed one of the target.
            components.

**Parameters** \
`world` [World](../../Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### OnActivated(World, ImmutableArray<T>)
```csharp
public virtual void OnActivated(World world, ImmutableArray<T> entities)
```

[Optional] This is called when an entity gets enabled.

**Parameters** \
`world` [World](../../Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### OnDeactivated(World, ImmutableArray<T>)
```csharp
public virtual void OnDeactivated(World world, ImmutableArray<T> entities)
```

[Optional] This is called when an entity gets disabled. Called if an entity was
            previously disabled.

**Parameters** \
`world` [World](../../Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \



⚡