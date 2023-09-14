# PrefabEntityInstance

**Namespace:** Murder.Prefabs \
**Assembly:** Murder.dll

```csharp
public class PrefabEntityInstance : EntityInstance, IEntity
```

**Implements:** _[EntityInstance](../..//Murder/Prefabs/EntityInstance.html), [IEntity](../..//Murder/Prefabs/IEntity.html)_

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

**Returns** \
[Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
#### ActivateWithParent
```csharp
public bool ActivateWithParent;
```

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

Returns all the components of the entity asset, followed by all the components of the instance.

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

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### IsDeactivated
```csharp
public bool IsDeactivated;
```

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
#### PrefabRef
```csharp
public readonly PrefabReference PrefabRef;
```

This is the guid of the [PrefabAsset](../..//Murder/Assets/PrefabAsset.html) that this refers to.

**Returns** \
[PrefabReference](../..//Murder/Prefabs/PrefabReference.html) \
#### PrefabRefName
```csharp
public virtual string PrefabRefName { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### CanRevertComponentForChild(Guid, Type)
```csharp
public bool CanRevertComponentForChild(Guid guid, Type t)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### FetchChildChildren(IEntity)
```csharp
public ImmutableArray<T> FetchChildChildren(IEntity child)
```

Get all the children of a child.
            This will take into account any modifiers of the parent.

**Parameters** \
`child` [IEntity](../..//Murder/Prefabs/IEntity.html) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### CreateChildrenlessInstance(Guid)
```csharp
public PrefabEntityInstance CreateChildrenlessInstance(Guid assetGuid)
```

**Parameters** \
`assetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[PrefabEntityInstance](../..//Murder/Prefabs/PrefabEntityInstance.html) \

#### AddOrReplaceComponentForChild(Guid, IComponent)
```csharp
public virtual bool AddOrReplaceComponentForChild(Guid instance, IComponent component)
```

**Parameters** \
`instance` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`component` [IComponent](../..//Bang/Components/IComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CanModifyChildAt(Guid)
```csharp
public virtual bool CanModifyChildAt(Guid childId)
```

This checks whether a child can be modified.
            This means that it does not belong to any prefab reference.

**Parameters** \
`childId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

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
public virtual bool HasComponentAtChild(Guid instance, Type type)
```

**Parameters** \
`instance` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsComponentInAsset(IComponent)
```csharp
public virtual bool IsComponentInAsset(IComponent c)
```

Returns whether a component is present in the entity asset.

**Parameters** \
`c` [IComponent](../..//Bang/Components/IComponent.html) \

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
`instance` [EntityInstance&](../..//Murder/Prefabs/EntityInstance.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetChild(Guid)
```csharp
public virtual EntityInstance GetChild(Guid instanceGuid)
```

**Parameters** \
`instanceGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[EntityInstance](../..//Murder/Prefabs/EntityInstance.html) \

#### GetComponent(Type)
```csharp
public virtual IComponent GetComponent(Type componentType)
```

**Parameters** \
`componentType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[IComponent](../..//Bang/Components/IComponent.html) \

#### TryGetComponentForChild(Guid, Type)
```csharp
public virtual IComponent TryGetComponentForChild(Guid guid, Type t)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[IComponent](../..//Bang/Components/IComponent.html) \

#### FetchChildren()
```csharp
public virtual ImmutableArray<T> FetchChildren()
```

Returns all the children of the entity asset, followed by all the children of the instance.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetChildComponents(Guid)
```csharp
public virtual ImmutableArray<T> GetChildComponents(Guid guid)
```

Fetch the components for a given child.
            This will filter any modifiers made to the children components.

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### Create(World, IEntity)
```csharp
public virtual int Create(World world, IEntity parent)
```

**Parameters** \
`world` [World](../..//Bang/World.html) \
`parent` [IEntity](../..//Murder/Prefabs/IEntity.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Create(World)
```csharp
public virtual int Create(World world)
```

Create the instance entity in the world.

**Parameters** \
`world` [World](../..//Bang/World.html) \
\

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### AddChild(EntityInstance)
```csharp
public virtual void AddChild(EntityInstance asset)
```

**Parameters** \
`asset` [EntityInstance](../..//Murder/Prefabs/EntityInstance.html) \

#### AddChildAtChild(Guid, EntityInstance)
```csharp
public virtual void AddChildAtChild(Guid childId, EntityInstance instance)
```

**Parameters** \
`childId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`instance` [EntityInstance](../..//Murder/Prefabs/EntityInstance.html) \

#### AddOrReplaceComponent(IComponent)
```csharp
public virtual void AddOrReplaceComponent(IComponent c)
```

**Parameters** \
`c` [IComponent](../..//Bang/Components/IComponent.html) \

#### RemoveChildAtChild(Guid, Guid)
```csharp
public virtual void RemoveChildAtChild(Guid childId, Guid instance)
```

**Parameters** \
`childId` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`instance` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

#### RemoveComponentForChild(Guid, Type)
```csharp
public virtual void RemoveComponentForChild(Guid instance, Type t)
```

**Parameters** \
`instance` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

#### SetName(string)
```csharp
public virtual void SetName(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡