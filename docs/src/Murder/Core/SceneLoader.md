# SceneLoader

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public class SceneLoader
```

### ⭐ Constructors
```csharp
public SceneLoader(GraphicsDeviceManager graphics, GameProfile settings, Scene scene)
```

**Parameters** \
`graphics` [GraphicsDeviceManager](https://docs.monogame.net/api/Microsoft.Xna.Framework.GraphicsDeviceManager.html) \
`settings` [GameProfile](../../Murder/Assets/GameProfile.html) \
`scene` [Scene](../../Murder/Core/Scene.html) \

### ⭐ Properties
#### _graphics
```csharp
protected readonly GraphicsDeviceManager _graphics;
```

**Returns** \
[GraphicsDeviceManager](https://docs.monogame.net/api/Microsoft.Xna.Framework.GraphicsDeviceManager.html) \
#### _settings
```csharp
protected readonly GameProfile _settings;
```

**Returns** \
[GameProfile](../../Murder/Assets/GameProfile.html) \
#### ActiveScene
```csharp
public Scene ActiveScene { get; }
```

**Returns** \
[Scene](../../Murder/Core/Scene.html) \
### ⭐ Methods
#### ReplaceWorldOnCurrentScene(MonoWorld)
```csharp
public bool ReplaceWorldOnCurrentScene(MonoWorld world)
```

**Parameters** \
`world` [MonoWorld](../../Murder/Core/MonoWorld.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LoadContentAsync()
```csharp
public ValueTask LoadContentAsync()
```

Load the content of the current active scene.

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### SwitchScene()
```csharp
public void SwitchScene()
```

Switch to a scene of type <typeparamref name="T" />.

#### SwitchScene(Scene)
```csharp
public void SwitchScene(Scene scene)
```

Switch to <paramref name="scene" />.

**Parameters** \
`scene` [Scene](../../Murder/Core/Scene.html) \

#### SwitchScene(Guid)
```csharp
public void SwitchScene(Guid worldGuid)
```

**Parameters** \
`worldGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \



⚡