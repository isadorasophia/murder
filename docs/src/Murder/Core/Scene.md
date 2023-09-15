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
#### _calledStart
```csharp
protected bool _calledStart;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### RenderContext
```csharp
public RenderContext RenderContext { get; private set; }
```

Context renderer unique to this scene.

**Returns** \
[RenderContext](../../Murder/Core/Graphics/RenderContext.html) \
#### World
```csharp
public abstract virtual MonoWorld World { get; }
```

**Returns** \
[MonoWorld](../../Murder/Core/MonoWorld.html) \
### ⭐ Methods
#### RefreshWindow(GraphicsDevice, GameProfile)
```csharp
public virtual int RefreshWindow(GraphicsDevice graphics, GameProfile settings)
```

**Parameters** \
`graphics` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`settings` [GameProfile](../../Murder/Assets/GameProfile.html) \

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### UnloadAsyncImpl()
```csharp
public virtual Task UnloadAsyncImpl()
```

**Returns** \
[Task](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task?view=net-7.0) \

#### LoadContentAsync(GraphicsDevice, GameProfile)
```csharp
public virtual ValueTask LoadContentAsync(GraphicsDevice graphics, GameProfile settings)
```

**Parameters** \
`graphics` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`settings` [GameProfile](../../Murder/Assets/GameProfile.html) \

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

#### AddOnWindowRefresh(Action)
```csharp
public void AddOnWindowRefresh(Action notification)
```

This will trigger UI refresh operations.

**Parameters** \
`notification` [Action](https://learn.microsoft.com/en-us/dotnet/api/System.Action?view=net-7.0) \

#### Reload()
```csharp
public void Reload()
```

Reload the active scene.

#### ResetWindowRefreshEvents()
```csharp
public void ResetWindowRefreshEvents()
```

This will reset all watchers of trackers.

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