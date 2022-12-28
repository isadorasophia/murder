# AnimationOverloadComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct AnimationOverloadComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public AnimationOverloadComponent(bool loop, String[] animationId, int current)
```

**Parameters** \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`animationId` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`current` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

```csharp
public AnimationOverloadComponent(bool loop, String[] animationId)
```

**Parameters** \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`animationId` [string[]](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

```csharp
public AnimationOverloadComponent(string animationId, bool loop, bool ignoreFacing)
```

**Parameters** \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

```csharp
public AnimationOverloadComponent(string animationId, float duration, bool loop, bool ignoreFacing)
```

**Parameters** \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`ignoreFacing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### AnimationCount
```csharp
public int AnimationCount { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### AnimationId
```csharp
public string AnimationId { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Current
```csharp
public readonly int Current;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### CurrentAnimation
```csharp
public string CurrentAnimation { get; }
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### Duration
```csharp
public readonly float Duration;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### IgnoreFacing
```csharp
public readonly bool IgnoreFacing;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Loop
```csharp
public readonly bool Loop;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Start
```csharp
public readonly float Start;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### PlayNext()
```csharp
public AnimationOverloadComponent PlayNext()
```

**Returns** \
[AnimationOverloadComponent](/Murder/Components/AnimationOverloadComponent.html) \



⚡