# InputButtonAxis

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public sealed struct InputButtonAxis
```

### ⭐ Constructors
```csharp
public InputButtonAxis(Buttons up, Buttons left, Buttons down, Buttons right)
```

**Parameters** \
`up` [Buttons](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \
`left` [Buttons](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \
`down` [Buttons](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \
`right` [Buttons](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Buttons.html) \

```csharp
public InputButtonAxis(Keys up, Keys left, Keys down, Keys right)
```

**Parameters** \
`up` [Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \
`left` [Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \
`down` [Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \
`right` [Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html) \

```csharp
public InputButtonAxis(GamepadAxis axis)
```

**Parameters** \
`axis` [GamepadAxis](../../../Murder/Core/Input/GamepadAxis.html) \

```csharp
public InputButtonAxis(InputButton up, InputButton left, InputButton down, InputButton right)
```

**Parameters** \
`up` [InputButton](../../../Murder/Core/Input/InputButton.html) \
`left` [InputButton](../../../Murder/Core/Input/InputButton.html) \
`down` [InputButton](../../../Murder/Core/Input/InputButton.html) \
`right` [InputButton](../../../Murder/Core/Input/InputButton.html) \

### ⭐ Properties
#### Down
```csharp
public readonly InputButton Down;
```

**Returns** \
[InputButton](../../../Murder/Core/Input/InputButton.html) \
#### Left
```csharp
public readonly InputButton Left;
```

**Returns** \
[InputButton](../../../Murder/Core/Input/InputButton.html) \
#### Right
```csharp
public readonly InputButton Right;
```

**Returns** \
[InputButton](../../../Murder/Core/Input/InputButton.html) \
#### Single
```csharp
public readonly T? Single;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Source
```csharp
public readonly InputSource Source;
```

**Returns** \
[InputSource](../../../Murder/Core/Input/InputSource.html) \
#### Up
```csharp
public readonly InputButton Up;
```

**Returns** \
[InputButton](../../../Murder/Core/Input/InputButton.html) \
### ⭐ Methods
#### Check(InputState)
```csharp
public Vector2 Check(InputState state)
```

**Parameters** \
`state` [InputState](../../../Murder/Core/Input/InputState.html) \

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \

#### ToString()
```csharp
public virtual string ToString()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \



⚡