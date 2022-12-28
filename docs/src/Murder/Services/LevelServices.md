# LevelServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class LevelServices
```

### ⭐ Methods
#### SwitchSceneOnSecondsCoroutine(Guid, int)
```csharp
public IEnumerator<T> SwitchSceneOnSecondsCoroutine(Guid nextWorldGuid, int seconds)
```

**Parameters** \
`nextWorldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`seconds` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

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

#### SwitchSceneOnSeconds(World, Guid, int)
```csharp
public ValueTask SwitchSceneOnSeconds(World world, Guid nextWorldGuid, int seconds)
```

**Parameters** \
`world` [World](/Bang/World.html) \
`nextWorldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`seconds` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \



⚡