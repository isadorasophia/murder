# PickEntityToAddOnStartComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct PickEntityToAddOnStartComponent : IComponent
```

This will trigger an effect by placing [PickEntityToAddOnStartComponent.OnNotMatchPrefab](../../Murder/Components/PickEntityToAddOnStartComponent.html#onnotmatchprefab) in the world.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public PickEntityToAddOnStartComponent()
```

### ⭐ Properties
#### OnMatchPrefab
```csharp
public readonly Guid OnMatchPrefab;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### OnNotMatchPrefab
```csharp
public readonly Guid OnNotMatchPrefab;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \


⚡