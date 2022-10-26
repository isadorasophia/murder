# InteractOnRuleMatchComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct InteractOnRuleMatchComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public InteractOnRuleMatchComponent()
```

```csharp
public InteractOnRuleMatchComponent(CriterionNode[] criteria)
```

**Parameters** \
`criteria` [CriterionNode[]](/Murder/Core/Dialogs/CriterionNode.html) \

### ⭐ Properties
#### Requirements
```csharp
public readonly ImmutableArray<T> Requirements;
```

List of requirements which will trigger the interactive component within the same entity.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \


⚡