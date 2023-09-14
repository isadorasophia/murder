# MonoWorld

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public class MonoWorld : World, IDisposable
```

World implementation based in MonoGame.

**Implements:** _[World](../../Bang/World.html), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public MonoWorld(IList<T> systems, Camera2D camera, Guid worldAssetGuid)
```

**Parameters** \
`systems` [IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \
`camera` [Camera2D](../../Murder/Core/Graphics/Camera2D.html) \
`worldAssetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### _cachedRenderSystems
```csharp
protected readonly SortedList<TKey, TValue> _cachedRenderSystems;
```

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
#### Camera
```csharp
public readonly Camera2D Camera;
```

**Returns** \
[Camera2D](../../Murder/Core/Graphics/Camera2D.html) \
#### Contexts
```csharp
protected readonly Dictionary<TKey, TValue> Contexts;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
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

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### GuiCounters
```csharp
public readonly Dictionary<TKey, TValue> GuiCounters;
```

This has the duration of each gui render system (id) to its corresponding time (in ms).
            See [World.IdToSystem](../../Bang/World.html#IdToSystem) on how to fetch the actual system.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### IdToSystem
```csharp
public readonly ImmutableDictionary<TKey, TValue> IdToSystem;
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### IsPaused
```csharp
public bool IsPaused { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### PreRenderCounters
```csharp
public readonly Dictionary<TKey, TValue> PreRenderCounters;
```

This has the duration of each reactive system (id) to its corresponding time (in ms).
            See [World.IdToSystem](../../Bang/World.html#IdToSystem) on how to fetch the actual system.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### ReactiveCounters
```csharp
public readonly Dictionary<TKey, TValue> ReactiveCounters;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### RenderCounters
```csharp
public readonly Dictionary<TKey, TValue> RenderCounters;
```

This has the duration of each render system (id) to its corresponding time (in ms).
            See [World.IdToSystem](../../Bang/World.html#IdToSystem) on how to fetch the actual system.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### UpdateCounters
```csharp
public readonly Dictionary<TKey, TValue> UpdateCounters;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### WorldAssetGuid
```csharp
public readonly Guid WorldAssetGuid;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
### ⭐ Methods
#### ClearDiagnosticsCountersForSystem(int)
```csharp
protected virtual void ClearDiagnosticsCountersForSystem(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### InitializeDiagnosticsForSystem(int, ISystem)
```csharp
protected virtual void InitializeDiagnosticsForSystem(int systemId, ISystem system)
```

**Parameters** \
`systemId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`system` [ISystem](../../Bang/Systems/ISystem.html) \

#### InitializeDiagnosticsCounters()
```csharp
protected void InitializeDiagnosticsCounters()
```

#### ActivateSystem()
```csharp
public bool ActivateSystem()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ActivateSystem(Type)
```csharp
public bool ActivateSystem(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeactivateSystem()
```csharp
public bool DeactivateSystem()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeactivateSystem(int, bool)
```csharp
public bool DeactivateSystem(int id, bool immediately)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`immediately` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeactivateSystem(Type)
```csharp
public bool DeactivateSystem(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsSystemActive(Type)
```csharp
public bool IsSystemActive(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### AddEntity()
```csharp
public Entity AddEntity()
```

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### AddEntity(IComponent[])
```csharp
public Entity AddEntity(IComponent[] components)
```

**Parameters** \
`components` [IComponent[]](../../Bang/Components/IComponent.html) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### AddEntity(T?, IComponent[])
```csharp
public Entity AddEntity(T? id, IComponent[] components)
```

**Parameters** \
`id` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`components` [IComponent[]](../../Bang/Components/IComponent.html) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### GetEntity(int)
```csharp
public Entity GetEntity(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### GetUniqueEntity()
```csharp
public Entity GetUniqueEntity()
```

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### TryGetEntity(int)
```csharp
public Entity TryGetEntity(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### TryGetUniqueEntity()
```csharp
public Entity TryGetUniqueEntity()
```

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### GetAllEntities()
```csharp
public ImmutableArray<T> GetAllEntities()
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetEntitiesWith(ContextAccessorFilter, Type[])
```csharp
public ImmutableArray<T> GetEntitiesWith(ContextAccessorFilter filter, Type[] components)
```

**Parameters** \
`filter` [ContextAccessorFilter](../../Bang/Contexts/ContextAccessorFilter.html) \
`components` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetEntitiesWith(Type[])
```csharp
public ImmutableArray<T> GetEntitiesWith(Type[] components)
```

**Parameters** \
`components` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetUnique()
```csharp
public T GetUnique()
```

**Returns** \
[T](../../) \

#### TryGetUnique()
```csharp
public T? TryGetUnique()
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Dispose()
```csharp
public virtual void Dispose()
```

#### Pause()
```csharp
public virtual void Pause()
```

#### Resume()
```csharp
public virtual void Resume()
```

#### ActivateAllSystems()
```csharp
public void ActivateAllSystems()
```

#### DeactivateAllSystems(Type[])
```csharp
public void DeactivateAllSystems(Type[] skip)
```

**Parameters** \
`skip` [Type[]](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

#### Draw(RenderContext)
```csharp
public void Draw(RenderContext render)
```

**Parameters** \
`render` [RenderContext](../../Murder/Core/Graphics/RenderContext.html) \

#### DrawGui(RenderContext)
```csharp
public void DrawGui(RenderContext render)
```

**Parameters** \
`render` [RenderContext](../../Murder/Core/Graphics/RenderContext.html) \

#### Exit()
```csharp
public void Exit()
```

#### FixedUpdate()
```csharp
public void FixedUpdate()
```

#### PreDraw()
```csharp
public void PreDraw()
```

#### Start()
```csharp
public void Start()
```

#### Update()
```csharp
public void Update()
```



⚡