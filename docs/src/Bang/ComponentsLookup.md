# ComponentsLookup

**Namespace:** Bang \
**Assembly:** Bang.dll

```csharp
public abstract class ComponentsLookup
```

Implemented by generators in order to provide a mapping of all the types to their respective id.

### ⭐ Constructors
```csharp
protected ComponentsLookup()
```

### ⭐ Properties
#### ComponentsIndex
```csharp
protected ImmutableDictionary<TKey, TValue> ComponentsIndex { get; protected set; }
```

Maps all the components to their unique id.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### MessagesIndex
```csharp
protected ImmutableDictionary<TKey, TValue> MessagesIndex { get; protected set; }
```

Maps all the messages to their unique id.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \
#### NextLookupId
```csharp
public static const int NextLookupId;
```

Tracks the last id this particular implementation is tracking plus one.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### RelativeComponents
```csharp
public ImmutableHashSet<T> RelativeComponents { get; protected set; }
```

List of all the unique id of the components that inherit from [IParentRelativeComponent](../Bang/Components/IParentRelativeComponent.html).

**Returns** \
[ImmutableHashSet\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableHashSet-1?view=net-7.0) \
### ⭐ Methods
#### IsRelative(int)
```csharp
public bool IsRelative(int id)
```

Returns whether a <paramref name="id" /> is relative to its parent.

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Id(Type)
```csharp
public int Id(Type t)
```

Get the id for <paramref name="t" /> component type.

**Parameters** \
`t` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
\

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡