# ParameterId

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public sealed struct ParameterId : IEqualityComparer<T>
```

**Implements:** _[IEqualityComparer\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEqualityComparer-1?view=net-7.0)_

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
### ⭐ Methods
#### WithPath(string)
```csharp
public ParameterId WithPath(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[ParameterId](/Murder/Core/Sounds/ParameterId.html) \

#### Equals(ParameterId, ParameterId)
```csharp
public virtual bool Equals(ParameterId x, ParameterId y)
```

**Parameters** \
`x` [ParameterId](/Murder/Core/Sounds/ParameterId.html) \
`y` [ParameterId](/Murder/Core/Sounds/ParameterId.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetHashCode(ParameterId)
```csharp
public virtual int GetHashCode(ParameterId obj)
```

**Parameters** \
`obj` [ParameterId](/Murder/Core/Sounds/ParameterId.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡