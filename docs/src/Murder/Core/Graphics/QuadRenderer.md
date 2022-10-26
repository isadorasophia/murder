# QuadRenderer

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class QuadRenderer
```

Renders a simple quad to the screen. Uncomment the Vertex / Index buffers to make it a static fullscreen quad. 
            The performance effect is barely measurable though and you need to dispose of the buffers when finished!

### ⭐ Constructors
```csharp
public QuadRenderer(GraphicsDevice _)
```

**Parameters** \
`_` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \

### ⭐ Methods
#### RenderQuad(GraphicsDevice, Vector2, Vector2)
```csharp
public void RenderQuad(GraphicsDevice graphicsDevice, Vector2 v1, Vector2 v2)
```

**Parameters** \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
`v1` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \
`v2` [Vector2](https://docs.monogame.net/api/Microsoft.Xna.Framework.Vector2.html) \



⚡