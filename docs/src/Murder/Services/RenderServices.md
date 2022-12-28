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
#### RenderSprite(Batch2D, Vector2, float, string, AsepriteAsset, float, Color, Vector3, float, bool)
```csharp
public bool RenderSprite(Batch2D spriteBatch, Vector2 pos, float rotation, string animationId, AsepriteAsset ase, float animationStartedTime, Color color, Vector3 blend, float sort, bool useScaledTime)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`pos` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`ase` [AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
`animationStartedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RenderSprite(Batch2D, Vector2, float, string, AsepriteAsset, float, Color, float, bool)
```csharp
public bool RenderSprite(Batch2D spriteBatch, Vector2 pos, float rotation, string animationId, AsepriteAsset ase, float animationStartedTime, Color color, float sort, bool useScaledTime)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`pos` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`ase` [AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
`animationStartedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RenderSprite(Batch2D, Camera2D, Vector2, string, AsepriteAsset, float, float, Vector2, bool, float, Vector2, Color, Vector3, float, bool)
```csharp
public bool RenderSprite(Batch2D spriteBatch, Camera2D camera, Vector2 pos, string animationId, AsepriteAsset ase, float animationStartedTime, float animationDuration, Vector2 offset, bool flipped, float rotation, Vector2 scale, Color color, Vector3 blend, float sort, bool useScaledTime)
```

Renders a sprite on the screen

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
\
`camera` [Camera2D](/Murder/Core/Graphics/Camera2D.html) \
\
`pos` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\
`ase` [AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
\
`animationStartedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`animationDuration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`offset` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`flipped` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`scale` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`color` [Color](/Murder/Core/Graphics/Color.html) \
\
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
\
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
If the animation is complete or not\

#### RenderSprite(Batch2D, AtlasId, Vector2, float, Vector2, string, AsepriteAsset, float, Color, Vector3, float, bool)
```csharp
public bool RenderSprite(Batch2D spriteBatch, AtlasId atlasId, Vector2 pos, float rotation, Vector2 scale, string animationId, AsepriteAsset ase, float animationStartedTime, Color color, Vector3 blend, float sort, bool useScaledTime)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`atlasId` [AtlasId](/Murder/Data/AtlasId.html) \
`pos` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`scale` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`ase` [AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
`animationStartedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RenderSpriteWithOutline(Batch2D, AtlasId, Camera2D, Vector2, string, AsepriteAsset, float, float, Vector2, bool, float, Color, Vector3, float, bool)
```csharp
public bool RenderSpriteWithOutline(Batch2D spriteBatch, AtlasId atlasId, Camera2D camera, Vector2 pos, string animationId, AsepriteAsset ase, float animationStartedTime, float animationDuration, Vector2 offset, bool flipped, float rotation, Color color, Vector3 blend, float sort, bool useScaledTime)
```

Renders a sprite on the screen

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
\
`atlasId` [AtlasId](/Murder/Data/AtlasId.html) \
\
`camera` [Camera2D](/Murder/Core/Graphics/Camera2D.html) \
\
`pos` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
\
`ase` [AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
\
`animationStartedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`animationDuration` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`offset` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`flipped` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`color` [Color](/Murder/Core/Graphics/Color.html) \
\
`blend` [Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
\
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
If the animation is complete or not\

#### YSort(float)
```csharp
public float YSort(float y)
```

**Parameters** \
`y` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawCircle(Batch2D, Point, float, int, Color)
```csharp
public void DrawCircle(Batch2D spriteBatch, Point center, float radius, int sides, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`center` [Point](/Murder/Core/Geometry/Point.html) \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawCircle(Batch2D, Vector2, float, int, Color)
```csharp
public void DrawCircle(Batch2D spriteBatch, Vector2 center, float radius, int sides, Color color)
```

Draw a circle

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
\
`center` [Vector2](/Murder/Core/Geometry/Vector2.html) \
\
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`color` [Color](/Murder/Core/Graphics/Color.html) \
\

#### DrawFilledFlatenedCircle(Batch2D, Vector2, float, float, int, Color, float)
```csharp
public void DrawFilledFlatenedCircle(Batch2D spriteBatch, Vector2 center, float radius, float scaleY, int sides, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`center` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`scaleY` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawFlatenedCircle(Batch2D, Vector2, float, float, int, Color, float)
```csharp
public void DrawFlatenedCircle(Batch2D spriteBatch, Vector2 center, float radius, float scaleY, int sides, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`center` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`scaleY` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawFlatenedCircle(Batch2D, Vector2, float, float, int, Color)
```csharp
public void DrawFlatenedCircle(Batch2D spriteBatch, Vector2 center, float radius, float scaleY, int sides, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`center` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`radius` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`scaleY` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sides` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawHorizontalLine(Batch2D, int, int, int, Color, float)
```csharp
public void DrawHorizontalLine(Batch2D spriteBatch, int x, int y, int length, Color color, float sorting)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`length` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawIndexedVertices(Matrix, GraphicsDevice, T[], int, Int16[], int, Effect, BlendState, Texture2D)
```csharp
public void DrawIndexedVertices(Matrix matrix, GraphicsDevice graphicsDevice, T[] vertices, int vertexCount, Int16[] indices, int primitiveCount, Effect effect, BlendState blendState, Texture2D texture)
```

**Parameters** \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`vertices` [T[]]() \
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
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point1` [Point](/Murder/Core/Geometry/Point.html) \
`point2` [Point](/Murder/Core/Geometry/Point.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, float, float, Color, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`length` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, float, float, Color, float, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float thickness, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`length` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, Vector2, Color, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point1` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`point2` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, Vector2, Color, float, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness, float sort)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point1` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`point2` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawPoint(Batch2D, Point, Color)
```csharp
public void DrawPoint(Batch2D spriteBatch, Point pos, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`pos` [Point](/Murder/Core/Geometry/Point.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawQuad(Rectangle, Color)
```csharp
public void DrawQuad(Rectangle rect, Color color)
```

**Parameters** \
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawQuadOutline(Rectangle, Color)
```csharp
public void DrawQuadOutline(Rectangle rect, Color color)
```

**Parameters** \
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawRectangle(Batch2D, Rectangle, Color, float)
```csharp
public void DrawRectangle(Batch2D batch, Rectangle rectangle, Color color, float sorting)
```

**Parameters** \
`batch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawRectangleOutline(Batch2D, Rectangle, Color, int, float)
```csharp
public void DrawRectangleOutline(Batch2D spriteBatch, Rectangle rectangle, Color color, int lineWidth, float sorting)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`lineWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawRectangleOutline(Batch2D, Rectangle, Color, int)
```csharp
public void DrawRectangleOutline(Batch2D spriteBatch, Rectangle rectangle, Color color, int lineWidth)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`lineWidth` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### DrawRectangleOutline(Batch2D, Rectangle, Color)
```csharp
public void DrawRectangleOutline(Batch2D spriteBatch, Rectangle rectangle, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`rectangle` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawTextureQuad(Texture2D, Rectangle, Rectangle, Matrix, Color, BlendState)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, BlendState blend)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`source` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`destination` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`blend` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \

#### DrawTextureQuad(Texture2D, Rectangle, Rectangle, Matrix, Color, Effect, BlendState, bool)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle source, Rectangle destination, Matrix matrix, Color color, Effect effect, BlendState blend, bool smoothing)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`source` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`destination` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`effect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`blend` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
`smoothing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DrawVerticalLine(Batch2D, int, int, int, Color, float)
```csharp
public void DrawVerticalLine(Batch2D spriteBatch, int x, int y, int length, Color color, float sorting)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`x` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`y` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`length` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`sorting` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawVerticalMenu(RenderContext, Point, Vector2, PixelFont, Color, Color, Color, int, out Point&, IList<T>)
```csharp
public void DrawVerticalMenu(RenderContext render, Point position, Vector2 origin, PixelFont font, Color selectedColor, Color color, Color shadow, int selected, Point& selectorPosition, IList<T> choices)
```

**Parameters** \
`render` [RenderContext](/Murder/Core/Graphics/RenderContext.html) \
`position` [Point](/Murder/Core/Geometry/Point.html) \
`origin` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`font` [PixelFont](/Murder/Core/Graphics/PixelFont.html) \
`selectedColor` [Color](/Murder/Core/Graphics/Color.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`shadow` [Color](/Murder/Core/Graphics/Color.html) \
`selected` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`selectorPosition` [Point&](/Murder/Core/Geometry/Point.html) \
`choices` [IList\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IList-1?view=net-7.0) \

#### MessageCompleteAnimations(Entity, AsepriteComponent)
```csharp
public void MessageCompleteAnimations(Entity e, AsepriteComponent s)
```

**Parameters** \
`e` [Entity](/Bang/Entities/Entity.html) \
`s` [AsepriteComponent](/Murder/Components/AsepriteComponent.html) \

#### Render3Slice(Batch2D, AtlasTexture, Rectangle, Vector2, Vector2, Vector2, Orientation, float)
```csharp
public void Render3Slice(Batch2D batch, AtlasTexture texture, Rectangle core, Vector2 position, Vector2 size, Vector2 origin, Orientation orientation, float sort)
```

**Parameters** \
`batch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`texture` [AtlasTexture](/Murder/Core/Graphics/AtlasTexture.html) \
`core` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`size` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`origin` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`orientation` [Orientation](/Murder/Services/Orientation.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### RenderRepeating(Batch2D, AtlasTexture, Rectangle, float)
```csharp
public void RenderRepeating(Batch2D batch, AtlasTexture texture, Rectangle area, float sort)
```

**Parameters** \
`batch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`texture` [AtlasTexture](/Murder/Core/Graphics/AtlasTexture.html) \
`area` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### RenderSprite(Batch2D, Vector2, AsepriteAsset, string, float, bool)
```csharp
public void RenderSprite(Batch2D batch, Vector2 position, AsepriteAsset ase, string animation, float sort, bool useScaledTime)
```

**Parameters** \
`batch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`position` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`ase` [AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
`animation` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`useScaledTime` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡