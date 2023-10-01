# RenderServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class RenderServices
```

### ⭐ Properties
#### BLEND_COLOR_ONLY
```csharp
public static Vector3 BLEND_COLOR_ONLY;
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
#### BLEND_NORMAL
```csharp
public static Vector3 BLEND_NORMAL;
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
#### BLEND_WASH
```csharp
public static Vector3 BLEND_WASH;
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
### ⭐ Methods
#### DrawVerticalMenu(Batch2D, Point&, Point&, DrawMenuStyle&, MenuInfo&)
```csharp
public DrawMenuInfo DrawVerticalMenu(Batch2D batch, Point& position, Point& textPosition, DrawMenuStyle& style, MenuInfo& menuInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`position` [Point&](../../Murder/Core/Geometry/Point.html) \
`textPosition` [Point&](../../Murder/Core/Geometry/Point.html) \
`style` [DrawMenuStyle&](../../Murder/Services/DrawMenuStyle.html) \
`menuInfo` [MenuInfo&](../../Murder/Core/Input/MenuInfo.html) \

**Returns** \
[DrawMenuInfo](../../Murder/Services/Info/DrawMenuInfo.html) \

#### DrawVerticalMenu(Batch2D, Point&, DrawMenuStyle&, MenuInfo&)
```csharp
public DrawMenuInfo DrawVerticalMenu(Batch2D batch, Point& position, DrawMenuStyle& style, MenuInfo& menuInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`position` [Point&](../../Murder/Core/Geometry/Point.html) \
`style` [DrawMenuStyle&](../../Murder/Services/DrawMenuStyle.html) \
`menuInfo` [MenuInfo&](../../Murder/Core/Input/MenuInfo.html) \

**Returns** \
[DrawMenuInfo](../../Murder/Services/Info/DrawMenuInfo.html) \

#### YSort(float)
```csharp
public float YSort(float y)
```

**Parameters** \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawSprite(Batch2D, SpriteAsset, Vector2, DrawInfo, AnimationInfo)
```csharp
public FrameInfo DrawSprite(Batch2D batch, SpriteAsset asset, Vector2 position, DrawInfo drawInfo, AnimationInfo animationInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`asset` [SpriteAsset](../../Murder/Assets/Graphics/SpriteAsset.html) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \
`animationInfo` [AnimationInfo](../../Murder/Core/Graphics/AnimationInfo.html) \

**Returns** \
[FrameInfo](../../Murder/Core/FrameInfo.html) \

#### DrawSprite(Batch2D, SpriteAsset, Vector2, DrawInfo)
```csharp
public FrameInfo DrawSprite(Batch2D batch, SpriteAsset assetGuid, Vector2 position, DrawInfo drawInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`assetGuid` [SpriteAsset](../../Murder/Assets/Graphics/SpriteAsset.html) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[FrameInfo](../../Murder/Core/FrameInfo.html) \

#### DrawSprite(Batch2D, Guid, float, float, DrawInfo, AnimationInfo)
```csharp
public FrameInfo DrawSprite(Batch2D batch, Guid assetGuid, float x, float y, DrawInfo drawInfo, AnimationInfo animationInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`assetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`x` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \
`animationInfo` [AnimationInfo](../../Murder/Core/Graphics/AnimationInfo.html) \

**Returns** \
[FrameInfo](../../Murder/Core/FrameInfo.html) \

#### DrawSprite(Batch2D, Guid, Vector2, DrawInfo, AnimationInfo)
```csharp
public FrameInfo DrawSprite(Batch2D batch, Guid assetGuid, Vector2 position, DrawInfo drawInfo, AnimationInfo animationInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`assetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \
`animationInfo` [AnimationInfo](../../Murder/Core/Graphics/AnimationInfo.html) \

**Returns** \
[FrameInfo](../../Murder/Core/FrameInfo.html) \

#### DrawSprite(Batch2D, Guid, Vector2, DrawInfo)
```csharp
public FrameInfo DrawSprite(Batch2D batch, Guid assetGuid, Vector2 position, DrawInfo drawInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`assetGuid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[FrameInfo](../../Murder/Core/FrameInfo.html) \

#### DrawSprite(Batch2D, Vector2, Rectangle, string, SpriteAsset, float, float, bool, Vector2, bool, float, Vector2, Color, Vector3, float, bool)
```csharp
public FrameInfo DrawSprite(Batch2D spriteBatch, Vector2 pos, Rectangle clip, string animationId, SpriteAsset ase, float animationStartedTime, float animationDuration, bool animationLoop, Vector2 origin, bool flipped, float rotation, Vector2 scale, Color color, Vector3 blend, float sort, bool useScaledTime)
```

The Renders a sprite on the screen. This is the most basic rendering method with all paramethers exposed, avoid using this if possible.

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
\
`pos` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`clip` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
\
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\
`ase` [SpriteAsset](../../Murder/Assets/Graphics/SpriteAsset.html) \
\
`animationStartedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`animationDuration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`animationLoop` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`origin` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`flipped` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`scale` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`color` [Color](../../Murder/Core/Graphics/Color.html) \
\
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
\
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

**Returns** \
[FrameInfo](../../Murder/Core/FrameInfo.html) \
\

#### DrawSimpleText(Batch2D, int, string, Vector2, DrawInfo)
```csharp
public Point DrawSimpleText(Batch2D uiBatch, int pixelFont, string text, Vector2 position, DrawInfo drawInfo)
```

Draw a simple text. Without line wrapping, color formatting, line splitting or anything fancy.

**Parameters** \
`uiBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`pixelFont` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### DrawText(Batch2D, MurderFonts, string, Vector2, DrawInfo)
```csharp
public Point DrawText(Batch2D uiBatch, MurderFonts font, string text, Vector2 position, DrawInfo drawInfo)
```

**Parameters** \
`uiBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`font` [MurderFonts](../../Murder/Services/MurderFonts.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### DrawText(Batch2D, MurderFonts, string, Vector2, int, DrawInfo)
```csharp
public Point DrawText(Batch2D uiBatch, MurderFonts font, string text, Vector2 position, int maxWidth, DrawInfo drawInfo)
```

**Parameters** \
`uiBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`font` [MurderFonts](../../Murder/Services/MurderFonts.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`maxWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### DrawText(Batch2D, MurderFonts, string, Vector2, int, int, DrawInfo)
```csharp
public Point DrawText(Batch2D uiBatch, MurderFonts font, string text, Vector2 position, int maxWidth, int visibleCharacters, DrawInfo drawInfo)
```

**Parameters** \
`uiBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`font` [MurderFonts](../../Murder/Services/MurderFonts.html) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`maxWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`visibleCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### DrawText(Batch2D, int, string, Vector2, DrawInfo)
```csharp
public Point DrawText(Batch2D uiBatch, int font, string text, Vector2 position, DrawInfo drawInfo)
```

**Parameters** \
`uiBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`font` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### DrawText(Batch2D, int, string, Vector2, int, DrawInfo)
```csharp
public Point DrawText(Batch2D uiBatch, int font, string text, Vector2 position, int maxWidth, DrawInfo drawInfo)
```

**Parameters** \
`uiBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`font` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`maxWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### DrawText(Batch2D, int, string, Vector2, int, int, DrawInfo)
```csharp
public Point DrawText(Batch2D uiBatch, int pixelFont, string text, Vector2 position, int maxWidth, int visibleCharacters, DrawInfo drawInfo)
```

**Parameters** \
`uiBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`pixelFont` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`text` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`maxWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`visibleCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

**Returns** \
[Point](../../Murder/Core/Geometry/Point.html) \

#### FetchPortraitAsSprite(Portrait)
```csharp
public T? FetchPortraitAsSprite(Portrait portrait)
```

**Parameters** \
`portrait` [Portrait](../../Murder/Core/Portrait.html) \

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \

#### CreateGameplayScreenShot()
```csharp
public Texture2D CreateGameplayScreenShot()
```

Don't forget to dispose this!

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
\

#### Draw3Slice(Batch2D, AtlasCoordinates, Rectangle, Vector2, Vector2, Vector2, Orientation, float)
```csharp
public void Draw3Slice(Batch2D batch, AtlasCoordinates texture, Rectangle core, Vector2 position, Vector2 size, Vector2 origin, Orientation orientation, float sort)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`texture` [AtlasCoordinates](../../Murder/Core/Graphics/AtlasCoordinates.html) \
`core` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`size` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`origin` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`orientation` [Orientation](../../Murder/Core/Orientation.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw9Slice(Batch2D, AtlasCoordinates, IntRectangle, IntRectangle, DrawInfo)
```csharp
public void Draw9Slice(Batch2D batch, AtlasCoordinates texture, IntRectangle core, IntRectangle target, DrawInfo info)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`texture` [AtlasCoordinates](../../Murder/Core/Graphics/AtlasCoordinates.html) \
`core` [IntRectangle](../../Murder/Core/Geometry/IntRectangle.html) \
`target` [IntRectangle](../../Murder/Core/Geometry/IntRectangle.html) \
`info` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

#### Draw9Slice(Batch2D, AtlasCoordinates, Rectangle, Rectangle, float)
```csharp
public void Draw9Slice(Batch2D batch, AtlasCoordinates texture, Rectangle core, Rectangle target, float sort)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`texture` [AtlasCoordinates](../../Murder/Core/Graphics/AtlasCoordinates.html) \
`core` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`target` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### Draw9Slice(Batch2D, Guid, Rectangle, DrawInfo, AnimationInfo)
```csharp
public void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, DrawInfo drawInfo, AnimationInfo animationInfo)
```

Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`target` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \
`animationInfo` [AnimationInfo](../../Murder/Core/Graphics/AnimationInfo.html) \

#### Draw9Slice(Batch2D, Guid, Rectangle, DrawInfo)
```csharp
public void Draw9Slice(Batch2D batch, Guid guid, Rectangle target, DrawInfo drawInfo)
```

Draws a 9-slice using the given texture and target rectangle. The core rectangle is specified in the Aseprite file

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`guid` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \
`target` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

#### DrawCircleOutline(Batch2D, Point, float, int, Color, float)
```csharp
public void DrawCircleOutline(Batch2D spriteBatch, Point center, float radius, int sides, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`center` [Point](../../Murder/Core/Geometry/Point.html) \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawCircleOutline(Batch2D, Rectangle, int, Color)
```csharp
public void DrawCircleOutline(Batch2D spriteBatch, Rectangle rectangle, int sides, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \

#### DrawCircleOutline(Batch2D, Vector2, float, int, Color, float)
```csharp
public void DrawCircleOutline(Batch2D spriteBatch, Vector2 center, float radius, int sides, Color color, float sort)
```

Draw a circle

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
\
`center` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`color` [Color](../../Murder/Core/Graphics/Color.html) \
\
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### DrawFilledCircle(Batch2D, Rectangle, int, DrawInfo)
```csharp
public void DrawFilledCircle(Batch2D batch, Rectangle circleRect, int steps, DrawInfo drawInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`circleRect` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`steps` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

#### DrawFilledCircle(Batch2D, Vector2, float, int, DrawInfo)
```csharp
public void DrawFilledCircle(Batch2D batch, Vector2 center, float radius, int steps, DrawInfo drawInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`center` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`steps` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

#### DrawHorizontalLine(Batch2D, int, int, int, Color, float)
```csharp
public void DrawHorizontalLine(Batch2D spriteBatch, int x, int y, int length, Color color, float sorting)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`length` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawIndexedVertices(Matrix, GraphicsDevice, T[], int, Int16[], int, Effect, BlendState, Texture2D)
```csharp
public void DrawIndexedVertices(Matrix matrix, GraphicsDevice graphicsDevice, T[] vertices, int vertexCount, Int16[] indices, int primitiveCount, Effect effect, BlendState blendState, Texture2D texture)
```

**Parameters** \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`vertices` [T[]](../../) \
`vertexCount` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`indices` [short[]](https://learn.microsoft.com/en-us/dotnet/api/System.Int16?view=net-7.0) \
`primitiveCount` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`effect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`blendState` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

#### DrawLine(Batch2D, Point, Point, Color, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Point point1, Point point2, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`point1` [Point](../../Murder/Core/Geometry/Point.html) \
`point2` [Point](../../Murder/Core/Geometry/Point.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, float, float, Color, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`point` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`length` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, float, float, Color, float, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`point` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`length` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, Vector2, Color, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`point1` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`point2` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, Vector2, Color, float, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`point1` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`point2` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawPoint(Batch2D, Point, Color, float)
```csharp
public void DrawPoint(Batch2D spriteBatch, Point pos, Color color, float sorting)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`pos` [Point](../../Murder/Core/Geometry/Point.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawPoints(Batch2D, Vector2, Vector2[], Color, float)
```csharp
public void DrawPoints(Batch2D spriteBatch, Vector2 position, Vector2[] points, Color color, float thickness)
```

Draws a list of connecting points

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
\
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`points` [Vector2[]](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`color` [Color](../../Murder/Core/Graphics/Color.html) \
\
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### DrawPoints(Batch2D, Vector2, ReadOnlySpan<T>, Color, float)
```csharp
public void DrawPoints(Batch2D spriteBatch, Vector2 position, ReadOnlySpan<T> points, Color color, float thickness)
```

Draws a list of connecting points

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
\
`position` [Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
\
`points` [ReadOnlySpan\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.ReadOnlySpan-1?view=net-7.0) \
\
`color` [Color](../../Murder/Core/Graphics/Color.html) \
\
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

#### DrawPolygon(Batch2D, ImmutableArray<T>, DrawInfo)
```csharp
public void DrawPolygon(Batch2D batch, ImmutableArray<T> vertices, DrawInfo drawInfo)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`vertices` [ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
`drawInfo` [DrawInfo](../../Murder/Core/Graphics/DrawInfo.html) \

#### DrawQuad(Rectangle, Color)
```csharp
public void DrawQuad(Rectangle rect, Color color)
```

**Parameters** \
`rect` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \

#### DrawQuadOutline(Rectangle, Color)
```csharp
public void DrawQuadOutline(Rectangle rect, Color color)
```

**Parameters** \
`rect` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \

#### DrawRectangle(Batch2D, Rectangle, Color, float)
```csharp
public void DrawRectangle(Batch2D batch, Rectangle rectangle, Color color, float sorting)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawRectangleOutline(Batch2D, Rectangle, Color, int, float)
```csharp
public void DrawRectangleOutline(Batch2D spriteBatch, Rectangle rectangle, Color color, int lineWidth, float sorting)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`lineWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawRectangleOutline(Batch2D, Rectangle, Color, int)
```csharp
public void DrawRectangleOutline(Batch2D spriteBatch, Rectangle rectangle, Color color, int lineWidth)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`lineWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### DrawRectangleOutline(Batch2D, Rectangle, Color)
```csharp
public void DrawRectangleOutline(Batch2D spriteBatch, Rectangle rectangle, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \

#### DrawRepeating(Batch2D, AtlasCoordinates, Rectangle, float)
```csharp
public void DrawRepeating(Batch2D batch, AtlasCoordinates texture, Rectangle area, float sort)
```

**Parameters** \
`batch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`texture` [AtlasCoordinates](../../Murder/Core/Graphics/AtlasCoordinates.html) \
`area` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawTextureQuad(Texture2D, Rectangle, Rectangle, Matrix, Color, BlendState, Effect)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend, Effect shaderEffect)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`source` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`destination` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`blend` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
`shaderEffect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \

#### DrawTextureQuad(Texture2D, Rectangle, Rectangle, Matrix, Color, BlendState)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`source` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`destination` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`blend` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \

#### DrawTextureQuad(Texture2D, Rectangle, Rectangle, Matrix, Color, Effect, BlendState, bool)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, Effect effect, BlendState blend, bool smoothing)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`source` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`destination` [Rectangle](../../Murder/Core/Geometry/Rectangle.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`effect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`blend` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
`smoothing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DrawVerticalLine(Batch2D, int, int, int, Color, float)
```csharp
public void DrawVerticalLine(Batch2D spriteBatch, int x, int y, int length, Color color, float sorting)
```

**Parameters** \
`spriteBatch` [Batch2D](../../Murder/Core/Graphics/Batch2D.html) \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`length` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](../../Murder/Core/Graphics/Color.html) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### MessageCompleteAnimations(Entity, SpriteComponent)
```csharp
public void MessageCompleteAnimations(Entity e, SpriteComponent s)
```

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \
`s` [SpriteComponent](../../Murder/Components/SpriteComponent.html) \

#### MessageCompleteAnimations(Entity)
```csharp
public void MessageCompleteAnimations(Entity e)
```

**Parameters** \
`e` [Entity](../../Bang/Entities/Entity.html) \



⚡