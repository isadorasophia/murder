# Fact

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed struct Fact
```

### ⭐ Constructors
```csharp
public Fact()
```

```csharp
public Fact(string blackboard, string name, FactKind kind, Type componentType)
```

**Parameters** \
`blackboard` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`kind` [FactKind](../../../Murder/Core/Dialogs/FactKind.html) \
`componentType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

```csharp
public Fact(Type componentType)
```

**Parameters** \
`componentType` [Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \

### ⭐ Properties
#### Blackboard
```csharp
public readonly string Blackboard;
```

If null, grab the default blackboard.

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### ComponentType
```csharp
public readonly Type ComponentType;
```

Set when the fact is of type [FactKind.Component](../../../Murder/Core/Dialogs/FactKind.html#component)

**Returns** \
[Type](https://learn.microsoft.com/en-us/dotnet/api/System.Type?view=net-7.0) \
#### EditorName
```csharp
public string EditorName { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Kind
```csharp
public readonly FactKind Kind;
```

**Returns** \
[FactKind](../../../Murder/Core/Dialogs/FactKind.html) \
#### Name
```csharp
public readonly string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \


⚡