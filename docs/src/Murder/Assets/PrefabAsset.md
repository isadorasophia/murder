# PrefabAsset

**Namespace:** Murder.Assets \
**Assembly:** Murder.dll

```csharp
public class PrefabAsset : GameAsset, IEntity
```

**Implements:** _[GameAsset](../..//Murder/Assets/GameAsset.html), [IEntity](../..//Murder/Prefabs/IEntity.html)_

### ⭐ Constructors
```csharp
public PrefabAsset()
```

```csharp
public PrefabAsset(EntityInstance instance)
```

**Parameters** \
`instance` [EntityInstance](../..//Murder/Prefabs/EntityInstance.html) \

### ⭐ Properties
#### CanBeCreated
```csharp
public virtual bool CanBeCreated { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeDeleted
```csharp
public virtual bool CanBeDeleted { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeRenamed
```csharp
public virtual bool CanBeRenamed { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### CanBeSaved
```csharp
public virtual bool CanBeSaved { get; }
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

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Dimensions
```csharp
public readonly TileDimensions Dimensions;
```

Dimensions of the prefab. Used when drawing it on the map or the editor.

**Returns** \
[TileDimensions](../..//Murder/Core/TileDimensions.html) \
#### EditorColor
```csharp
public virtual Vector4 EditorColor { get; }
```

**Returns** \
[Vector4](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector4?view=net-7.0) \
#### EditorFolder
```csharp
public virtual string EditorFolder { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### FileChanged
```csharp
public bool FileChanged { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### FilePath
```csharp
public string FilePath { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Guid
```csharp
public Guid Guid { get; protected set; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### Icon
```csharp
public virtual char Icon { get; }
```

**Returns** \
[char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
#### IsStoredInSaveData
```csharp
public virtual bool IsStoredInSaveData { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Name
```csharp
public string Name { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### PrefabRefName
```csharp
public virtual string PrefabRefName { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Rename
```csharp
public bool Rename { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### SaveLocation
```csharp
public virtual string SaveLocation { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ShowOnPrefabSelector
```csharp
public bool ShowOnPrefabSelector;
```

Whether this should show in the editor selector.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### StoreInDatabase
```csharp
public virtual bool StoreInDatabase { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TaggedForDeletion
```csharp
public bool TaggedForDeletion;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### OnModified()
```csharp
protected virtual void OnModified()
```

#### HasComponent(IComponent)
```csharp
public bool HasComponent(IComponent c)
```

**Parameters** \
`c` [IComponent](../..//Bang/Components/IComponent.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CreateAndFetch(World)
```csharp
public Entity CreateAndFetch(World world)
```

**Parameters** \
`world` [World](../..//Bang/World.html) \

**Returns** \
[Entity](../..//Bang/Entities/Entity.html) \

#### ToInstance(string)
```csharp
public EntityInstance ToInstance(string name)
```

Creates a new instance entity from the current asset.

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[EntityInstance](../..//Murder/Prefabs/EntityInstance.html) \

#### Duplicate(string)
```csharp
public GameAsset Duplicate(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[GameAsset](../..//Murder/Assets/GameAsset.html) \

#### ToInstanceAsAsset(string)
```csharp
public PrefabAsset ToInstanceAsAsset(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[PrefabAsset](../..//Murder/Assets/PrefabAsset.html) \

#### GetSimplifiedName()
```csharp
public string GetSimplifiedName()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetSplitNameWithEditorPath()
```csharp
public String[] GetSplitNameWithEditorPath()
```

**Returns** \
[string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### AddOrReplaceComponentForChild(Guid, IComponent)
```csharp
public virtual bool AddOrReplaceComponentForChild(Guid childGuid, IComponent component)
```

**Parameters** \
`childGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`component` [IComponent](../..//Bang/Components/IComponent.html) \

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

#### RemoveChild(Guid)
```csharp
public virtual bool RemoveChild(Guid instanceGuid)
```

Add an entity asset as a children of the current asset.
            Each of the children will be an instance of the current asset.

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

#### GetComponent(Type)
```csharp
public virtual IComponent GetComponent(Type type)
```

**Parameters** \
`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

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

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### GetChildComponents(Guid)
```csharp
public virtual ImmutableArray<T> GetChildComponents(Guid childGuid)
```

**Parameters** \
`childGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### Create(World)
```csharp
public virtual int Create(World world)
```

Create an instance of the entity and all of its children.

**Parameters** \
`world` [World](../..//Bang/World.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### AddChild(EntityInstance)
```csharp
public virtual void AddChild(EntityInstance asset)
```

Add an entity asset as a children of the current asset.
            Each of the children will be an instance of the current asset.

**Parameters** \
`asset` [EntityInstance](../..//Murder/Prefabs/EntityInstance.html) \

#### AddOrReplaceComponent(IComponent)
```csharp
public virtual void AddOrReplaceComponent(IComponent c)
```

**Parameters** \
`c` [IComponent](../..//Bang/Components/IComponent.html) \

#### AfterDeserialized()
```csharp
public virtual void AfterDeserialized()
```

#### RemoveComponentForChild(Guid, Type)
```csharp
public virtual void RemoveComponentForChild(Guid childGuid, Type t)
```

**Parameters** \
`childGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

#### MakeGuid()
```csharp
public void MakeGuid()
```

#### Replace(World, Entity, IComponent[])
```csharp
public void Replace(World world, Entity e, IComponent[] startWithComponents)
```

This will replace an existing entity in the world.
            It keeps some elements of the original entity: position and target id components.

**Parameters** \
`world` [World](../..//Bang/World.html) \
\
`e` [Entity](../..//Bang/Entities/Entity.html) \
\
`startWithComponents` [IComponent[]](../..//Bang/Components/IComponent.html) \
\

#### Replace(World, Entity)
```csharp
public void Replace(World world, Entity e)
```

This will replace an existing entity in the world.
            It keeps some elements of the original entity: position and target id components.

**Parameters** \
`world` [World](../..//Bang/World.html) \
`e` [Entity](../..//Bang/Entities/Entity.html) \



⚡