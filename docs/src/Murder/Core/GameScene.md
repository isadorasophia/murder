# GameScene

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public class GameScene : Scene, IDisposable
```

**Implements:** _[Scene](/Murder/Core/Scene.html), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public GameScene(Guid guid)
```

**Parameters** \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

### ⭐ Properties
#### RenderContext
```csharp
public RenderContext RenderContext { get; }
```

**Returns** \
[RenderContext](/Murder/Core/Graphics/RenderContext.html) \
#### World
```csharp
public virtual MonoWorld World { get; }
```

**Returns** \
[MonoWorld](/Murder/Core/MonoWorld.html) \
#### WorldGuid
```csharp
public Guid WorldGuid { get; }
```

**Returns** \
[Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
### ⭐ Methods
#### RefreshWindow(GraphicsDevice, GameProfile)
```csharp
public virtual int RefreshWindow(GraphicsDevice graphics, GameProfile settings)
```

**Parameters** \
`graphics` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`settings` [GameProfile](/Murder/Assets/GameProfile.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### LoadContentAsync(GraphicsDevice, GameProfile)
```csharp
public virtual ValueTask LoadContentAsync(GraphicsDevice graphics, GameProfile settings)
```

**Parameters** \
`graphics` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`settings` [GameProfile](/Murder/Assets/GameProfile.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \

#### Dispose()
```csharp
public virtual void Dispose()
```

#### Draw()
```csharp
public virtual void Draw()
```

#### DrawGui()
```csharp
public virtual void DrawGui()
```

#### FixedUpdate()
```csharp
public virtual void FixedUpdate()
```

#### ReloadImpl()
```csharp
public virtual void ReloadImpl()
```

#### ResumeImpl()
```csharp
public virtual void ResumeImpl()
```

#### Start()
```csharp
public virtual void Start()
```

#### SuspendImpl()
```csharp
public virtual void SuspendImpl()
```

#### Update()
```csharp
public virtual void Update()
```

#### Reload()
```csharp
public void Reload()
```

#### Resume()
```csharp
public void Resume()
```

#### Suspend()
```csharp
public void Suspend()
```

#### Unload()
```csharp
public void Unload()
```



⚡