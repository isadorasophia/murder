# AgentSpriteSystem

**Namespace:** Murder.Systems \
**Assembly:** Murder.dll

```csharp
public class AgentSpriteSystem : IMurderRenderSystem, IRenderSystem, ISystem
```

**Implements:** _[IMurderRenderSystem](../../Murder/Core/Graphics/IMurderRenderSystem.html), [IRenderSystem](../../Bang/Systems/IRenderSystem.html), [ISystem](../../Bang/Systems/ISystem.html)_

### ⭐ Constructors
```csharp
public AgentSpriteSystem()
```

### ⭐ Methods
#### SetParticleWalk(World, Entity, bool)
```csharp
protected virtual void SetParticleWalk(World world, Entity e, bool isWalking)
```

**Parameters** \
`world` [World](../../Bang/World.html) \
`e` [Entity](../../Bang/Entities/Entity.html) \
`isWalking` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Draw(RenderContext, Context)
```csharp
public virtual void Draw(RenderContext render, Context context)
```

**Parameters** \
`render` [RenderContext](../../Murder/Core/Graphics/RenderContext.html) \
`context` [Context](../../Bang/Contexts/Context.html) \



⚡