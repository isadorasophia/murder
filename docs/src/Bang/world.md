# World

**Namespace:** Bang \
**Assembly:** Bang.dll

```csharp
public class World
```

This is the internal representation of a world within ECS.
            A world has the knowledge of all the entities and all the systems that exist within the game.
            This handles dispatching information and handling disposal of entities.

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
If no systems are passed to the world.\
### ⭐ Properties
#### _cachedRenderSystems
```csharp
protected readonly SortedList<TKey, TValue> _cachedRenderSystems;
```

This must be called by engine implementations of Bang to handle with rendering.

**Returns** \
[SortedList\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.SortedList-2?view=net-7.0) \
#### Contexts
```csharp
protected readonly Dictionary<TKey, TValue> Contexts;
```

Maps all the context IDs with the context.
            We might add new ones if a system calls for a new context filter.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### IsPaused
```csharp
public bool IsPaused { get; private set; }
```

Whether the world has been queried to be on pause or not.
            See [World.Pause](/bang/world.html#pause).

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
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
Whether the system is found and has been activated.\

#### DeactivateSystem()
```csharp
public bool DeactivateSystem()
```

Deactivate a system within our world.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeactivateSystem(int)
```csharp
public bool DeactivateSystem(int id)
```

Deactivate a system within our world.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

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

#### AddEntity()
```csharp
public Entity AddEntity()
```

Add a new empty entity to the world. 
            This will map the instance to the world.
            Any components added after this entity has been created will be notified to any reactive systems.

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### AddEntity(IComponent[])
```csharp
public Entity AddEntity(IComponent[] components)
```

Add a single entity to the world (e.g. collection of <paramref name="components" />). 
            This will map the instance to the world.

**Parameters** \
`components` [IComponent[]](/Bang/Components/IComponent.html) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### AddEntity(IEnumerable<T>)
```csharp
public Entity AddEntity(IEnumerable<T> components)
```

Add a single entity to the world (e.g. collection of <paramref name="components" />). 
            This will map the instance to the world.

**Parameters** \
`components` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### AddEntity(T?, IEnumerable<T>)
```csharp
public Entity AddEntity(T? id, IEnumerable<T> components)
```

**Parameters** \
`id` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`components` [IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### GetEntity(int)
```csharp
public Entity GetEntity(int id)
```

Get an entity with the specific id.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### GetUniqueEntity()
```csharp
public Entity GetUniqueEntity()
```

Get an entity with the unique component <typeparamref name="T" />.

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### TryGetEntity(int)
```csharp
public Entity TryGetEntity(int id)
```

Tries to get an entity with the specific id.
            If the entity is no longer among us, return null.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### TryGetUniqueEntity()
```csharp
public Entity TryGetUniqueEntity()
```

Try to get a unique entity that owns <typeparamref name="T" />.

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

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
`filter` [ContextAccessorFilter](/Bang/Contexts/ContextAccessorFilter.html) \
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
[T]() \

#### TryGetUnique()
```csharp
public T? TryGetUnique()
```

Try to get a unique entity that owns <typeparamref name="T" />.

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
The unique component .\

#### FixedUpdate()
```csharp
public ValueTask FixedUpdate()
```

Calls update on all [IFixedUpdateSystem](/Bang/Systems/IFixedUpdateSystem.html) systems.
            This will be called on fixed intervals.

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Start()
```csharp
public ValueTask Start()
```

Call start on all systems.
            This is called before any updates and will notify any reactive systems by the end of it.

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Update()
```csharp
public ValueTask Update()
```

Calls update on all [IUpdateSystem](/Bang/Systems/IUpdateSystem.html) systems.
            At the end of update, it will notify all reactive systems of any changes made to entities
            they were watching.
            Finally, it destroys all pending entities and clear all messages.

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Pause()
```csharp
public virtual void Pause()
```

Pause all the set of systems that qualify in [World.IsPauseSystem(Bang.Systems.ISystem)](/Bang/World.html).
            A paused system will no longer be called on any [World.Update](/bang/world.html#update) calls.

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



⚡