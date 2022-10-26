# IReactiveSystem

**Namespace:** Bang.Systems \
**Assembly:** Bang.dll

```csharp
public abstract IReactiveSystem : ISystem
```

A reactive system that reacts to changes of certain components.

**Implements:** _[ISystem](/Bang/Systems/ISystem.html)_

### ⭐ Methods
#### OnAdded(World, ImmutableArray<T>)
```csharp
public abstract ValueTask OnAdded(World world, ImmutableArray<T> entities)
```

This is called at the end of the frame for all entities which were added one of the target
            components.
            This is not called if the entity died.

**Parameters** \
`world` [World](/Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### OnModified(World, ImmutableArray<T>)
```csharp
public abstract ValueTask OnModified(World world, ImmutableArray<T> entities)
```

This is called at the end of the frame for all entities which modified one of the target
            components.
            This is not called if the entity died.

**Parameters** \
`world` [World](/Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### OnRemoved(World, ImmutableArray<T>)
```csharp
public abstract ValueTask OnRemoved(World world, ImmutableArray<T> entities)
```

This is called at the end of the frame for all entities which removed one of the target
            components.

**Parameters** \
`world` [World](/Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \



⚡