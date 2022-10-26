# Scene

**Namespace:** Murder.Core \
**Assembly:** Murder.dll

```csharp
public abstract class Scene : IDisposable
```

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
protected Scene()
```

### ⭐ Properties
#### RenderContext
```csharp
public RenderContext RenderContext { get; private set; }
```

Context renderer unique to this scene.

**Returns** \
[RenderContext](/Murder/Core/Graphics/RenderContext.html) \
#### World
```csharp
public abstract virtual MonoWorld World { get; }
```

**Returns** \
[MonoWorld](/Murder/Core/MonoWorld.html) \
### ⭐ Methods
#### RefreshWindow(GraphicsDevice, GameProfile, float)
```csharp
public virtual int RefreshWindow(GraphicsDevice graphics, GameProfile settings, float downsample)
```

**Parameters** \
`graphics` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`settings` [GameProfile](/Murder/Assets/GameProfile.html) \
`downsample` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

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

Scenes that would like to implement a Gui should use this method.

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

Reload the active scene.

#### Resume()
```csharp
public void Resume()
```

#### Suspend()
```csharp
public void Suspend()
```

Rests the current scene temporarily.

#### Unload()
```csharp
public void Unload()
```



⚡