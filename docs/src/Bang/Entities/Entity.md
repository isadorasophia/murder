# Entity

**Namespace:** Bang.Entities \
**Assembly:** Bang.dll

```csharp
public class Entity : IDisposable
```

An entity is a collection of components within the world.
            This supports hierarchy (parent, children).

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Properties
#### Children
```csharp
public ImmutableArray<T> Children { get; }
```

Unique id of all the children of the entity.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Components
```csharp
public ImmutableArray<T> Components { get; }
```

This is used for editor and serialization.
            TODO: Optimize this. For now, this is okay since it's only used once the entity is serialized.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### EntityId
```csharp
public int EntityId { get; }
```

Entity unique identifier.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### FetchChildrenWithNames
```csharp
public ImmutableDictionary<TKey, TValue> FetchChildrenWithNames { get; }
```

Fetch a list of all the unique identifiers of the children with their respective names.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### IsActive
```csharp
public bool IsActive { get; }
```

Whether this entity is active or not.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsDeactivated
```csharp
public bool IsDeactivated { get; private set; }
```

Whether this entity has been deactivated or not.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsDestroyed
```csharp
public bool IsDestroyed { get; private set; }
```

Whether this entity has been destroyed (and probably recycled) or not.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Parent
```csharp
public T? Parent { get; }
```

This is the unique id of the parent of the entity.
            Null if none (no parent).

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
### ⭐ Events
#### OnComponentAdded
```csharp
public event Action<T1, T2> OnComponentAdded;
```

Fired whenever a new component is added.

**Returns** \
[Action\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-2?view=net-7.0) \
#### OnComponentModified
```csharp
public event Action<T1, T2> OnComponentModified;
```

Fired whenever any component is replaced.

**Returns** \
[Action\<T1, T2\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-2?view=net-7.0) \
#### OnComponentRemoved
```csharp
public event Action<T1, T2, T3> OnComponentRemoved;
```

Fired whenever a new component is removed.
            This will send the entity, the component id that was just removed and
            whether this was caused by a destroy.

**Returns** \
[Action\<T1, T2, T3\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-3?view=net-7.0) \
#### OnEntityActivated
```csharp
public event Action<T> OnEntityActivated;
```

Fired when the entity gets activated, so it gets filtered
            back in the context listeners.

**Returns** \
[Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \
#### OnEntityDeactivated
```csharp
public event Action<T> OnEntityDeactivated;
```

Fired when the entity gets deactivated, so it is filtered out
            from its context listeners.

**Returns** \
[Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \
#### OnEntityDestroyed
```csharp
public event Action<T> OnEntityDestroyed;
```

Fired when the entity gets destroyed.

**Returns** \
[Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \
#### OnMessage
```csharp
public event Action<T1, T2, T3> OnMessage;
```

This will be fired when a message gets sent to the entity.

**Returns** \
[Action\<T1, T2, T3\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-3?view=net-7.0) \
### ⭐ Methods
#### AddComponent(T, int)
```csharp
public bool AddComponent(T c, int index)
```

**Parameters** \
`c` [T](../../) \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### AddComponentOnce()
```csharp
public bool AddComponentOnce()
```

Add an empty component only once to the entity.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### HasChild(int)
```csharp
public bool HasChild(int entityId)
```

Try to fetch a child with a <paramref name="entityId" /> entity identifier.

**Parameters** \
`entityId` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasChild(string)
```csharp
public bool HasChild(string name)
```

Try to fetch a child with a <paramref name="name" /> identifier

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### HasComponent()
```csharp
public bool HasComponent()
```

Whether this entity has a component of type T.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasComponent(int)
```csharp
public bool HasComponent(int index)
```

Checks whether an entity has a component.

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasComponent(Type)
```csharp
public bool HasComponent(Type t)
```

Whether this entity has a component of type <paramref name="t" />.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasMessage()
```csharp
public bool HasMessage()
```

Whether entity has a message of type <typeparamref name="T" />.
            This should be avoided since it highly depends on the order of the systems
            being fired and can lead to several bugs.
            For example, if we check for that on the state machine, it will depend on the order
            of the entities in the world.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasMessage(int)
```csharp
public bool HasMessage(int index)
```

Whether entity has a message of index <paramref name="index" />.
            This should be avoided since it highly depends on the order of the systems
            being fired and can lead to several bugs.
            For example, if we check for that on the state machine, it will depend on the order
            of the entities in the world.

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsActivateWithParent()
```csharp
public bool IsActivateWithParent()
```

Whether this entity should be reactivated with the parent.
            This is used when serializing data and we might need to revisit this soon.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveChild(string)
```csharp
public bool RemoveChild(string name)
```

Remove a child from the entity.

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveComponent()
```csharp
public bool RemoveComponent()
```

Removes component of type <typeparamref name="T" />.
            Do nothing if <typeparamref name="T" /> is not owned by this entity.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveComponent(int)
```csharp
public bool RemoveComponent(int index)
```

Remove a component from the entity.
            Returns true if the element existed and was removed.

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ReplaceComponent(T, int, bool)
```csharp
public bool ReplaceComponent(T c, int index, bool forceReplace)
```

**Parameters** \
`c` [T](../../) \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`forceReplace` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetComponent(out T&)
```csharp
public bool TryGetComponent(T& component)
```

**Parameters** \
`component` [T&](../../) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### AddComponent(T)
```csharp
public Entity AddComponent(T c)
```

**Parameters** \
`c` [T](../../) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### TryFetchChild(int)
```csharp
public Entity TryFetchChild(int id)
```

Try to fetch a child with a <paramref name="id" /> identifier

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \
\

#### TryFetchChild(string)
```csharp
public Entity TryFetchChild(string name)
```

Try to fetch a child with a <paramref name="name" /> identifier

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \
\

#### TryFetchChildWithComponent()
```csharp
public Entity TryFetchChildWithComponent()
```

This fetches a child with a given component.
            TODO: Optimize, or cache?

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \

#### TryFetchParent()
```csharp
public Entity TryFetchParent()
```

Try to fetch the parent entity.

**Returns** \
[Entity](../../Bang/Entities/Entity.html) \
\

#### GetComponent()
```csharp
public T GetComponent()
```

Fetch a component of type T. If the entity does not have that component, this method will assert and fail.

**Returns** \
[T](../../) \

#### GetComponent(int)
```csharp
public T GetComponent(int index)
```

Fetch a component of type T with <paramref name="index" />. 
            If the entity does not have that component, this method will assert and fail.

**Parameters** \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[T](../../) \

#### TryGetComponent()
```csharp
public T? TryGetComponent()
```

Try to get a component of type T. If none, returns null.

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### Dispose()
```csharp
public virtual void Dispose()
```

Dispose the entity.
            This will unparent and remove all components.
            It also removes subscription from all their contexts or entities.

#### Activate()
```csharp
public void Activate()
```

Marks an entity as active if it isn't already.

#### AddChild(int, string)
```csharp
public void AddChild(int id, string name)
```

Assign an existing entity as a child.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\

#### AddComponent(IComponent, Type)
```csharp
public void AddComponent(IComponent c, Type t)
```

Add a component <paramref name="c" /> of type <paramref name="t" />.

**Parameters** \
`c` [IComponent](../../Bang/Components/IComponent.html) \
\
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\

#### AddOrReplaceComponent(IComponent, Type)
```csharp
public void AddOrReplaceComponent(IComponent c, Type t)
```

Add or replace component of type <paramref name="t" /> with <paramref name="c" />.
            Do nothing if the entity has been destroyed.

**Parameters** \
`c` [IComponent](../../Bang/Components/IComponent.html) \
\
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\

#### AddOrReplaceComponent(T, int)
```csharp
public void AddOrReplaceComponent(T c, int index)
```

**Parameters** \
`c` [T](../../) \
`index` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### AddOrReplaceComponent(T)
```csharp
public void AddOrReplaceComponent(T c)
```

**Parameters** \
`c` [T](../../) \

#### Deactivate()
```csharp
public void Deactivate()
```

Marks an entity as deactivated if it isn't already.

#### Destroy()
```csharp
public void Destroy()
```

Destroy the entity from the world.
            This will notify all components that it will be removed from the entity.
            At the end of the update of the frame, it will wipe this entity from the world.
            However, if someone still holds reference to an [Entity](../../Bang/Entities/Entity.html) (they shouldn't),
            they might see a zombie entity after this.

#### RemoveChild(int)
```csharp
public void RemoveChild(int id)
```

Remove a child from the entity.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### RemoveComponent(Type)
```csharp
public void RemoveComponent(Type t)
```

Removes component of type <paramref name="t" />.
            Do nothing if <paramref name="t" /> is not owned by this entity.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\

#### Reparent(Entity)
```csharp
public void Reparent(Entity parent)
```

Set the parent of this entity.

**Parameters** \
`parent` [Entity](../../Bang/Entities/Entity.html) \

#### Replace(IComponent[], List<T>, bool)
```csharp
public void Replace(IComponent[] components, List<T> children, bool wipe)
```

Replace all the components of the entity. This is useful when you want to reuse
            the same entity id with new components.

**Parameters** \
`components` [IComponent[]](../../Bang/Components/IComponent.html) \
\
`children` [List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \
\
`wipe` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### ReplaceComponent(IComponent, Type, bool)
```csharp
public void ReplaceComponent(IComponent c, Type t, bool forceReplace)
```

Replace componenent of type <paramref name="t" /> with <paramref name="c" />.
            This asserts if the component does not exist or is not assignable from <paramref name="t" />.
            Do nothing if the entity has been destroyed.

**Parameters** \
`c` [IComponent](../../Bang/Components/IComponent.html) \
\
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\
`forceReplace` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### ReplaceComponent(T)
```csharp
public void ReplaceComponent(T c)
```

**Parameters** \
`c` [T](../../) \

#### SendMessage()
```csharp
public void SendMessage()
```

Sends a message of type <typeparamref name="T" /> for any system watching it.

#### SendMessage(T)
```csharp
public void SendMessage(T message)
```

**Parameters** \
`message` [T](../../) \

#### SetActivateWithParent()
```csharp
public void SetActivateWithParent()
```

Force the entity to be activated and propagated according to the parent. Default is false (they are independent!)

#### Unparent()
```csharp
public void Unparent()
```

This will remove a parent of the entity.
            It untracks all the tracked components and removes itself from the parent's children.



⚡