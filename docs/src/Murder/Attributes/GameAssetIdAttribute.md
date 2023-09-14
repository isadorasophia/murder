# GameAssetIdAttribute

**Namespace:** Murder.Attributes \
**Assembly:** Murder.dll

```csharp
public class GameAssetIdAttribute : Attribute
```

This is an attribute used for a field guid that point to a game asset id.

**Implements:** _[Attribute](https://learn.microsoft.com/en-us/dotnet/api/System.Attribute?view=net-7.0)_

### ⭐ Constructors
```csharp
public GameAssetIdAttribute(Type type, bool allowInheritance)
```

Creates a new [GameAssetIdAttribute](../../Murder/Attributes/GameAssetIdAttribute.html).

**Parameters** \
`type` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\
`allowInheritance` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

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
#### TypeId
```csharp
public virtual Object TypeId { get; }
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡