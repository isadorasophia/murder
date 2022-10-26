# RenderServices

**Namespace:** Murder.Services \
**Assembly:** Murder.dll

```csharp
public static class RenderServices
```

### ⭐ Properties
#### BlendColorOnly
```csharp
public static Vector3 BlendColorOnly { get; }
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
#### BlendNormal
```csharp
public static Vector3 BlendNormal { get; }
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
#### BlendWash
```csharp
public static Vector3 BlendWash { get; }
```

**Returns** \
[Vector3](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector3.html) \
#### MultiplyBlend
```csharp
public static BlendState MultiplyBlend;
```

**Returns** \
[BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \
### ⭐ Methods
#### RenderSprite(Batch2D, Vector2, float, string, AsepriteAsset, float, Color)
```csharp
public bool RenderSprite(Batch2D spriteBatch, Vector2 pos, float rotation, string animationId, AsepriteAsset ase, float animationStartedTime, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`pos` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`rotation` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`animationId` [string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \
`ase` [AsepriteAsset](/Murder/Assets/Graphics/AsepriteAsset.html) \
`animationStartedTime` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### RenderSprite(Batch2D, Camera2D, Vector2, string, AsepriteAsset, float, float, Vector2, bool, float, Color, Vector3, float)
```csharp
public bool RenderSprite(Batch2D spriteBatch, Camera2D camera, Vector2 pos, string animationId, AsepriteAsset ase, float animationStartedTime, float animationDuration, Vector2 offset, bool flipped, float rotation, Color color, Vector3 blend, float sort)
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
`color` [Color](/Murder/Core/Graphics/Color.html) \
\
`blend` [Vector3](/Murder/Core/Geometry/Vector3.html) \
\
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
\

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
If the animation is complete or not\

#### RenderSpriteWithOutline(Batch2D, Vector2, string, AsepriteAsset, float, float, Vector2, bool, float, Color, float)
```csharp
public bool RenderSpriteWithOutline(Batch2D spriteBatch, Vector2 pos, string animationId, AsepriteAsset ase, float animationStartedTime, float animationDuration, Vector2 offset, bool flipped, float rotation, Color color, float sort)
```

Renders a sprite on the screen

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
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
`sort` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
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

#### DrawLine(Batch2D, Point, Point, Color)
```csharp
public void DrawLine(Batch2D spriteBatch, Point point1, Point point2, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point1` [Point](/Murder/Core/Geometry/Point.html) \
`point2` [Point](/Murder/Core/Geometry/Point.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawLine(Batch2D, Vector2, float, float, Color, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point, float length, float angle, Color color, float thickness)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`length` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, float, float, Color)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point, float length, float angle, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`length` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`angle` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

#### DrawLine(Batch2D, Vector2, Vector2, Color, float)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point1` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`point2` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`thickness` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### DrawLine(Batch2D, Vector2, Vector2, Color)
```csharp
public void DrawLine(Batch2D spriteBatch, Vector2 point1, Vector2 point2, Color color)
```

**Parameters** \
`spriteBatch` [Batch2D](/Murder/Core/Graphics/Batch2D.html) \
`point1` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`point2` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

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

#### DrawQuad(Vector2, Vector2, Vector2, Vector2, Color)
```csharp
public void DrawQuad(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
```

**Parameters** \
`p1` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`p2` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`p3` [Vector2](/Murder/Core/Geometry/Vector2.html) \
`p4` [Vector2](/Murder/Core/Geometry/Vector2.html) \
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

#### DrawTextureQuad(Texture2D, Point, Matrix)
```csharp
public void DrawTextureQuad(Texture2D texture, Point position, Matrix matrix)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`position` [Point](/Murder/Core/Geometry/Point.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \

#### DrawTextureQuad(Texture2D, Rectangle, Vector3, Color, Effect, bool)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle rect, Vector3 blend, Color color, Effect effect, bool smoothing)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`blend` [Vector3](/Murder/Core/Geometry/Vector3.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`effect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`smoothing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### DrawTextureQuad(Texture2D, Rectangle, Vector3, Color, Matrix)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle rect, Vector3 blend, Color color, Matrix matrix)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`blend` [Vector3](/Murder/Core/Geometry/Vector3.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \

#### DrawTextureQuad(Texture2D, Rectangle, Vector3, Color)
```csharp
public void DrawTextureQuad(Texture2D texture, Rectangle rect, Vector3 blend, Color color)
```

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`rect` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`blend` [Vector3](/Murder/Core/Geometry/Vector3.html) \
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

#### DrawTextureQuad(AtlasTexture, Rectangle, Matrix, Color, BlendState)
```csharp
public void DrawTextureQuad(AtlasTexture texture, Rectangle destination, Matrix matrix, Color color, BlendState blend)
```

**Parameters** \
`texture` [AtlasTexture](/Murder/Core/Graphics/AtlasTexture.html) \
`destination` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \
`blend` [BlendState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BlendState.html) \

#### DrawTextureQuad(AtlasTexture, Rectangle, Color)
```csharp
public void DrawTextureQuad(AtlasTexture texture, Rectangle destination, Color color)
```

**Parameters** \
`texture` [AtlasTexture](/Murder/Core/Graphics/AtlasTexture.html) \
`destination` [Rectangle](/Murder/Core/Geometry/Rectangle.html) \
`color` [Color](/Murder/Core/Graphics/Color.html) \

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

#### DrawVertices(Matrix, GraphicsDevice, T[], int, Effect, Texture2D)
```csharp
public void DrawVertices(Matrix matrix, GraphicsDevice graphicsDevice, T[] vertices, int vertexCount, Effect effect, Texture2D texture)
```

**Parameters** \
`matrix` [Matrix](https://docs.monogame.net/api/Microsoft.Xna.Framework.Matrix.html) \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`vertices` [T[]]() \
`vertexCount` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`effect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

#### MessageCompleteAnimations(Entity, AsepriteComponent, bool)
```csharp
public void MessageCompleteAnimations(Entity e, AsepriteComponent s, bool complete)
```

**Parameters** \
`e` [Entity](/Bang/Entities/Entity.html) \
`s` [AsepriteComponent](/Murder/Components/AsepriteComponent.html) \
`complete` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \



⚡