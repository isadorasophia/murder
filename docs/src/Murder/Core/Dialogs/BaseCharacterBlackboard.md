# BaseCharacterBlackboard

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public class BaseCharacterBlackboard : ICharacterBlackboard, IBlackboard
```

Built-in capabilities for each speaker blackboard.

**Implements:** _[ICharacterBlackboard](../../../Murder/Core/Dialogs/ICharacterBlackboard.html), [IBlackboard](../../../Murder/Core/Dialogs/IBlackboard.html)_

### ⭐ Constructors
```csharp
public BaseCharacterBlackboard()
```

### ⭐ Properties
#### Kind
```csharp
public virtual BlackboardKind Kind { get; }
```

**Returns** \
[BlackboardKind](../../../Murder/Core/Dialogs/BlackboardKind.html) \
#### Name
```csharp
public static const string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### TotalInteractions
```csharp
public int TotalInteractions;
```

Total of times that it has been interacted to.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \


⚡