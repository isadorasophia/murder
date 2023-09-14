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
#### AllAxis
```csharp
public Int32[] AllAxis { get; }
```

**Returns** \
[int[]](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### AllButtons
```csharp
public Int32[] AllButtons { get; }
```

**Returns** \
[int[]](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### CursorPosition
```csharp
public Point CursorPosition;
```

**Returns** \
[Point](../..//Murder/Core/Geometry/Point.html) \
#### MouseConsumed
```csharp
public bool MouseConsumed;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
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

#### GridMenu(GenericMenuInfo`1&, int, int, GridMenuFlags)
```csharp
public bool GridMenu(GenericMenuInfo`1& currentInfo, int width, int size, GridMenuFlags gridMenuFlags)
```

**Parameters** \
`currentInfo` [GenericMenuInfo\<T\>&](../..//Murder/Core/Input/GenericMenuInfo-1.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`gridMenuFlags` [GridMenuFlags](../..//Murder/Core/Input/GridMenuFlags.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GridMenu(MenuInfo&, int, int, int, GridMenuFlags)
```csharp
public bool GridMenu(MenuInfo& currentInfo, int width, int _, int size, GridMenuFlags gridMenuFlags)
```

**Parameters** \
`currentInfo` [MenuInfo&](../..//Murder/Core/Input/MenuInfo.html) \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`_` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`gridMenuFlags` [GridMenuFlags](../..//Murder/Core/Input/GridMenuFlags.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HorizontalMenu(MenuInfo&)
```csharp
public bool HorizontalMenu(MenuInfo& currentInfo)
```

**Parameters** \
`currentInfo` [MenuInfo&](../..//Murder/Core/Input/MenuInfo.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### HorizontalMenu(Int32&, int)
```csharp
public bool HorizontalMenu(Int32& selectedOption, int length)
```

**Parameters** \
`selectedOption` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`length` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

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

#### SimpleVerticalMenu(Int32&, int)
```csharp
public bool SimpleVerticalMenu(Int32& selectedOption, int length)
```

**Parameters** \
`selectedOption` [int&](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`length` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### VerticalMenu(GenericMenuInfo`1&)
```csharp
public bool VerticalMenu(GenericMenuInfo`1& currentInfo)
```

**Parameters** \
`currentInfo` [GenericMenuInfo\<T\>&](../..//Murder/Core/Input/GenericMenuInfo-1.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### VerticalMenu(MenuInfo&)
```csharp
public bool VerticalMenu(MenuInfo& currentInfo)
```

**Parameters** \
`currentInfo` [MenuInfo&](../..//Murder/Core/Input/MenuInfo.html) \

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

#### GetKeyboardInput()
```csharp
public string GetKeyboardInput()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### GetAxis(int)
```csharp
public VirtualAxis GetAxis(int axis)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[VirtualAxis](../..//Murder/Core/Input/VirtualAxis.html) \

#### GetOrCreateAxis(int)
```csharp
public VirtualAxis GetOrCreateAxis(int axis)
```

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[VirtualAxis](../..//Murder/Core/Input/VirtualAxis.html) \

#### GetOrCreateButton(int)
```csharp
public VirtualButton GetOrCreateButton(int button)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

**Returns** \
[VirtualButton](../..//Murder/Core/Input/VirtualButton.html) \

#### Bind(int, Action<T>)
```csharp
public void Bind(int button, Action<T> action)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`action` [Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \

#### ClampText(int)
```csharp
public void ClampText(int size)
```

**Parameters** \
`size` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### ClearBinds(int)
```csharp
public void ClearBinds(int button)
```

Clears all binds from a button

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

#### ListenToKeyboardInput(bool, int)
```csharp
public void ListenToKeyboardInput(bool enable, int maxCharacters)
```

**Parameters** \
`enable` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`maxCharacters` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Register(int, InputButtonAxis[])
```csharp
public void Register(int axis, InputButtonAxis[] buttonAxes)
```

Registers input axes

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`buttonAxes` [InputButtonAxis[]](../..//Murder/Core/Input/InputButtonAxis.html) \

#### Register(int, Buttons[])
```csharp
public void Register(int button, Buttons[] buttons)
```

Registers a mouse button as a button

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`buttons` [Buttons[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \

#### Register(int, Keys[])
```csharp
public void Register(int button, Keys[] keys)
```

Registers a keyboard key as a button

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`keys` [Keys[]](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \

#### Register(int, MouseButtons[])
```csharp
public void Register(int button, MouseButtons[] buttons)
```

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`buttons` [MouseButtons[]](../..//Murder/Core/Input/MouseButtons.html) \

#### RegisterAxes(int, GamepadAxis[])
```csharp
public void RegisterAxes(int axis, GamepadAxis[] gamepadAxis)
```

Registers a gamepad axis as a button

**Parameters** \
`axis` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`gamepadAxis` [GamepadAxis[]](../..//Murder/Core/Input/GamepadAxis.html) \

#### RegisterAxesAsButton(int, GamepadAxis[])
```csharp
public void RegisterAxesAsButton(int button, GamepadAxis[] gamepadAxis)
```

Registers a gamepad axis as a button

**Parameters** \
`button` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
`gamepadAxis` [GamepadAxis[]](../..//Murder/Core/Input/GamepadAxis.html) \

#### Update()
```csharp
public void Update()
```



⚡