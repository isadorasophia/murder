# ChoiceLine

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed struct ChoiceLine
```

### ⭐ Constructors
```csharp
public ChoiceLine(Guid speaker, string portrait, string title, ImmutableArray<T> choices)
```

**Parameters** \
`speaker` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`portrait` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`title` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`choices` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

### ⭐ Properties
#### Choices
```csharp
public readonly ImmutableArray<T> Choices;
```

Choices available to the player to pick.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Portrait
```csharp
public readonly string Portrait;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Speaker
```csharp
public readonly T? Speaker;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Title
```csharp
public readonly string Title;
```

Dialog title.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \


⚡