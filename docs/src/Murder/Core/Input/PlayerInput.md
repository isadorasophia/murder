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
#### UsingKeyboard
```csharp
public bool UsingKeyboard;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### Down(int, bool)
```csharp
public bool Down(int button, bool raw)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
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

#### Pressed(int, bool)
```csharp
public bool Pressed(int button, bool raw)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`raw` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### PressedAndConsume(int)
```csharp
public bool PressedAndConsume(int button)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Released(int)
```csharp
public bool Released(int button)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

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

#### VerticalMenu(Int32&, int)
```csharp
public bool VerticalMenu(Int32& selectedOption, int length)
```

**Parameters** \
`selectedOption` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`length` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetAxisDescriptor(int)
```csharp
public string GetAxisDescriptor(int axis)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetButtonDescriptor(int)
```csharp
public string GetButtonDescriptor(int button)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetAxis(int)
```csharp
public VirtualAxis GetAxis(int axis)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[VirtualAxis](/Murder/Core/Input/VirtualAxis.html) \

#### GetOrCreateAxis(int)
```csharp
public VirtualAxis GetOrCreateAxis(int axis)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[VirtualAxis](/Murder/Core/Input/VirtualAxis.html) \

#### GetOrCreateButton(int)
```csharp
public VirtualButton GetOrCreateButton(int button)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[VirtualButton](/Murder/Core/Input/VirtualButton.html) \

#### Bind(int, Action<T>)
```csharp
public void Bind(int button, Action<T> action)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`action` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \

#### ClearBinds(int)
```csharp
public void ClearBinds(int button)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Consume(int)
```csharp
public void Consume(int button)
```

Consumes all buttons that have anything in common with this

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

#### ConsumeAll()
```csharp
public void ConsumeAll()
```

#### Lock(bool)
```csharp
public void Lock(bool value)
```

Lock [PlayerInput._buttons](/murder/core/input/playerinput.html#_buttons) queries and do not propagate then to the game.

**Parameters** \
`value` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Register(int, ButtonAxis[])
```csharp
public void Register(int axis, ButtonAxis[] buttonAxes)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`buttonAxes` [ButtonAxis[]](/Murder/Core/Input/ButtonAxis.html) \

#### Register(int, GamepadAxis[])
```csharp
public void Register(int axis, GamepadAxis[] gamepadAxis)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`gamepadAxis` [GamepadAxis[]](/Murder/Core/Input/GamepadAxis.html) \

#### Register(int, KeyboardAxis[])
```csharp
public void Register(int axis, KeyboardAxis[] keyboardAxes)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`keyboardAxes` [KeyboardAxis[]](/Murder/Core/Input/KeyboardAxis.html) \

#### Register(int, Buttons[])
```csharp
public void Register(int button, Buttons[] buttons)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`buttons` [Buttons[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \

#### Register(int, Keys[])
```csharp
public void Register(int button, Keys[] keys)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`keys` [Keys[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \

#### Register(int, MouseButtons[])
```csharp
public void Register(int button, MouseButtons[] keys)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`keys` [MouseButtons[]](/Murder/Core/Input/MouseButtons.html) \

#### Update()
```csharp
public void Update()
```



⚡