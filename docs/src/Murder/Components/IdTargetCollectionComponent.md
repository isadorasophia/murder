# IdTargetCollectionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct IdTargetCollectionComponent : IComponent
```

This is a component with a collection of entities tracked in the world.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public IdTargetCollectionComponent(ImmutableDictionary<TKey, TValue> targets)
```

**Parameters** \
`targets` [ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \

### ⭐ Properties
#### Targets
```csharp
public readonly ImmutableDictionary<TKey, TValue> Targets;
```

Id of the target entity.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \


⚡