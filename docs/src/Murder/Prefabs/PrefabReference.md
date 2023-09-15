# PrefabReference

**Namespace:** Murder.Prefabs \
**Assembly:** Murder.dll

```csharp
public sealed struct PrefabReference
```

Represents an entity placed on the map.

### ⭐ Constructors
```csharp
public PrefabReference(Guid guid)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### CanFetch
```csharp
public bool CanFetch { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Guid
```csharp
public readonly Guid Guid;
```

Reference to a [PrefabAsset](../../Murder/Assets/PrefabAsset.html).

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
### ⭐ Methods
#### Fetch()
```csharp
public PrefabAsset Fetch()
```

**Returns** \
[PrefabAsset](../../Murder/Assets/PrefabAsset.html) \



⚡