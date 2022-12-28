# RenderContext

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class RenderContext : IDisposable
```

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public RenderContext(GraphicsDevice graphicsDevice, Camera2D camera, bool useCustomShader)
```

**Parameters** \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`camera` [Camera2D](/Murder/Core/Graphics/Camera2D.html) \
`useCustomShader` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### _floorBufferTarget
```csharp
protected RenderTarget2D _floorBufferTarget;
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
#### _graphicsDevice
```csharp
protected GraphicsDevice _graphicsDevice;
```

**Returns** \
[GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
#### BackColor
```csharp
public Color BackColor { get; }
```

**Returns** \
[Color](/Murder/Core/Graphics/Color.html) \
#### CachedTextTextures
```csharp
public readonly CacheDictionary<TKey, TValue> CachedTextTextures;
```

**Returns** \
[CacheDictionary\<TKey, TValue\>](/Murder/Utilities/CacheDictionary-2.html) \
#### Camera
```csharp
public readonly Camera2D Camera;
```

**Returns** \
[Camera2D](/Murder/Core/Graphics/Camera2D.html) \
#### CAMERA_BLEED
```csharp
public readonly static int CAMERA_BLEED;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### ColorGrade
```csharp
public Texture2D ColorGrade;
```

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
#### CustomFinalShader
```csharp
public Effect CustomFinalShader;
```

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### DebugFxSpriteBatch
```csharp
public readonly Batch2D DebugFxSpriteBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### DebugSpriteBatch
```csharp
public readonly Batch2D DebugSpriteBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### FloorSpriteBatch
```csharp
public readonly Batch2D FloorSpriteBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### GameBufferSize
```csharp
public Point GameBufferSize;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### GameplayBatch
```csharp
public readonly Batch2D GameplayBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### GameUiBatch
```csharp
public readonly Batch2D GameUiBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### LastRenderTarget
```csharp
public RenderTarget2D LastRenderTarget { get; }
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
#### RenderToScreen
```csharp
public bool RenderToScreen;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### ScreenSize
```csharp
public Point ScreenSize;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### UiBatch
```csharp
public readonly Batch2D UiBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
### ⭐ Methods
#### DrawFinalTarget(RenderTarget2D)
```csharp
protected virtual void DrawFinalTarget(RenderTarget2D target)
```

**Parameters** \
`target` [RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \

#### UnloadImpl()
```csharp
protected virtual void UnloadImpl()
```

#### UpdateBufferTargetImpl()
```csharp
protected virtual void UpdateBufferTargetImpl()
```

#### GetSpriteBatch(TargetSpriteBatches)
```csharp
public Batch2D GetSpriteBatch(TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`targetSpriteBatch` [TargetSpriteBatches](/Murder/Core/Graphics/TargetSpriteBatches.html) \

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \

#### RefreshWindow(Point, int)
```csharp
public bool RefreshWindow(Point size, int scale)
```

Refresh the window size with <paramref name="size" /> with width and height information,
            respectively.

**Parameters** \
`size` [Point](/Murder/Core/Geometry/Point.html) \
`scale` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
Whether the window actually required a refresh.\

#### GetRenderTargetFromEnum(RenderTargets)
```csharp
public virtual Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
```

**Parameters** \
`inspectingRenderTarget` [RenderTargets](/Murder/Core/Graphics/RenderTargets.html) \

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

#### Dispose()
```csharp
public virtual void Dispose()
```

#### Begin()
```csharp
public void Begin()
```

#### End()
```csharp
public void End()
```

#### Unload()
```csharp
public void Unload()
```

Unload the render context.
            Called when the render context is no longer being actively displayed.

#### UpdateBufferTarget(int)
```csharp
public void UpdateBufferTarget(int scale)
```

**Parameters** \
`scale` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \



⚡