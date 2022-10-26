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
#### CustomPath
```csharp
public virtual string CustomPath { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
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
#### Name
```csharp
public string Name { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
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
### ⭐ Methods
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