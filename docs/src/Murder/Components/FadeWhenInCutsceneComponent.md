# FadeWhenInCutsceneComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct FadeWhenInCutsceneComponent : IComponent
```

For now, this is only supported for aseprite components.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public FadeWhenInCutsceneComponent()
```

```csharp
public FadeWhenInCutsceneComponent(float duration, float previousAlpha)
```

**Parameters** \
`duration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`previousAlpha` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Duration
```csharp
public readonly float Duration;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### PreviousAlpha
```csharp
public readonly float PreviousAlpha;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
### ⭐ Methods
#### TrackAlpha(float)
```csharp
public FadeWhenInCutsceneComponent TrackAlpha(float alpha)
```

**Parameters** \
`alpha` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[FadeWhenInCutsceneComponent](../../Murder/Components/FadeWhenInCutsceneComponent.html) \



⚡