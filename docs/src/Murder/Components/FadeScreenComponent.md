# FadeScreenComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct FadeScreenComponent : IComponent
```

**Implements:** _[IComponent](/Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public FadeScreenComponent(FadeType fade, float startedTime, float duration, Color color, bool destroyAfterFinished)
```

Fades the screen using the FadeScreenSystem

**Parameters** \
`fade` [FadeType](/Murder/Components/FadeType.html) \
\
`startedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`color` [Color](/Murder/Core/Graphics/Color.html) \
\
`destroyAfterFinished` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

### ⭐ Properties
#### Color
```csharp
public readonly Color Color;
```

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \
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
[FadeType](/Murder/Components/FadeType.html) \
#### StartedTime
```csharp
public readonly float StartedTime;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \


⚡