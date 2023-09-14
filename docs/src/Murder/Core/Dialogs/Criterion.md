# Criterion

**Namespace:** Murder.Core.Dialogs \
**Assembly:** Murder.dll

```csharp
public sealed struct Criterion
```

### ⭐ Constructors
```csharp
public Criterion()
```

```csharp
public Criterion(Fact fact, CriterionKind kind, Object value)
```

**Parameters** \
`fact` [Fact](../../../Murder/Core/Dialogs/Fact.html) \
`kind` [CriterionKind](../../../Murder/Core/Dialogs/CriterionKind.html) \
`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

```csharp
public Criterion(Fact fact, CriterionKind kind, T? bool, T? int, string string, Object value)
```

**Parameters** \
`fact` [Fact](../../../Murder/Core/Dialogs/Fact.html) \
`kind` [CriterionKind](../../../Murder/Core/Dialogs/CriterionKind.html) \
`bool` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`int` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`string` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`value` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \

### ⭐ Properties
#### BoolValue
```csharp
public readonly T? BoolValue;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Component
```csharp
public static Criterion Component { get; }
```

Creates a fact of type [FactKind.Component](../../../Murder/Core/Dialogs/FactKind.html#component).

**Returns** \
[Criterion](../../../Murder/Core/Dialogs/Criterion.html) \
#### Fact
```csharp
public readonly Fact Fact;
```

**Returns** \
[Fact](../../../Murder/Core/Dialogs/Fact.html) \
#### IntValue
```csharp
public readonly T? IntValue;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Kind
```csharp
public readonly CriterionKind Kind;
```

**Returns** \
[CriterionKind](../../../Murder/Core/Dialogs/CriterionKind.html) \
#### StrValue
```csharp
public readonly string StrValue;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Value
```csharp
public readonly Object Value;
```

**Returns** \
[Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \
#### Weight
```csharp
public static Criterion Weight { get; }
```

Creates a fact of type [FactKind.Weight](../../../Murder/Core/Dialogs/FactKind.html#weight).

**Returns** \
[Criterion](../../../Murder/Core/Dialogs/Criterion.html) \


⚡