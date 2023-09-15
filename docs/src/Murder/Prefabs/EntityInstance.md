# EntityInstance

**Namespace:** Murder.Prefabs \
**Assembly:** Murder.dll

```csharp
public class EntityInstance : IEntity
```

Represents an entity as an instance placed on the map.
            This map may be relative to the world or another entity.

**Implements:** _[IEntity](../../Murder/Prefabs/IEntity.html)_

### ⭐ Constructors
```csharp
public EntityInstance()
```

```csharp
public EntityInstance(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### _children
```csharp
protected Dictionary<TKey, TValue> _children;
```

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### _components
```csharp
protected readonly Dictionary<TKey, TValue> _components;
```

List of custom components that difer from the parent entity.

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### ActivateWithParent
```csharp
public bool ActivateWithParent;
```

Whether this instance must have its activation propagated according to the parent. 
            <br /><br />
            TODO: We might need to revisit on whether this is okay/actually scales well.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Children
```csharp
public virtual ImmutableArray<T> Children { get; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Components
```csharp
public virtual ImmutableArray<T> Components { get; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Guid
```csharp
public virtual Guid Guid { get; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Id
```csharp
public T? Id;
```

Entity id, if any. This will be persisted across save files.
            This only exists for instances in the world.

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### IsDeactivated
```csharp
public bool IsDeactivated;
```

Returns whether the entity is currently deactivated once instantiated in the map.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsEmpty
```csharp
public virtual bool IsEmpty { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Name
```csharp
public virtual string Name { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### PrefabRefName
```csharp
public virtual string PrefabRefName { get; }
```

By default, this is not based on any prefab.
            Return null.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### AddOrReplaceComponentForChild(Guid, IComponent)
```csharp
public virtual bool AddOrReplaceComponentForChild(Guid childGuid, IComponent component)
```

**Parameters** \
`childGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`component` [IComponent](../../Bang/Components/IComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CanRemoveChild(Guid)
```csharp
public virtual bool CanRemoveChild(Guid instanceGuid)
```

**Parameters** \
`instanceGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CanRevertComponent(Type)
```csharp
public virtual bool CanRevertComponent(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasComponent(Type)
```csharp
public virtual bool HasComponent(Type type)
```

Returns whether an instance of <paramref name="type" /> exists in the list of components.

**Parameters** \
`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasComponentAtChild(Guid, Type)
```csharp
public virtual bool HasComponentAtChild(Guid childGuid, Type type)
```

**Parameters** \
`childGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsComponentInAsset(IComponent)
```csharp
public virtual bool IsComponentInAsset(IComponent c)
```

Returns whether a component is present in the entity asset.

**Parameters** \
`c` [IComponent](../../Bang/Components/IComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveChild(Guid)
```csharp
public virtual bool RemoveChild(Guid instanceGuid)
```

**Parameters** \
`instanceGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RemoveComponent(Type)
```csharp
public virtual bool RemoveComponent(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RevertComponent(Type)
```csharp
public virtual bool RevertComponent(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RevertComponentForChild(Guid, Type)
```csharp
public virtual bool RevertComponentForChild(Guid childGuid, Type t)
```

**Parameters** \
`childGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### TryGetChild(Guid, out EntityInstance&)
```csharp
public virtual bool TryGetChild(Guid guid, EntityInstance& instance)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`instance` [EntityInstance&](../../Murder/Prefabs/EntityInstance.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetChild(Guid)
```csharp
public virtual EntityInstance GetChild(Guid instanceGuid)
```

**Parameters** \
`instanceGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[EntityInstance](../../Murder/Prefabs/EntityInstance.html) \

#### GetComponent(Type)
```csharp
public virtual IComponent GetComponent(Type componentType)
```

**Parameters** \
`componentType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[IComponent](../../Bang/Components/IComponent.html) \

#### TryGetComponentForChild(Guid, Type)
```csharp
public virtual IComponent TryGetComponentForChild(Guid guid, Type t)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[IComponent](../../Bang/Components/IComponent.html) \

#### FetchChildren()
```csharp
public virtual ImmutableArray<T> FetchChildren()
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetChildComponents(Guid)
```csharp
public virtual ImmutableArray<T> GetChildComponents(Guid guid)
```

Try to get the components for a child.
            TODO: Do not expose the instance children directly...? Is this only necessary for prefabs?
            Are we limiting the amount of children recursive to two?

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
\

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### Create(World, IEntity)
```csharp
public virtual int Create(World world, IEntity parent)
```

Create the instance entity in the world with a specified parent.

**Parameters** \
`world` [World](../../Bang/World.html) \
\
`parent` [IEntity](../../Murder/Prefabs/IEntity.html) \
\

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Create(World)
```csharp
public virtual int Create(World world)
```

Create the instance entity in the world.

**Parameters** \
`world` [World](../../Bang/World.html) \
\

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### AddChild(EntityInstance)
```csharp
public virtual void AddChild(EntityInstance asset)
```

**Parameters** \
`asset` [EntityInstance](../../Murder/Prefabs/EntityInstance.html) \

#### AddOrReplaceComponent(IComponent)
```csharp
public virtual void AddOrReplaceComponent(IComponent c)
```

**Parameters** \
`c` [IComponent](../../Bang/Components/IComponent.html) \

#### RemoveComponentForChild(Guid, Type)
```csharp
public virtual void RemoveComponentForChild(Guid childGuid, Type t)
```

**Parameters** \
`childGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

#### SetName(string)
```csharp
public virtual void SetName(string name)
```

Set the name of the entity instance.

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡