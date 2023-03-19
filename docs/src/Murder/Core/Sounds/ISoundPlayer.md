# ISoundPlayer

**Namespace:** Murder.Core.Sounds \
**Assembly:** Murder.dll

```csharp
public abstract ISoundPlayer
```

### ⭐ Methods
#### PlayEvent(SoundEventId, bool)
```csharp
public abstract ValueTask PlayEvent(SoundEventId id, bool isLoop)
```

Play a sound/event with the id of <paramref name="id" />.
            If <paramref name="isLoop" /> is set, the sound will be persisted.

**Parameters** \
`id` [SoundEventId](/Murder/Core/Sounds/SoundEventId.html) \
`isLoop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### PlayStreaming(SoundEventId)
```csharp
public abstract ValueTask PlayStreaming(SoundEventId id)
```

Start a streaming sound/event in the background.
            This is called for music or ambience sounds.

**Parameters** \
`id` [SoundEventId](/Murder/Core/Sounds/SoundEventId.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Initialize(string)
```csharp
public abstract void Initialize(string resourcesPath)
```

**Parameters** \
`resourcesPath` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### SetVolume(T?, float)
```csharp
public abstract void SetVolume(T? id, float volume)
```

Change volume.

**Parameters** \
`id` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`volume` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Stop(bool)
```csharp
public abstract void Stop(bool fadeOut)
```

Stop all active streaming events.
            If <paramref name="fadeOut" /> is set, this will stop with a fadeout.

**Parameters** \
`fadeOut` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Update()
```csharp
public abstract void Update()
```



⚡