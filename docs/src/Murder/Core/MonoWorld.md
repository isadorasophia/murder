# MonoWorld

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public class MonoWorld : World
```

World implementation based in MonoGame.

**Implements:** _[World](/Bang/World.html)_

### ⭐ Constructors
```csharp
public MonoWorld(IList<T> systems, Camera2D camera, Guid worldAssetGuid)
```

**Parameters** \
`systems` [IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \
`camera` [Camera2D](/Murder/Core/Graphics/Camera2D.html) \
`worldAssetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### _cachedRenderSystems
```csharp
protected readonly SortedList<TKey, TValue> _cachedRenderSystems;
```

**Returns** \
[SortedList\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.SortedList-2?view=net-7.0) \
#### Camera
```csharp
public readonly Camera2D Camera;
```

**Returns** \
[Camera2D](/Murder/Core/Graphics/Camera2D.html) \
#### Contexts
```csharp
protected readonly Dictionary<TKey, TValue> Contexts;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### IsPaused
```csharp
public bool IsPaused { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### WorldAssetGuid
```csharp
public readonly Guid WorldAssetGuid;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
### ⭐ Methods
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

#### DeactivateSystem(int)
```csharp
public bool DeactivateSystem(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

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

#### AddEntity()
```csharp
public Entity AddEntity()
```

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### AddEntity(IComponent[])
```csharp
public Entity AddEntity(IComponent[] components)
```

**Parameters** \
`components` [IComponent[]](/Bang/Components/IComponent.html) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### AddEntity(IEnumerable<T>)
```csharp
public Entity AddEntity(IEnumerable<T> components)
```

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

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### GetUniqueEntity()
```csharp
public Entity GetUniqueEntity()
```

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### TryGetEntity(int)
```csharp
public Entity TryGetEntity(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

#### TryGetUniqueEntity()
```csharp
public Entity TryGetUniqueEntity()
```

**Returns** \
[Entity](/Bang/Entities/Entity.html) \

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
`filter` [ContextAccessorFilter](/Bang/Contexts/ContextAccessorFilter.html) \
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
[T]() \

#### TryGetUnique()
```csharp
public T? TryGetUnique()
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Draw(RenderContext)
```csharp
public ValueTask Draw(RenderContext render)
```

**Parameters** \
`render` [RenderContext](/Murder/Core/Graphics/RenderContext.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### DrawGui(RenderContext)
```csharp
public ValueTask DrawGui(RenderContext render)
```

**Parameters** \
`render` [RenderContext](/Murder/Core/Graphics/RenderContext.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### FixedUpdate()
```csharp
public ValueTask FixedUpdate()
```

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### PreDraw()
```csharp
public ValueTask PreDraw()
```

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Start()
```csharp
public ValueTask Start()
```

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Update()
```csharp
public ValueTask Update()
```

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

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



⚡