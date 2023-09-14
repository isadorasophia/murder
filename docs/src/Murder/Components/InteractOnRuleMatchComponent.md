# InteractOnRuleMatchComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct InteractOnRuleMatchComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public InteractOnRuleMatchComponent()
```

```csharp
public InteractOnRuleMatchComponent(AfterInteractRule after, bool triggered, ImmutableArray<T> requirements)
```

**Parameters** \
`after` [AfterInteractRule](../../Murder/Components/AfterInteractRule.html) \
`triggered` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`requirements` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

```csharp
public InteractOnRuleMatchComponent(InteractOn interactOn, AfterInteractRule after, ImmutableArray<T> requirements)
```

**Parameters** \
`interactOn` [InteractOn](../../Murder/Components/InteractOn.html) \
`after` [AfterInteractRule](../../Murder/Components/AfterInteractRule.html) \
`requirements` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \

```csharp
public InteractOnRuleMatchComponent(CriterionNode[] criteria)
```

**Parameters** \
`criteria` [CriterionNode[]](../../Murder/Core/Dialogs/CriterionNode.html) \

### ⭐ Properties
#### AfterInteraction
```csharp
public readonly AfterInteractRule AfterInteraction;
```

**Returns** \
[AfterInteractRule](../../Murder/Components/AfterInteractRule.html) \
#### InteractOn
```csharp
public readonly InteractOn InteractOn;
```

**Returns** \
[InteractOn](../../Murder/Components/InteractOn.html) \
#### Requirements
```csharp
public readonly ImmutableArray<T> Requirements;
```

List of requirements which will trigger the interactive component within the same entity.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Triggered
```csharp
public readonly bool Triggered;
```

This will only be triggered once the component has been interacted with.
            Used if [AfterInteractRule.InteractOnReload](../../Murder/Components/AfterInteractRule.html#interactonreload) is set.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### Disable()
```csharp
public InteractOnRuleMatchComponent Disable()
```

**Returns** \
[InteractOnRuleMatchComponent](../../Murder/Components/InteractOnRuleMatchComponent.html) \



⚡