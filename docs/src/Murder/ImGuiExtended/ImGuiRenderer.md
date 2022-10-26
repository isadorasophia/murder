# ImGuiRenderer

**Namespace:** Murder.ImGuiExtended \
**Assembly:** Murder.dll

```csharp
public class ImGuiRenderer
```

ImGui renderer for use with XNA-likes (FNA and MonoGame)
            Stolen from https://github.com/mellinoe/ImGui.NET/tree/master/src/ImGui.NET.SampleProgram.XNA

### ⭐ Constructors
```csharp
public ImGuiRenderer(Game game)
```

**Parameters** \
`game` [Game](https://docs.monogame.net/api/Microsoft.Xna.Framework.Game.html) \

### ⭐ Methods
#### UpdateEffect(Texture2D)
```csharp
protected virtual Effect UpdateEffect(Texture2D texture)
```

Updates the [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) to the current matrices and texture

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

**Returns** \
[Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \

#### SetupInput()
```csharp
protected virtual void SetupInput()
```

Maps ImGui keys to XNA keys. We use this later on to tell ImGui what keys were pressed

#### UpdateInput()
```csharp
protected virtual void UpdateInput()
```

Sends XNA input state to ImGui

#### GetNextIntPtr()
```csharp
public IntPtr GetNextIntPtr()
```

**Returns** \
[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/System.IntPtr?view=net-7.0) \

#### GetLoadedTexture(IntPtr)
```csharp
public Texture2D GetLoadedTexture(IntPtr id)
```

**Parameters** \
`id` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/System.IntPtr?view=net-7.0) \

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

#### BindTexture(Texture2D)
```csharp
public virtual IntPtr BindTexture(Texture2D texture)
```

Creates a pointer to a texture, which can be passed through ImGui calls such as [ImGui.Image(System.IntPtr,System.Numerics.Vector2)](). That pointer is then used by ImGui to let us know what texture to draw

**Parameters** \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \

**Returns** \
[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/System.IntPtr?view=net-7.0) \

#### BindTexture(IntPtr, Texture2D, bool)
```csharp
public virtual IntPtr BindTexture(IntPtr id, Texture2D texture, bool unloadPrevious)
```

Creates a pointer to a texture, which can be passed through ImGui calls such as [ImGui.Image(System.IntPtr,System.Numerics.Vector2)](). That pointer is then used by ImGui to let us know what texture to draw

**Parameters** \
`id` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/System.IntPtr?view=net-7.0) \
`texture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
`unloadPrevious` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[IntPtr](https://learn.microsoft.com/en-us/dotnet/api/System.IntPtr?view=net-7.0) \

#### AfterLayout()
```csharp
public virtual void AfterLayout()
```

Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls

#### BeforeLayout(GameTime)
```csharp
public virtual void BeforeLayout(GameTime gameTime)
```

Sets up ImGui for a new frame, should be called at frame start

**Parameters** \
`gameTime` [GameTime](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameTime.html) \

#### RebuildFontAtlas()
```csharp
public virtual void RebuildFontAtlas()
```

Creates a texture and loads the font data from ImGui. Should be called when the [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) is initialized but before any rendering is done

#### UnbindTexture(IntPtr)
```csharp
public virtual void UnbindTexture(IntPtr textureId)
```

Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated

**Parameters** \
`textureId` [IntPtr](https://learn.microsoft.com/en-us/dotnet/api/System.IntPtr?view=net-7.0) \



⚡