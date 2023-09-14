# ParameterRuleAction

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public sealed struct ParameterRuleAction
```

This is a generic blackboard action with a command.

### ⭐ Constructors
```csharp
public ParameterRuleAction()
```

```csharp
public ParameterRuleAction(ParameterId parameter, BlackboardActionKind kind, float value)
```

**Parameters** \
`parameter` [ParameterId](../../../Murder/Core/Sounds/ParameterId.html) \
`kind` [BlackboardActionKind](../../../Murder/Core/Dialogs/BlackboardActionKind.html) \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Kind
```csharp
public readonly BlackboardActionKind Kind;
```

**Returns** \
[BlackboardActionKind](../../../Murder/Core/Dialogs/BlackboardActionKind.html) \
#### Parameter
```csharp
public readonly ParameterId Parameter;
```

**Returns** \
[ParameterId](../../../Murder/Core/Sounds/ParameterId.html) \
#### Value
```csharp
public readonly float Value;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \


⚡