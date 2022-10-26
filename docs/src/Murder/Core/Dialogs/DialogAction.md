# DialogAction

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed struct DialogAction
```

### ⭐ Constructors
```csharp
public DialogAction()
```

```csharp
public DialogAction(T? fact, BlackboardActionKind kind, string string, T? int, T? bool)
```

**Parameters** \
`fact` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`kind` [BlackboardActionKind](/Murder/Core/Dialogs/BlackboardActionKind.html) \
`string` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`int` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`bool` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

### ⭐ Properties
#### BoolValue
```csharp
public readonly T? BoolValue;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Fact
```csharp
public readonly T? Fact;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### IntValue
```csharp
public readonly T? IntValue;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Kind
```csharp
public readonly BlackboardActionKind Kind;
```

**Returns** \
[BlackboardActionKind](/Murder/Core/Dialogs/BlackboardActionKind.html) \
#### StrValue
```csharp
public readonly string StrValue;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### FetchValidActionKind()
```csharp
public BlackboardActionKind[] FetchValidActionKind()
```

This returns a list of all the valid [Murder.Core.Dialogs.DialogAction.Fact]().

**Returns** \
[BlackboardActionKind[]](/Murder/Core/Dialogs/BlackboardActionKind.html) \

#### WithFact(Fact)
```csharp
public DialogAction WithFact(Fact fact)
```

**Parameters** \
`fact` [Fact](/Murder/Core/Dialogs/Fact.html) \

**Returns** \
[DialogAction](/Murder/Core/Dialogs/DialogAction.html) \

#### WithKind(BlackboardActionKind)
```csharp
public DialogAction WithKind(BlackboardActionKind kind)
```

**Parameters** \
`kind` [BlackboardActionKind](/Murder/Core/Dialogs/BlackboardActionKind.html) \

**Returns** \
[DialogAction](/Murder/Core/Dialogs/DialogAction.html) \



⚡