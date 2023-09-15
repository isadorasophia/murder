# RemoveEntityOnRuleMatchAtLoadComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct RemoveEntityOnRuleMatchAtLoadComponent : IComponent
```

This will remove the entity that contains this component as soon as the entity is serialized
            into an actual world instance.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public RemoveEntityOnRuleMatchAtLoadComponent()
```

### ⭐ Properties
#### Requirements
```csharp
public readonly ImmutableArray<T> Requirements;
```

List of requirements which will trigger the interactive component within the same entity.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \


⚡