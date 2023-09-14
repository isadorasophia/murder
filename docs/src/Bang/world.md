# World

**Namespace:** Bang \
**Assembly:** Bang.dll

```csharp
public class World : IDisposable
```

This is the internal representation of a world within ECS.
            A world has the knowledge of all the entities and all the systems that exist within the game.
            This handles dispatching information and handling disposal of entities.

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public World(IList<T> systems)
```

Initialize the world!

**Parameters** \
`systems` [IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \
\

**Exceptions** \
[ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/System.ArgumentException?view=net-7.0) \
\
### ⭐ Properties
#### _cachedRenderSystems
```csharp
protected readonly SortedList<TKey, TValue> _cachedRenderSystems;
```

This must be called by engine implementations of Bang to handle with rendering.

**Returns** \
[SortedList\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.SortedList-2?view=net-7.0) \
#### _overallStopwatch
```csharp
protected readonly Stopwatch _overallStopwatch;
```

**Returns** \
[Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/System.Diagnostics.Stopwatch?view=net-7.0) \
#### _stopwatch
```csharp
protected readonly Stopwatch _stopwatch;
```

**Returns** \
[Stopwatch](https://learn.microsoft.com/en-us/dotnet/api/System.Diagnostics.Stopwatch?view=net-7.0) \
#### Contexts
```csharp
protected readonly Dictionary<TKey, TValue> Contexts;
```

Maps all the context IDs with the context.
            We might add new ones if a system calls for a new context filter.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### DIAGNOSTICS_MODE
```csharp
public static bool DIAGNOSTICS_MODE;
```

Use this to set whether diagnostics should be pulled from the world run.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### EntityCount
```csharp
public int EntityCount { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### FixedUpdateCounters
```csharp
public readonly Dictionary<TKey, TValue> FixedUpdateCounters;
```

This has the duration of each fixed update system (id) to its corresponding time (in ms).
            See [World.IdToSystem](../Bang/World.html#idtosystem) on how to fetch the actual system.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### IdToSystem
```csharp
public readonly ImmutableDictionary<TKey, TValue> IdToSystem;
```

Used when fetching systems based on its unique identifier.
            Maps: System order id -&gt; System instance.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### IsPaused
```csharp
public bool IsPaused { get; private set; }
```

Whether the world has been queried to be on pause or not.
            See [World.Pause](../Bang/World.html#pause).

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### ReactiveCounters
```csharp
public readonly Dictionary<TKey, TValue> ReactiveCounters;
```

This has the duration of each reactive system (id) to its corresponding time (in ms).
            See [World.IdToSystem](../Bang/World.html#idtosystem) on how to fetch the actual system.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### UpdateCounters
```csharp
public readonly Dictionary<TKey, TValue> UpdateCounters;
```

This has the duration of each update system (id) to its corresponding time (in ms).
            See [World.IdToSystem](../Bang/World.html#idtosystem) on how to fetch the actual system.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
### ⭐ Methods
#### ClearDiagnosticsCountersForSystem(int)
```csharp
protected virtual void ClearDiagnosticsCountersForSystem(int systemId)
```

Implemented by custom world in order to clear diagnostic information about the world.

**Parameters** \
`systemId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

#### InitializeDiagnosticsForSystem(int, ISystem)
```csharp
protected virtual void InitializeDiagnosticsForSystem(int systemId, ISystem system)
```

Implemented by custom world in order to express diagnostic information about the world.

**Parameters** \
`systemId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`system` [ISystem](../Bang/Systems/ISystem.html) \

#### InitializeDiagnosticsCounters()
```csharp
protected void InitializeDiagnosticsCounters()
```

Initialize the performance counters according to the systems present in the world.

#### ActivateSystem()
```csharp
public bool ActivateSystem()
```

Activate a system within our world.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ActivateSystem(Type)
```csharp
public bool ActivateSystem(Type t)
```

Activate a system of type <paramref name="t" /> within our world.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### DeactivateSystem()
```csharp
public bool DeactivateSystem()
```

Deactivate a system within our world.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeactivateSystem(int, bool)
```csharp
public bool DeactivateSystem(int id, bool immediately)
```

Deactivate a system within our world.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`immediately` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeactivateSystem(Type)
```csharp
public bool DeactivateSystem(Type t)
```

Deactivate a system within our world.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsSystemActive(Type)
```csharp
public bool IsSystemActive(Type t)
```

Whether a system is active within the world.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FindLookupImplementation()
```csharp
public ComponentsLookup FindLookupImplementation()
```

Look for an implementation for the lookup table of components.

**Returns** \
[ComponentsLookup](../Bang/ComponentsLookup.html) \

#### AddEntity()
```csharp
public Entity AddEntity()
```

Add a new empty entity to the world. 
            This will map the instance to the world.
            Any components added after this entity has been created will be notified to any reactive systems.

**Returns** \
[Entity](../Bang/Entities/Entity.html) \

#### AddEntity(IComponent[])
```csharp
public Entity AddEntity(IComponent[] components)
```

Add a single entity to the world (e.g. collection of <paramref name="components" />). 
            This will map the instance to the world.

**Parameters** \
`components` [IComponent[]](../Bang/Components/IComponent.html) \

**Returns** \
[Entity](../Bang/Entities/Entity.html) \

#### AddEntity(T?, IComponent[])
```csharp
public Entity AddEntity(T? id, IComponent[] components)
```

**Parameters** \
`id` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`components` [IComponent[]](../Bang/Components/IComponent.html) \

**Returns** \
[Entity](../Bang/Entities/Entity.html) \

#### GetEntity(int)
```csharp
public Entity GetEntity(int id)
```

Get an entity with the specific id.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](../Bang/Entities/Entity.html) \

#### GetUniqueEntity()
```csharp
public Entity GetUniqueEntity()
```

Get an entity with the unique component <typeparamref name="T" />.

**Returns** \
[Entity](../Bang/Entities/Entity.html) \

#### TryGetEntity(int)
```csharp
public Entity TryGetEntity(int id)
```

Tries to get an entity with the specific id.
            If the entity is no longer among us, return null.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](../Bang/Entities/Entity.html) \

#### TryGetUniqueEntity()
```csharp
public Entity TryGetUniqueEntity()
```

Try to get a unique entity that owns <typeparamref name="T" />.

**Returns** \
[Entity](../Bang/Entities/Entity.html) \

#### GetAllEntities()
```csharp
public ImmutableArray<T> GetAllEntities()
```

This should be used very cautiously! I hope you know what you are doing.
            It fetches all the entities within the world and return them.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetEntitiesWith(ContextAccessorFilter, Type[])
```csharp
public ImmutableArray<T> GetEntitiesWith(ContextAccessorFilter filter, Type[] components)
```

Retrieve a context for the specified filter and components.

**Parameters** \
`filter` [ContextAccessorFilter](../Bang/Contexts/ContextAccessorFilter.html) \
`components` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetEntitiesWith(Type[])
```csharp
public ImmutableArray<T> GetEntitiesWith(Type[] components)
```

Retrieve a context for the specified filter and components.

**Parameters** \
`components` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetUnique()
```csharp
public T GetUnique()
```

Get the unique component within an entity <typeparamref name="T" />.

**Returns** \
[T](../) \

#### TryGetUnique()
```csharp
public T? TryGetUnique()
```

Try to get a unique entity that owns <typeparamref name="T" />.

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
\

#### Dispose()
```csharp
public virtual void Dispose()
```

#### Pause()
```csharp
public virtual void Pause()
```

Pause all the set of systems that qualify in [World.IsPauseSystem(Bang.Systems.ISystem)](../Bang/World.html).
            A paused system will no longer be called on any [World.Update](../Bang/World.html#update) calls.

#### Resume()
```csharp
public virtual void Resume()
```

This will resume all paused systems.

#### ActivateAllSystems()
```csharp
public void ActivateAllSystems()
```

Activate all systems across the world.

#### DeactivateAllSystems(Type[])
```csharp
public void DeactivateAllSystems(Type[] skip)
```

Deactivate all systems across the world.

**Parameters** \
`skip` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

#### Exit()
```csharp
public void Exit()
```

Call to end all systems.
            This is called right before shutting down or switching scenes.

#### FixedUpdate()
```csharp
public void FixedUpdate()
```

Calls update on all [IFixedUpdateSystem](../Bang/Systems/IFixedUpdateSystem.html) systems.
            This will be called on fixed intervals.

#### Start()
```csharp
public void Start()
```

Call start on all systems.
            This is called before any updates and will notify any reactive systems by the end of it.

#### Update()
```csharp
public void Update()
```

Calls update on all [IUpdateSystem](../Bang/Systems/IUpdateSystem.html) systems.
            At the end of update, it will notify all reactive systems of any changes made to entities
            they were watching.
            Finally, it destroys all pending entities and clear all messages.



⚡