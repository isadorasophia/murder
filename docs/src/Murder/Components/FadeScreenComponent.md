# FadeScreenComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct FadeScreenComponent : IComponent
```

**Implements:** _[IComponent](../..//Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public FadeScreenComponent(FadeType fade, float startedTime, float duration, Color color, bool destroyAfterFinished, string customTexture)
```

Fades the screen using the FadeScreenSystem. Duration will be a minimum of 0.1

**Parameters** \
`fade` [FadeType](../..//Murder/Components/FadeType.html) \
`startedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](../..//Murder/Core/Graphics/Color.html) \
`destroyAfterFinished` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`customTexture` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

### ⭐ Properties
#### Color
```csharp
public readonly Color Color;
```

**Returns** \
[Color](../..//Murder/Core/Graphics/Color.html) \
#### CustomTexture
```csharp
public readonly string CustomTexture;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### DestroyAfterFinished
```csharp
public readonly bool DestroyAfterFinished;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Duration
```csharp
public readonly float Duration;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Fade
```csharp
public readonly FadeType Fade;
```

**Returns** \
[FadeType](../..//Murder/Components/FadeType.html) \
#### StartedTime
```csharp
public readonly float StartedTime;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \


⚡