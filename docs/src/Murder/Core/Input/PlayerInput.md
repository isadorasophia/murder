# PlayerInput

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public class PlayerInput
```

### ⭐ Constructors
```csharp
public PlayerInput()
```

### ⭐ Properties
#### CursorPosition
```csharp
public Point CursorPosition;
```

**Returns** \
[Point](/Murder/Core/Geometry/Point.html) \
#### ScrollWheel
```csharp
public int ScrollWheel { get; }
```

Scrollwheel delta

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
### ⭐ Methods
#### Down(InputButtons, bool)
```csharp
public bool Down(InputButtons button, bool raw)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \
`raw` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Pressed(Keys)
```csharp
public bool Pressed(Keys enter)
```

**Parameters** \
`enter` [Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Pressed(InputButtons, bool)
```csharp
public bool Pressed(InputButtons button, bool raw)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \
`raw` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Released(InputButtons)
```csharp
public bool Released(InputButtons button)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Shortcut(Keys, Keys[])
```csharp
public bool Shortcut(Keys key, Keys[] modifiers)
```

**Parameters** \
`key` [Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \
`modifiers` [Keys[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetDescriptor(InputAxis)
```csharp
public string GetDescriptor(InputAxis movement)
```

**Parameters** \
`movement` [InputAxis](/Murder/Core/Input/InputAxis.html) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetDescriptor(InputButtons)
```csharp
public string GetDescriptor(InputButtons button)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetAxis(InputAxis, bool)
```csharp
public VirtualAxis GetAxis(InputAxis axis, bool raw)
```

**Parameters** \
`axis` [InputAxis](/Murder/Core/Input/InputAxis.html) \
`raw` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[VirtualAxis](/Murder/Core/Input/VirtualAxis.html) \

#### GetOrCreateAxis(InputAxis)
```csharp
public VirtualAxis GetOrCreateAxis(InputAxis axis)
```

**Parameters** \
`axis` [InputAxis](/Murder/Core/Input/InputAxis.html) \

**Returns** \
[VirtualAxis](/Murder/Core/Input/VirtualAxis.html) \

#### GetOrCreateButton(InputButtons)
```csharp
public VirtualButton GetOrCreateButton(InputButtons button)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \

**Returns** \
[VirtualButton](/Murder/Core/Input/VirtualButton.html) \

#### Bind(InputButtons, Action<T>)
```csharp
public void Bind(InputButtons button, Action<T> action)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \
`action` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \

#### ClearBinds(InputButtons)
```csharp
public void ClearBinds(InputButtons button)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \

#### Consume(InputButtons)
```csharp
public void Consume(InputButtons button)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \

#### Lock(bool)
```csharp
public void Lock(bool value)
```

Lock [InputButtons](/Murder/Core/Input/InputButtons.html) queries and do not propagate then to the game.

**Parameters** \
`value` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Register(InputAxis, ButtonAxis[])
```csharp
public void Register(InputAxis axis, ButtonAxis[] buttonAxes)
```

**Parameters** \
`axis` [InputAxis](/Murder/Core/Input/InputAxis.html) \
`buttonAxes` [ButtonAxis[]](/Murder/Core/Input/ButtonAxis.html) \

#### Register(InputAxis, GamepadAxis[])
```csharp
public void Register(InputAxis axis, GamepadAxis[] gamepadAxis)
```

**Parameters** \
`axis` [InputAxis](/Murder/Core/Input/InputAxis.html) \
`gamepadAxis` [GamepadAxis[]](/Murder/Core/Input/GamepadAxis.html) \

#### Register(InputAxis, KeyboardAxis[])
```csharp
public void Register(InputAxis axis, KeyboardAxis[] keyboardAxes)
```

**Parameters** \
`axis` [InputAxis](/Murder/Core/Input/InputAxis.html) \
`keyboardAxes` [KeyboardAxis[]](/Murder/Core/Input/KeyboardAxis.html) \

#### Register(InputButtons, Buttons[])
```csharp
public void Register(InputButtons button, Buttons[] buttons)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \
`buttons` [Buttons[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \

#### Register(InputButtons, Keys[])
```csharp
public void Register(InputButtons button, Keys[] keys)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \
`keys` [Keys[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \

#### Register(InputButtons, MouseButtons[])
```csharp
public void Register(InputButtons button, MouseButtons[] keys)
```

**Parameters** \
`button` [InputButtons](/Murder/Core/Input/InputButtons.html) \
`keys` [MouseButtons[]](/Murder/Core/Input/MouseButtons.html) \

#### Update()
```csharp
public void Update()
```



⚡