# LevelServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class LevelServices
```

### ⭐ Methods
#### SwitchSceneOnSecondsCoroutine(Guid, float)
```csharp
public IEnumerator<T> SwitchSceneOnSecondsCoroutine(Guid nextWorldGuid, float seconds)
```

**Parameters** \
`nextWorldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`seconds` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[IEnumerator\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerator-1?view=net-7.0) \

#### SwitchScene(Guid)
```csharp
public ValueTask SwitchScene(Guid nextWorldGuid)
```

**Parameters** \
`nextWorldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### SwitchSceneAfterSeconds(World, Guid, float)
```csharp
public ValueTask SwitchSceneAfterSeconds(World world, Guid nextWorldGuid, float seconds)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`nextWorldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`seconds` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \



⚡