# InteractOnRuleMatchCollectionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct InteractOnRuleMatchCollectionComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public InteractOnRuleMatchCollectionComponent()
```

```csharp
public InteractOnRuleMatchCollectionComponent(ImmutableArray<T> requirements)
```

**Parameters** \
`requirements` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

### ⭐ Properties
#### Requirements
```csharp
public readonly ImmutableArray<T> Requirements;
```

List of interactions that will be triggered.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \


⚡