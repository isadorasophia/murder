# ParameterId

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public sealed struct ParameterId : IEqualityComparer<T>, IEquatable<T>
```

**Implements:** _[IEqualityComparer\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEqualityComparer-1?view=net-7.0), [IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public ParameterId()
```

### ⭐ Properties
#### Data1
```csharp
public uint Data1 { get; public set; }
```

**Returns** \
[uint](https://learn.microsoft.com/en-us/dotnet/api/System.UInt32?view=net-7.0) \
#### Data2
```csharp
public uint Data2 { get; public set; }
```

**Returns** \
[uint](https://learn.microsoft.com/en-us/dotnet/api/System.UInt32?view=net-7.0) \
#### EditorName
```csharp
public string EditorName { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### IsGlobal
```csharp
public bool IsGlobal { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsGuidEmpty
```csharp
public bool IsGuidEmpty { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Name
```csharp
public string Name { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Owner
```csharp
public T? Owner { get; public set; }
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
### ⭐ Methods
#### WithPath(string)
```csharp
public ParameterId WithPath(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[ParameterId](../../../Murder/Core/Sounds/ParameterId.html) \

#### Equals(ParameterId)
```csharp
public virtual bool Equals(ParameterId other)
```

**Parameters** \
`other` [ParameterId](../../../Murder/Core/Sounds/ParameterId.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(ParameterId, ParameterId)
```csharp
public virtual bool Equals(ParameterId x, ParameterId y)
```

**Parameters** \
`x` [ParameterId](../../../Murder/Core/Sounds/ParameterId.html) \
`y` [ParameterId](../../../Murder/Core/Sounds/ParameterId.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetHashCode(ParameterId)
```csharp
public virtual int GetHashCode(ParameterId obj)
```

**Parameters** \
`obj` [ParameterId](../../../Murder/Core/Sounds/ParameterId.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡