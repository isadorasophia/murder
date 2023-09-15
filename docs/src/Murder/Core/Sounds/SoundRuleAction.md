# SoundRuleAction

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public sealed struct SoundRuleAction
```

This is a generic blackboard action with a command.

### ⭐ Constructors
```csharp
public SoundRuleAction()
```

```csharp
public SoundRuleAction(SoundFact fact, BlackboardActionKind kind, Object value)
```

**Parameters** \
`fact` [SoundFact](../../../Murder/Core/Sounds/SoundFact.html) \
`kind` [BlackboardActionKind](../../../Murder/Core/Dialogs/BlackboardActionKind.html) \
`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

### ⭐ Properties
#### Fact
```csharp
public readonly SoundFact Fact;
```

**Returns** \
[SoundFact](../../../Murder/Core/Sounds/SoundFact.html) \
#### Kind
```csharp
public readonly BlackboardActionKind Kind;
```

**Returns** \
[BlackboardActionKind](../../../Murder/Core/Dialogs/BlackboardActionKind.html) \
#### Value
```csharp
public readonly Object Value;
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \


⚡