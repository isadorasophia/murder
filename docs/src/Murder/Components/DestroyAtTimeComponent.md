# DestroyAtTimeComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct DestroyAtTimeComponent : IComponent
```

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public DestroyAtTimeComponent()
```

Destroy at the end of the frame

```csharp
public DestroyAtTimeComponent(RemoveStyle style, float timeToDestroy)
```

**Parameters** \
`style` [RemoveStyle](../../Murder/Components/RemoveStyle.html) \
`timeToDestroy` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

```csharp
public DestroyAtTimeComponent(float timeToDestroy)
```

**Parameters** \
`timeToDestroy` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

### ⭐ Properties
#### Style
```csharp
public readonly RemoveStyle Style;
```

**Returns** \
[RemoveStyle](../../Murder/Components/RemoveStyle.html) \
#### TimeToDestroy
```csharp
public readonly float TimeToDestroy;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \


⚡