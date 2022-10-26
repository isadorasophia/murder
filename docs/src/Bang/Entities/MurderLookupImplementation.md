# MurderLookupImplementation

**Namespace:** Bang.Entities \
**Assembly:** Murder.dll

```csharp
public class MurderLookupImplementation : ComponentsLookup
```

**Implements:** _[ComponentsLookup](/Bang/ComponentsLookup.html)_

### ⭐ Constructors
```csharp
public MurderLookupImplementation()
```

### ⭐ Properties
#### ComponentsIndex
```csharp
protected virtual ImmutableDictionary<TKey, TValue> ComponentsIndex { get; }
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### MessagesIndex
```csharp
protected virtual ImmutableDictionary<TKey, TValue> MessagesIndex { get; }
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### RelativeComponents
```csharp
public virtual ImmutableHashSet<T> RelativeComponents { get; }
```

**Returns** \
[ImmutableHashSet\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableHashSet-1?view=net-7.0) \
### ⭐ Methods
#### IsRelative(int)
```csharp
public bool IsRelative(int id)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Id(Type)
```csharp
public int Id(Type t)
```

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡