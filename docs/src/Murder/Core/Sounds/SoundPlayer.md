# SoundPlayer

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public class SoundPlayer : ISoundPlayer
```

**Implements:** _[ISoundPlayer](/Murder/Core/Sounds/ISoundPlayer.html)_

### ⭐ Constructors
```csharp
public SoundPlayer()
```

### ⭐ Methods
#### PlayEvent(SoundEventId, bool)
```csharp
public virtual ValueTask PlayEvent(SoundEventId id, bool _)
```

**Parameters** \
`id` [SoundEventId](/Murder/Core/Sounds/SoundEventId.html) \
`_` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### PlayStreaming(SoundEventId)
```csharp
public virtual ValueTask PlayStreaming(SoundEventId sound)
```

**Parameters** \
`sound` [SoundEventId](/Murder/Core/Sounds/SoundEventId.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Initialize(string)
```csharp
public virtual void Initialize(string resourcesPath)
```

**Parameters** \
`resourcesPath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### SetVolume(T?, float)
```csharp
public virtual void SetVolume(T? _, float volume)
```

Change volume.

**Parameters** \
`_` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`volume` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Stop(bool)
```csharp
public virtual void Stop(bool _)
```

**Parameters** \
`_` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Update()
```csharp
public virtual void Update()
```



⚡