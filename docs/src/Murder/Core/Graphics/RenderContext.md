# RenderContext

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class RenderContext : IDisposable
```

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public RenderContext(GraphicsDevice graphicsDevice, Camera2D camera)
```

**Parameters** \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`camera` [Camera2D](/Murder/Core/Graphics/Camera2D.html) \

### ⭐ Properties
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
#### DebugSpriteBatch
```csharp
public readonly Batch2D DebugSpriteBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### FloorLightRenderTarget
```csharp
public RenderTarget2D FloorLightRenderTarget { get; }
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
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
#### LightRenderTarget
```csharp
public RenderTarget2D LightRenderTarget { get; }
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
#### ShadowFloorSpriteBatch
```csharp
public readonly Batch2D ShadowFloorSpriteBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### SpotsRenderTarget
```csharp
public RenderTarget2D SpotsRenderTarget { get; }
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
#### SpriteBatch
```csharp
public readonly Batch2D SpriteBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### UiBatch
```csharp
public readonly Batch2D UiBatch;
```

**Returns** \
[Batch2D](/Murder/Core/Graphics/Batch2D.html) \
#### UiMinScale
```csharp
public float UiMinScale { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### UiReferenceScale
```csharp
public Point UiReferenceScale;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### UiRenderTarget
```csharp
public RenderTarget2D UiRenderTarget { get; }
```

**Returns** \
[RenderTarget2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RenderTarget2D.html) \
#### UiScale
```csharp
public Vector2 UiScale { get; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
### ⭐ Methods
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
public Texture2D GetRenderTargetFromEnum(RenderTargets inspectingRenderTarget)
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

#### UpdateBufferTarget(int, float)
```csharp
public void UpdateBufferTarget(int scale, float downsample)
```

**Parameters** \
`scale` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`downsample` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \



⚡