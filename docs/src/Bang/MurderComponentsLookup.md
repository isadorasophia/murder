# MurderComponentsLookup

**Namespace:** Bang \
**Assembly:** Murder.dll

```csharp
public class MurderComponentsLookup : ComponentsLookup
```

Auto-generated implementation of [ComponentsLookup](../Bang/ComponentsLookup.html) for this project.

**Implements:** _[ComponentsLookup](../Bang/ComponentsLookup.html)_

### ⭐ Constructors
```csharp
public MurderComponentsLookup()
```

Default constructor. This is only relevant for the internals of Bang, so you can ignore it.

### ⭐ Properties
#### ComponentsIndex
```csharp
protected ImmutableDictionary<TKey, TValue> ComponentsIndex { get; protected set; }
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### MessagesIndex
```csharp
protected ImmutableDictionary<TKey, TValue> MessagesIndex { get; protected set; }
```

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### MurderNextLookupId
```csharp
public static int MurderNextLookupId { get; }
```

First lookup id a [ComponentsLookup](../Bang/ComponentsLookup.html) implementation that inherits from this class must use.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### RelativeComponents
```csharp
public ImmutableHashSet<T> RelativeComponents { get; protected set; }
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