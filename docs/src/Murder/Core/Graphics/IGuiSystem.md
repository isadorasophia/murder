# IGuiSystem

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public abstract IGuiSystem : IRenderSystem, ISystem
```

System for rendering Gui entities.

**Implements:** _[IRenderSystem](/Bang/Systems/IRenderSystem.html), [ISystem](/Bang/Systems/ISystem.html)_

### ⭐ Methods
#### DrawGui(RenderContext, Context)
```csharp
public abstract ValueTask DrawGui(RenderContext render, Context context)
```

Called before rendering starts.
            This gets called before the ImGuiRenderer.BeforeLayout() and ImGuiRenderer.AfterLayout() starts.

**Parameters** \
`render` [RenderContext](/Murder/Core/Graphics/RenderContext.html) \
`context` [Context](/Bang/Contexts/Context.html) \

**Returns** \
[ValueTask](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask?view=net-7.0) \



⚡