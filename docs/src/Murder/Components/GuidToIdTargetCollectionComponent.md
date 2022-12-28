# GuidToIdTargetCollectionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct GuidToIdTargetCollectionComponent : IComponent
```

This is a component used to translate entity instaces guid to an actual entity id.

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public GuidToIdTargetCollectionComponent()
```

### ⭐ Properties
#### Targets
```csharp
public readonly ImmutableDictionary<TKey, TValue> Targets;
```

Guid of the target entity.

**Returns** \
[ImmutableDictionary\<TKey, TValue\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableDictionary-2?view=net-7.0) \


⚡