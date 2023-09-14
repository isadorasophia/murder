# ParticleRendererSystem

**Namespace:** Murder.Systems \
**Assembly:** Murder.dll

```csharp
public class ParticleRendererSystem : IStartupSystem, ISystem, IFixedUpdateSystem, IMonoRenderSystem, IRenderSystem
```

**Implements:** _[IStartupSystem](../..//Bang/Systems/IStartupSystem.html), [ISystem](../..//Bang/Systems/ISystem.html), [IFixedUpdateSystem](../..//Bang/Systems/IFixedUpdateSystem.html), [IMonoRenderSystem](../..//Murder/Core/Graphics/IMonoRenderSystem.html), [IRenderSystem](../..//Bang/Systems/IRenderSystem.html)_

### ⭐ Constructors
```csharp
public ParticleRendererSystem()
```

### ⭐ Methods
#### Draw(RenderContext, Context)
```csharp
public virtual void Draw(RenderContext render, Context context)
```

**Parameters** \
`render` [RenderContext](../..//Murder/Core/Graphics/RenderContext.html) \
`context` [Context](../..//Bang/Contexts/Context.html) \

#### FixedUpdate(Context)
```csharp
public virtual void FixedUpdate(Context context)
```

**Parameters** \
`context` [Context](../..//Bang/Contexts/Context.html) \

#### Start(Context)
```csharp
public virtual void Start(Context context)
```

**Parameters** \
`context` [Context](../..//Bang/Contexts/Context.html) \



⚡