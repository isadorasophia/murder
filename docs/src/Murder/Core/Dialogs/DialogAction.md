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
public DialogAction(int id, Fact fact, BlackboardActionKind kind, string string, T? int, T? bool, IComponent component)
```

**Parameters** \
`id` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`fact` [Fact](../../../Murder/Core/Dialogs/Fact.html) \
`kind` [BlackboardActionKind](../../../Murder/Core/Dialogs/BlackboardActionKind.html) \
`string` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`int` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`bool` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`component` [IComponent](../../../Bang/Components/IComponent.html) \

### ⭐ Properties
#### BoolValue
```csharp
public readonly T? BoolValue;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### ComponentValue
```csharp
public readonly IComponent ComponentValue;
```

**Returns** \
[IComponent](../../../Bang/Components/IComponent.html) \
#### Fact
```csharp
public readonly Fact Fact;
```

**Returns** \
[Fact](../../../Murder/Core/Dialogs/Fact.html) \
#### Id
```csharp
public readonly int Id;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
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
[BlackboardActionKind](../../../Murder/Core/Dialogs/BlackboardActionKind.html) \
#### StrValue
```csharp
public readonly string StrValue;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### WithComponent(IComponent)
```csharp
public DialogAction WithComponent(IComponent c)
```

**Parameters** \
`c` [IComponent](../../../Bang/Components/IComponent.html) \

**Returns** \
[DialogAction](../../../Murder/Core/Dialogs/DialogAction.html) \

#### WithFact(Fact)
```csharp
public DialogAction WithFact(Fact fact)
```

**Parameters** \
`fact` [Fact](../../../Murder/Core/Dialogs/Fact.html) \

**Returns** \
[DialogAction](../../../Murder/Core/Dialogs/DialogAction.html) \

#### WithKind(BlackboardActionKind)
```csharp
public DialogAction WithKind(BlackboardActionKind kind)
```

**Parameters** \
`kind` [BlackboardActionKind](../../../Murder/Core/Dialogs/BlackboardActionKind.html) \

**Returns** \
[DialogAction](../../../Murder/Core/Dialogs/DialogAction.html) \



⚡