# MurderTransformComponentsLookup

**Namespace:** Bang \
**Assembly:** Murder.dll

```csharp
public class MurderTransformComponentsLookup : MurderComponentsLookup
```

Additional lookup class on top of the Bang generated one. Needed for adding
            [IMurderTransformComponent](../Murder/Components/IMurderTransformComponent.html) to the relative component lookup table with the correct id.

**Implements:** _[MurderComponentsLookup](../Bang/MurderComponentsLookup.html)_

### ⭐ Constructors
```csharp
public MurderTransformComponentsLookup()
```

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
#### MurderTransformNextLookupId
```csharp
public readonly static int MurderTransformNextLookupId;
```

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