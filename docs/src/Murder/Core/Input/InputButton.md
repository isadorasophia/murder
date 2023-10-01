# InputButton

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public sealed struct InputButton
```

### ⭐ Constructors
```csharp
public InputButton()
```

```csharp
public InputButton(Buttons button)
```

**Parameters** \
`button` [Buttons](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \

```csharp
public InputButton(Keys key)
```

**Parameters** \
`key` [Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \

```csharp
public InputButton(GamepadAxis axis)
```

**Parameters** \
`axis` [GamepadAxis](../../../Murder/Core/Input/GamepadAxis.html) \

```csharp
public InputButton(MouseButtons button)
```

**Parameters** \
`button` [MouseButtons](../../../Murder/Core/Input/MouseButtons.html) \

### ⭐ Properties
#### Source
```csharp
public readonly InputSource Source;
```

**Returns** \
[InputSource](../../../Murder/Core/Input/InputSource.html) \
### ⭐ Methods
#### Check(InputState)
```csharp
public bool Check(InputState state)
```

**Parameters** \
`state` [InputState](../../../Murder/Core/Input/InputState.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### IsAvailable(GamePadCapabilities)
```csharp
public bool IsAvailable(GamePadCapabilities capabilities)
```

**Parameters** \
`capabilities` [GamePadCapabilities](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.GamePadCapabilities.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### GetInputImageStyle()
```csharp
public InputImageStyle GetInputImageStyle()
```

**Returns** \
[InputImageStyle](../../../Murder/Core/Input/InputImageStyle.html) \

#### ButtonToAxis(bool, bool, bool, bool)
```csharp
public Vector2 ButtonToAxis(bool up, bool right, bool left, bool down)
```

**Parameters** \
`up` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`right` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`left` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
`down` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### GetAxis(GamePadState)
```csharp
public Vector2 GetAxis(GamePadState gamepadState)
```

**Parameters** \
`gamepadState` [GamePadState](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.GamePadState.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡