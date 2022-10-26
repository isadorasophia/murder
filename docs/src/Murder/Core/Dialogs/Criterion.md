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
public Criterion(Fact fact, CriterionKind kind, string string, T? int, T? bool)
```

**Parameters** \
`fact` [Fact](/Murder/Core/Dialogs/Fact.html) \
`kind` [CriterionKind](/Murder/Core/Dialogs/CriterionKind.html) \
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
public readonly Fact Fact;
```

**Returns** \
[Fact](/Murder/Core/Dialogs/Fact.html) \
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
[CriterionKind](/Murder/Core/Dialogs/CriterionKind.html) \
#### StrValue
```csharp
public readonly string StrValue;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
### ⭐ Methods
#### WithFact(Fact)
```csharp
public Criterion WithFact(Fact fact)
```

**Parameters** \
`fact` [Fact](/Murder/Core/Dialogs/Fact.html) \

**Returns** \
[Criterion](/Murder/Core/Dialogs/Criterion.html) \

#### WithKind(CriterionKind)
```csharp
public Criterion WithKind(CriterionKind kind)
```

**Parameters** \
`kind` [CriterionKind](/Murder/Core/Dialogs/CriterionKind.html) \

**Returns** \
[Criterion](/Murder/Core/Dialogs/Criterion.html) \

#### FetchValidCriteriaKind()
```csharp
public CriterionKind[] FetchValidCriteriaKind()
```

This returns a list of all the valid criteria kind for the fact.

**Returns** \
[CriterionKind[]](/Murder/Core/Dialogs/CriterionKind.html) \
\



⚡