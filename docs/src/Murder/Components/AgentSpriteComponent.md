# AgentSpriteComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct AgentSpriteComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public AgentSpriteComponent()
```

### ⭐ Properties
#### AngleSuffixOffset
```csharp
public readonly float AngleSuffixOffset;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### AnimationGuid
```csharp
public readonly Guid AnimationGuid;
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
#### FlipWest
```csharp
public readonly bool FlipWest;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IdlePrefix
```csharp
public readonly string IdlePrefix;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Suffix
```csharp
public readonly string Suffix;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### TargetSpriteBatch
```csharp
public readonly int TargetSpriteBatch;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### WalkPrefix
```csharp
public readonly string WalkPrefix;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### YSortOffset
```csharp
public readonly int YSortOffset;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### WithAnimation(Guid, bool)
```csharp
public AgentSpriteComponent WithAnimation(Guid animationGuid, bool flip)
```

**Parameters** \
`animationGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`flip` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[AgentSpriteComponent](../../Murder/Components/AgentSpriteComponent.html) \



⚡