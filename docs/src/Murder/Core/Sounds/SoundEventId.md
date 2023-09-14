# SoundEventId

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public sealed struct SoundEventId : IEqualityComparer<T>, IEquatable<T>
```

**Implements:** _[IEqualityComparer\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEqualityComparer-1?view=net-7.0), [IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Properties
#### Data1
```csharp
public int Data1 { get; public set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Data2
```csharp
public int Data2 { get; public set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Data3
```csharp
public int Data3 { get; public set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Data4
```csharp
public int Data4 { get; public set; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### EditorName
```csharp
public string EditorName { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### IsGuidEmpty
```csharp
public bool IsGuidEmpty { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Path
```csharp
public string Path { get; public set; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### WithPath(string)
```csharp
public SoundEventId WithPath(string path)
```

**Parameters** \
`path` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[SoundEventId](../../../Murder/Core/Sounds/SoundEventId.html) \

#### Equals(SoundEventId)
```csharp
public virtual bool Equals(SoundEventId other)
```

**Parameters** \
`other` [SoundEventId](../../../Murder/Core/Sounds/SoundEventId.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(SoundEventId, SoundEventId)
```csharp
public virtual bool Equals(SoundEventId x, SoundEventId y)
```

**Parameters** \
`x` [SoundEventId](../../../Murder/Core/Sounds/SoundEventId.html) \
`y` [SoundEventId](../../../Murder/Core/Sounds/SoundEventId.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetHashCode(SoundEventId)
```csharp
public virtual int GetHashCode(SoundEventId obj)
```

**Parameters** \
`obj` [SoundEventId](../../../Murder/Core/Sounds/SoundEventId.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡