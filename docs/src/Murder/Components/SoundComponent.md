# SoundComponent

**Namespace:** Murder.Components \
**Assembly:** Murder.dll

```csharp
public sealed struct SoundComponent : IComponent
```

Sound component which will be immediately played and destroyed.

**Implements:** _[IComponent](../../Bang/Components/IComponent.html)_

### ⭐ Constructors
```csharp
public SoundComponent()
```

```csharp
public SoundComponent(SoundEventId sound, bool destroyEntity)
```

**Parameters** \
`sound` [SoundEventId](../../Murder/Core/Sounds/SoundEventId.html) \
`destroyEntity` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### DestroyEntity
```csharp
public readonly bool DestroyEntity;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Sound
```csharp
public readonly T? Sound;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \


⚡