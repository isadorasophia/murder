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
`camera` [Camera2D](../..//Murder/Core/Graphics/Camera2D.html) \
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
[Color](../..//Murder/Core/Graphics/Color.html) \
#### Bloom
```csharp
public float Bloom;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### CachedTextTextures
```csharp
public readonly CacheDictionary<TKey, TValue> CachedTextTextures;
```

**Returns** \
[CacheDictionary\<TKey, TValue\>](../..//Murder/Utilities/CacheDictionary-2.html) \
#### Camera
```csharp
public readonly Camera2D Camera;
```

**Returns** \
[Camera2D](../..//Murder/Core/Graphics/Camera2D.html) \
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
#### DebugFxSpriteBatch
```csharp
public readonly Batch2D DebugFxSpriteBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
#### DebugSpriteBatch
```csharp
public readonly Batch2D DebugSpriteBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
#### FloorSpriteBatch
```csharp
public readonly Batch2D FloorSpriteBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
#### GameBufferSize
```csharp
public Point GameBufferSize;
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### GameplayBatch
```csharp
public readonly Batch2D GameplayBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
#### GameUiBatch
```csharp
public readonly Batch2D GameUiBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
#### LastRenderTarget
```csharp
public RenderTarget2D LastRenderTarget { get; }
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
#### LightBatch
```csharp
public readonly Batch2D LightBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
#### MainTarget
```csharp
public RenderTarget2D MainTarget { get; }
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
#### PreviewState
```csharp
public BatchPreviewState PreviewState;
```

**Returns** \
[BatchPreviewState](../..//Murder/Core/Graphics/BatchPreviewState.html) \
#### PreviewStretch
```csharp
public bool PreviewStretch;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### ReflectedBatch
```csharp
public readonly Batch2D ReflectedBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
#### ReflectionAreaBatch
```csharp
public readonly Batch2D ReflectionAreaBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
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
[Point](../..//Murder/Core/Geometry/Point.html) \
#### UiBatch
```csharp
public readonly Batch2D UiBatch;
```

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \
### ⭐ Methods
#### UnloadImpl()
```csharp
protected virtual void UnloadImpl()
```

#### GetSpriteBatch(TargetSpriteBatches)
```csharp
public Batch2D GetSpriteBatch(TargetSpriteBatches targetSpriteBatch)
```

**Parameters** \
`targetSpriteBatch` [TargetSpriteBatches](../..//Murder/Core/Graphics/TargetSpriteBatches.html) \

**Returns** \
[Batch2D](../..//Murder/Core/Graphics/Batch2D.html) \

#### RefreshWindow(Point, float)
```csharp
public bool RefreshWindow(Point size, float scale)
```

Refresh the window size with <paramref name="size" /> with width and height information,
            respectively.

**Parameters** \
`size` [Point](../..//Murder/Core/Geometry/Point.html) \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### GetRenderTargetFromEnum(RenderTargets)
```csharp
public virtual Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
```

**Parameters** \
`inspectingRenderTarget` [RenderTargets](../..//Murder/Core/Graphics/RenderTargets.html) \

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

#### CreateDebugPreviewIfNecessary(BatchPreviewState, RenderTarget2D)
```csharp
public void CreateDebugPreviewIfNecessary(BatchPreviewState currentState, RenderTarget2D target)
```

**Parameters** \
`currentState` [BatchPreviewState](../..//Murder/Core/Graphics/BatchPreviewState.html) \
`target` [RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \

#### End()
```csharp
public void End()
```

#### SaveScreenShot(Rectangle)
```csharp
public void SaveScreenShot(Rectangle cameraRect)
```

**Parameters** \
`cameraRect` [Rectangle](../..//Murder/Core/Geometry/Rectangle.html) \

#### SwitchCustomShader(bool)
```csharp
public void SwitchCustomShader(bool enable)
```

**Parameters** \
`enable` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Unload()
```csharp
public void Unload()
```

Unload the render context.
            Called when the render context is no longer being actively displayed.

#### UpdateBufferTarget(float)
```csharp
public void UpdateBufferTarget(float scale)
```

**Parameters** \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡