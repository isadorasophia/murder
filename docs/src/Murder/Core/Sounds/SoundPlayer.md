# SoundPlayer

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public class SoundPlayer : ISoundPlayer
```

**Implements:** _[ISoundPlayer](../..//Murder/Core/Sounds/ISoundPlayer.html)_

### ⭐ Constructors
```csharp
public SoundPlayer()
```

### ⭐ Methods
#### Stop(T?, bool)
```csharp
public virtual bool Stop(T? id, bool fadeOut)
```

**Parameters** \
`id` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`fadeOut` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetGlobalParameter(ParameterId)
```csharp
public virtual float GetGlobalParameter(ParameterId _)
```

**Parameters** \
`_` [ParameterId](../..//Murder/Core/Sounds/ParameterId.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### LoadContentAsync()
```csharp
public virtual Task LoadContentAsync()
```

**Returns** \
[Task](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task?view=net-7.0) \

#### ReloadAsync()
```csharp
public virtual Task ReloadAsync()
```

**Returns** \
[Task](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task?view=net-7.0) \

#### PlayEvent(SoundEventId, SoundProperties)
```csharp
public virtual ValueTask PlayEvent(SoundEventId _, SoundProperties __)
```

**Parameters** \
`_` [SoundEventId](../..//Murder/Core/Sounds/SoundEventId.html) \
`__` [SoundProperties](../..//Murder/Core/Sounds/SoundProperties.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Initialize(string)
```csharp
public virtual void Initialize(string resourcesPath)
```

**Parameters** \
`resourcesPath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### SetGlobalParameter(ParameterId, float)
```csharp
public virtual void SetGlobalParameter(ParameterId parameter, float value)
```

**Parameters** \
`parameter` [ParameterId](../..//Murder/Core/Sounds/ParameterId.html) \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### SetParameter(SoundEventId, ParameterId, float)
```csharp
public virtual void SetParameter(SoundEventId instance, ParameterId parameter, float value)
```

**Parameters** \
`instance` [SoundEventId](../..//Murder/Core/Sounds/SoundEventId.html) \
`parameter` [ParameterId](../..//Murder/Core/Sounds/ParameterId.html) \
`value` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### SetVolume(T?, float)
```csharp
public virtual void SetVolume(T? _, float volume)
```

Change volume.

**Parameters** \
`_` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`volume` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Update()
```csharp
public virtual void Update()
```



⚡