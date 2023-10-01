# Batch2D

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class Batch2D : IDisposable
```

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public Batch2D(string name, GraphicsDevice graphicsDevice, Effect effect, BatchMode batchMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, bool autoHandleAlphaBlendedSprites)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`effect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`batchMode` [BatchMode](../../../Murder/Core/Graphics/BatchMode.html) \
`blendState` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
`samplerState` [SamplerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SamplerState.html) \
`depthStencilState` [DepthStencilState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.DepthStencilState.html) \
`rasterizerState` [RasterizerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RasterizerState.html) \
`autoHandleAlphaBlendedSprites` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

```csharp
public Batch2D(string name, GraphicsDevice graphicsDevice, bool followCamera, Effect effect, BatchMode batchMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, bool autoHandleAlphaBlendedSprites)
```

**Parameters** \
`name` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`followCamera` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`effect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`batchMode` [BatchMode](../../../Murder/Core/Graphics/BatchMode.html) \
`blendState` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
`samplerState` [SamplerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SamplerState.html) \
`depthStencilState` [DepthStencilState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.DepthStencilState.html) \
`rasterizerState` [RasterizerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RasterizerState.html) \
`autoHandleAlphaBlendedSprites` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

### ⭐ Properties
#### AllowIBasicShaderEffectParameterClone
```csharp
public bool AllowIBasicShaderEffectParameterClone { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### AutoHandleAlphaBlendedSprites
```csharp
public bool AutoHandleAlphaBlendedSprites { get; private set; }
```

Auto handle any non-opaque (i.e. with some transparency; Opacity &lt; 1.0f) sprite rendering.
            By drawing first all opaque sprites, with depth write enabled, followed by non-opaque sprites, with only depth read enabled.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### BatchMode
```csharp
public readonly BatchMode BatchMode;
```

**Returns** \
[BatchMode](../../../Murder/Core/Graphics/BatchMode.html) \
#### BlendState
```csharp
public readonly BlendState BlendState;
```

**Returns** \
[BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
#### DepthStencilState
```csharp
public readonly DepthStencilState DepthStencilState;
```

**Returns** \
[DepthStencilState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.DepthStencilState.html) \
#### Effect
```csharp
public Effect Effect { get; public set; }
```

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
#### GraphicsDevice
```csharp
public GraphicsDevice GraphicsDevice { get; public set; }
```

**Returns** \
[GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
#### IsBatching
```csharp
public bool IsBatching { get; private set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsDisposed
```csharp
public bool IsDisposed { get; private set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Name
```csharp
public string Name;
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
#### RasterizerState
```csharp
public readonly RasterizerState RasterizerState;
```

**Returns** \
[RasterizerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.RasterizerState.html) \
#### SamplerState
```csharp
public readonly SamplerState SamplerState;
```

**Returns** \
[SamplerState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SamplerState.html) \
#### SpriteCount
```csharp
public static int SpriteCount { get; private set; }
```

Sprite count at current buffer.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### StartBatchItemsCount
```csharp
public static const int StartBatchItemsCount;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### TotalDrawCalls
```csharp
public static int TotalDrawCalls { get; private set; }
```

Track number of draw calls.

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### TotalItemCount
```csharp
public int TotalItemCount { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### TotalTransparentItemCount
```csharp
public int TotalTransparentItemCount { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Transform
```csharp
public Matrix Transform { get; private set; }
```

**Returns** \
[Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
### ⭐ Methods
#### Dispose()
```csharp
public virtual void Dispose()
```

Immediately releases the unmanaged resources used by this object.

#### Begin(Matrix)
```csharp
public void Begin(Matrix cameraMatrix)
```

**Parameters** \
`cameraMatrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \

#### Draw(Texture2D, Vector2, Vector2, Rectangle, float, float, Vector2, ImageFlip, Color, Vector2, Vector3)
```csharp
public void Draw(Texture2D texture, Vector2 position, Vector2 targetSize, Rectangle sourceRectangle, float sort, float rotation, Vector2 scale, ImageFlip flip, Color color, Vector2 offset, Vector3 blendStyle)
```

Draw a sprite to this sprite batch.

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
\
`position` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`targetSize` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`sourceRectangle` [Rectangle](https://docs.monogame.net/api/Microsoft.Xna.Framework.Rectangle.html) \
\
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`scale` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`flip` [ImageFlip](../../../Murder/Core/Graphics/ImageFlip.html) \
\
`color` [Color](https://docs.monogame.net/api/Microsoft.Xna.Framework.Color.html) \
\
`offset` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
\
`blendStyle` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
\

**Exceptions** \
[InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/System.InvalidOperationException?view=net-7.0) \
\
#### DrawPolygon(Texture2D, ImmutableArray<T>, DrawInfo)
```csharp
public void DrawPolygon(Texture2D texture, ImmutableArray<T> vertices, DrawInfo drawInfo)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`vertices` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`drawInfo` [DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \

#### DrawPolygon(Texture2D, Vector2[], DrawInfo)
```csharp
public void DrawPolygon(Texture2D texture, Vector2[] vertices, DrawInfo drawInfo)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`vertices` [Vector2[]](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../../Murder/Core/Graphics/DrawInfo.html) \

#### End()
```csharp
public void End()
```

#### Flush(bool)
```csharp
public void Flush(bool includeAlphaBlendedSprites)
```

Send all stored batches to rendering, but doesn't end batching.
            If auto handle alpha blended sprites is active, be careful! Since it can includes alpha blended sprites too.

**Parameters** \
`includeAlphaBlendedSprites` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

#### GiveUp()
```csharp
public void GiveUp()
```

Similar to [Batch2D.End](../../../Murder/Core/Graphics/Batch2D.html#end) but without actually drawing the batch

#### SetTransform(Vector2)
```csharp
public void SetTransform(Vector2 position)
```

**Parameters** \
`position` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \



⚡