# WorldAsset

**Namespace:** Murder.Assets \
**Assembly:** Murder.dll

```csharp
public class WorldAsset : GameAsset, IWorldAsset
```

**Implements:** _[GameAsset](/Murder/Assets/GameAsset.html), [IWorldAsset](/Murder/Assets/IWorldAsset.html)_

### ⭐ Constructors
```csharp
public WorldAsset()
```

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
#### Features
```csharp
public ImmutableArray<T> Features { get; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### FileChanged
```csharp
public bool FileChanged;
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
#### HasSystems
```csharp
public bool HasSystems { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Icon
```csharp
public virtual char Icon { get; }
```

**Returns** \
[char](https://learn.microsoft.com/en-us/dotnet/api/System.Char?view=net-7.0) \
#### Instances
```csharp
public virtual ImmutableArray<T> Instances { get; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
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
#### Order
```csharp
public readonly int Order;
```

This is the order in which this world will be displayed in game (when selecting a lvel, etc.)

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
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
#### StoreInDatabase
```csharp
public virtual bool StoreInDatabase { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Systems
```csharp
public ImmutableArray<T> Systems { get; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### TaggedForDeletion
```csharp
public bool TaggedForDeletion;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### WorldGuid
```csharp
public virtual Guid WorldGuid { get; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### WorldName
```csharp
public readonly string WorldName;
```

This is the world name used when fetching this world within the game.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### PostProcessEntities(World, Dictionary<TKey, TValue>)
```csharp
protected void PostProcessEntities(World world, Dictionary<TKey, TValue> instancesToEntities)
```

This makes any fancy post process once all entities were created in the world.
            This may trigger reactive components within the world.

**Parameters** \
`world` [World](/Bang/World.html) \
\
`instancesToEntities` [Dictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.Dictionary-2?view=net-7.0) \
\

#### AddGroup(string)
```csharp
public bool AddGroup(string name)
```

Add a new folder to group entities.

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### BelongsToAnyGroup(Guid)
```csharp
public bool BelongsToAnyGroup(Guid entity)
```

Checks whether an entity belongs to any group.

**Parameters** \
`entity` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DeleteGroup(string)
```csharp
public bool DeleteGroup(string name)
```

Delete a new folder to group entities.

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HasGroup(string)
```csharp
public bool HasGroup(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### MoveToGroup(string, Guid, int)
```csharp
public bool MoveToGroup(string targetGroup, Guid instance, int targetPosition)
```

**Parameters** \
`targetGroup` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`instance` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`targetPosition` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Duplicate(string)
```csharp
public GameAsset Duplicate(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[GameAsset](/Murder/Assets/GameAsset.html) \

#### FetchAllSystems()
```csharp
public ImmutableArray<T> FetchAllSystems()
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### FetchEntitiesOfGroup(string)
```csharp
public ImmutableArray<T> FetchEntitiesOfGroup(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### FetchFolders()
```csharp
public ImmutableDictionary<TKey, TValue> FetchFolders()
```

This is for editor purposes, we group all entities in "folders" when visualizing them.
            This has no effect in the actual game.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

#### GroupsCount()
```csharp
public int GroupsCount()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### CreateInstance(Camera2D)
```csharp
public MonoWorld CreateInstance(Camera2D camera)
```

**Parameters** \
`camera` [Camera2D](/Murder/Core/Graphics/Camera2D.html) \

**Returns** \
[MonoWorld](/Murder/Core/MonoWorld.html) \

#### CreateInstanceFromSave(SavedWorld, Camera2D)
```csharp
public MonoWorld CreateInstanceFromSave(SavedWorld savedInstance, Camera2D camera)
```

**Parameters** \
`savedInstance` [SavedWorld](/Murder/Assets/SavedWorld.html) \
`camera` [Camera2D](/Murder/Core/Graphics/Camera2D.html) \

**Returns** \
[MonoWorld](/Murder/Core/MonoWorld.html) \

#### GetGroupOf(Guid)
```csharp
public string GetGroupOf(Guid entity)
```

Returns the group that an entity belongs.

**Parameters** \
`entity` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetSimplifiedName()
```csharp
public string GetSimplifiedName()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### TryGetInstance(Guid)
```csharp
public virtual EntityInstance TryGetInstance(Guid instanceGuid)
```

**Parameters** \
`instanceGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[EntityInstance](/Murder/Prefabs/EntityInstance.html) \

#### AddInstance(EntityInstance)
```csharp
public void AddInstance(EntityInstance e)
```

**Parameters** \
`e` [EntityInstance](/Murder/Prefabs/EntityInstance.html) \

#### MakeGuid()
```csharp
public void MakeGuid()
```

#### RemoveInstance(Guid)
```csharp
public void RemoveInstance(Guid instanceGuid)
```

**Parameters** \
`instanceGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

#### UpdateFeatures(ImmutableArray<T>)
```csharp
public void UpdateFeatures(ImmutableArray<T> features)
```

**Parameters** \
`features` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### UpdateSystems(ImmutableArray<T>)
```csharp
public void UpdateSystems(ImmutableArray<T> systems)
```

**Parameters** \
`systems` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

#### ValidateInstances()
```csharp
public void ValidateInstances()
```

Validate instances are remove any entities that no longer exist in the asset.



⚡