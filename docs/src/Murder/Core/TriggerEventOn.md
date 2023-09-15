# TriggerEventOn

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public sealed struct TriggerEventOn
```

### ⭐ Constructors
```csharp
public TriggerEventOn()
```

```csharp
public TriggerEventOn(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Name
```csharp
public readonly string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### OnlyOnce
```csharp
public readonly bool OnlyOnce;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Requirements
```csharp
public readonly ImmutableArray<T> Requirements;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Triggers
```csharp
public readonly ImmutableArray<T> Triggers;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### World
```csharp
public readonly T? World;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
### ⭐ Methods
#### CreateComponents()
```csharp
public IComponent[] CreateComponents()
```

**Returns** \
[IComponent[]](../../Bang/Components/IComponent.html) \



⚡