# SoundServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class SoundServices
```

### ⭐ Methods
#### Play(Guid, bool)
```csharp
public ValueTask Play(Guid guid, bool persist)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`persist` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### PlayMusic(SoundEventId)
```csharp
public ValueTask PlayMusic(SoundEventId id)
```

**Parameters** \
`id` [SoundEventId](/Murder/Core/Sounds/SoundEventId.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### PlaySound(SoundEventId, bool)
```csharp
public ValueTask PlaySound(SoundEventId id, bool loop)
```

**Parameters** \
`id` [SoundEventId](/Murder/Core/Sounds/SoundEventId.html) \
`loop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### StopAll()
```csharp
public void StopAll()
```



⚡