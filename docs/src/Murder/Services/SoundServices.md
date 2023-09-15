# SoundServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class SoundServices
```

### ⭐ Methods
#### GetGlobalParameter(ParameterId)
```csharp
public float GetGlobalParameter(ParameterId id)
```

**Parameters** \
`id` [ParameterId](../../Murder/Core/Sounds/ParameterId.html) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Play(SoundEventId, SoundProperties)
```csharp
public ValueTask Play(SoundEventId id, SoundProperties properties)
```

**Parameters** \
`id` [SoundEventId](../../Murder/Core/Sounds/SoundEventId.html) \
`properties` [SoundProperties](../../Murder/Core/Sounds/SoundProperties.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### PlayMusic(SoundEventId)
```csharp
public ValueTask PlayMusic(SoundEventId id)
```

**Parameters** \
`id` [SoundEventId](../../Murder/Core/Sounds/SoundEventId.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### SetGlobalParameter(ParameterId, T)
```csharp
public void SetGlobalParameter(ParameterId id, T value)
```

**Parameters** \
`id` [ParameterId](../../Murder/Core/Sounds/ParameterId.html) \
`value` [T](../../) \

#### Stop(T?, bool)
```csharp
public void Stop(T? id, bool fadeOut)
```

**Parameters** \
`id` [T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
`fadeOut` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### StopAll(bool)
```csharp
public void StopAll(bool fadeOut)
```

**Parameters** \
`fadeOut` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡