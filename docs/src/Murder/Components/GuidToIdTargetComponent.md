# GuidToIdTargetComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct GuidToIdTargetComponent : IComponent
```

This is a component used to translate entity instaces guid to an actual entity id.
            Gets translated to [IdTargetComponent](../../Murder/Components/IdTargetComponent.html).

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public GuidToIdTargetComponent(Guid target)
```

**Parameters** \
`target` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### Target
```csharp
public readonly Guid Target;
```

Guid of the target entity.

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \


⚡