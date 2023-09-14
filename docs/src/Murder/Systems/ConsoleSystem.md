# ConsoleSystem

**Namespace:** Murder.Systems \
**Assembly:** Murder.dll

```csharp
public class ConsoleSystem : IStartupSystem, ISystem, IGuiSystem, IRenderSystem
```

**Implements:** _[IStartupSystem](../../Bang/Systems/IStartupSystem.html), [ISystem](../../Bang/Systems/ISystem.html), [IGuiSystem](../../Murder/Core/Graphics/IGuiSystem.html), [IRenderSystem](../../Bang/Systems/IRenderSystem.html)_

### ⭐ Constructors
```csharp
public ConsoleSystem()
```

### ⭐ Methods
#### DrawGui(RenderContext, Context)
```csharp
public virtual void DrawGui(RenderContext render, Context context)
```

**Parameters** \
`render` [RenderContext](../../Murder/Core/Graphics/RenderContext.html) \
`context` [Context](../../Bang/Contexts/Context.html) \

#### Start(Context)
```csharp
public virtual void Start(Context context)
```

**Parameters** \
`context` [Context](../../Bang/Contexts/Context.html) \



⚡