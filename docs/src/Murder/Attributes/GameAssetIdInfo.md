# GameAssetIdInfo

**Namespace:** Murder.Attributes \
**Assembly:** Murder.dll

```csharp
public sealed struct GameAssetIdInfo
```

### ⭐ Constructors
```csharp
public GameAssetIdInfo(Type t, bool allowInheritance)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
`allowInheritance` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### AllowInheritance
```csharp
public readonly bool AllowInheritance;
```

Whether it should look for all assets that inherit from this asset.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### AssetType
```csharp
public readonly Type AssetType;
```

The type of the game asset.

**Returns** \
[Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \


⚡