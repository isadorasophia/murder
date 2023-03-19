# StateMachineSystem

**Namespace:** Murder.Systems \
**Assembly:** Murder.dll

```csharp
public class StateMachineSystem : IUpdateSystem, ISystem, IReactiveSystem
```

**Implements:** _[IUpdateSystem](/Bang/Systems/IUpdateSystem.html), [ISystem](/Bang/Systems/ISystem.html), [IReactiveSystem](/Bang/Systems/IReactiveSystem.html)_

### ⭐ Constructors
```csharp
public StateMachineSystem()
```

### ⭐ Methods
#### OnAdded(World, ImmutableArray<T>)
```csharp
public virtual void OnAdded(World world, ImmutableArray<T> entities)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### OnModified(World, ImmutableArray<T>)
```csharp
public virtual void OnModified(World world, ImmutableArray<T> entities)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### OnRemoved(World, ImmutableArray<T>)
```csharp
public virtual void OnRemoved(World world, ImmutableArray<T> entities)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`entities` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### Update(Context)
```csharp
public virtual void Update(Context context)
```

**Parameters** \
`context` [Context](/Bang/Contexts/Context.html) \



⚡