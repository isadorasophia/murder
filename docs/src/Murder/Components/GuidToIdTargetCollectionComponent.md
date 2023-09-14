# GuidToIdTargetCollectionComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct GuidToIdTargetCollectionComponent : IComponent
```

This is a component used to translate entity instaces guid to an actual entity id.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public GuidToIdTargetCollectionComponent()
```

### ⭐ Properties
#### Collection
```csharp
public readonly ImmutableArray<T> Collection;
```

Guid of the target entity.

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
### ⭐ Methods
#### TryFindGuid(string)
```csharp
public T? TryFindGuid(string name)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \



⚡