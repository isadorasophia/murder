# IMonoPreRenderSystem

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public abstract IMonoPreRenderSystem : IRenderSystem, ISystem
```

System called right before rendering.

**Implements:** _[IRenderSystem](../../../Bang/Systems/IRenderSystem.html), [ISystem](../../../Bang/Systems/ISystem.html)_

### ⭐ Methods
#### BeforeDraw(Context)
```csharp
public abstract void BeforeDraw(Context context)
```

Called before rendering starts.
            This gets called before the SpriteBatch.Begin() and SpriteBatch.End() starts.

**Parameters** \
`context` [Context](../../../Bang/Contexts/Context.html) \



⚡