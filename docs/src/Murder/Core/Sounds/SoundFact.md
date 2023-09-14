# SoundFact

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public sealed struct SoundFact : IComparable<T>, IEquatable<T>
```

**Implements:** _[IComparable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IComparable-1?view=net-7.0), [IEquatable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.IEquatable-1?view=net-7.0)_

### ⭐ Constructors
```csharp
public SoundFact()
```

```csharp
public SoundFact(string blackboard, string name)
```

**Parameters** \
`blackboard` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Blackboard
```csharp
public readonly string Blackboard;
```

If null, grab the default blackboard.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Name
```csharp
public readonly string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### Equals(SoundFact)
```csharp
public virtual bool Equals(SoundFact other)
```

**Parameters** \
`other` [SoundFact](../../../Murder/Core/Sounds/SoundFact.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Equals(Object)
```csharp
public virtual bool Equals(Object obj)
```

**Parameters** \
`obj` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### CompareTo(SoundFact)
```csharp
public virtual int CompareTo(SoundFact other)
```

**Parameters** \
`other` [SoundFact](../../../Murder/Core/Sounds/SoundFact.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### GetHashCode()
```csharp
public virtual int GetHashCode()
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡