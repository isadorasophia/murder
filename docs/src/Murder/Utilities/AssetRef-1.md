# AssetRef\<T\>

**Namespace:** Murder.Utilities \
**Assembly:** Murder.dll

```csharp
public sealed struct AssetRef<T>
```

### ⭐ Constructors
```csharp
public AssetRef<T>(Guid guid)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### Asset
```csharp
public T Asset { get; }
```

**Returns** \
[T](../../) \
#### Empty
```csharp
public static AssetRef<T> Empty { get; }
```

**Returns** \
[AssetRef\<T\>](../../Murder/Utilities/AssetRef-1.html) \
#### Guid
```csharp
public readonly Guid Guid;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### HasValue
```csharp
public bool HasValue { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TryAsset
```csharp
public T TryAsset { get; }
```

**Returns** \
[T](../../) \


⚡