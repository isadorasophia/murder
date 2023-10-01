# VirtualAxis

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public class VirtualAxis : IVirtualInput
```

**Implements:** _[IVirtualInput](../../../Murder/Core/Input/IVirtualInput.html)_

### ⭐ Constructors
```csharp
public VirtualAxis()
```

### ⭐ Properties
#### _lastPressedButton
```csharp
public Nullable`1[] _lastPressedButton;
```

**Returns** \
[T?[]](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### ButtonAxis
```csharp
public ImmutableArray<T> ButtonAxis { get; }
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Consumed
```csharp
public bool Consumed;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Down
```csharp
public bool Down { get; private set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IntPreviousValue
```csharp
public Point IntPreviousValue { get; private set; }
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### IntValue
```csharp
public Point IntValue { get; private set; }
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### Pressed
```csharp
public bool Pressed { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### PressedValue
```csharp
public Point PressedValue { get; private set; }
```

**Returns** \
[Point](../../../Murder/Core/Geometry/Point.html) \
#### PressedX
```csharp
public bool PressedX { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### PressedY
```csharp
public bool PressedY { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Previous
```csharp
public bool Previous { get; private set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### PreviousValue
```csharp
public Vector2 PreviousValue { get; private set; }
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
#### TickX
```csharp
public bool TickX { get; }
```

Like a keyboardkey, true on pressed and then every [VirtualAxis._firstTickDelay](../../../Murder/Core/Input/VirtualAxis.html#_firsttickdelay).

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TickY
```csharp
public bool TickY { get; }
```

Like a keyboardkey, true on pressed and then every [VirtualAxis._firstTickDelay](../../../Murder/Core/Input/VirtualAxis.html#_firsttickdelay).

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Value
```csharp
public Vector2 Value { get; private set; }
```

**Returns** \
[Vector2](https://learn.microsoft.com/en-us/dotnet/api/System.Numerics.Vector2?view=net-7.0) \
### ⭐ Events
#### OnPress
```csharp
public event Action<T> OnPress;
```

**Returns** \
[Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \
### ⭐ Methods
#### GetActiveButtonDescriptions()
```csharp
public IEnumerable<T> GetActiveButtonDescriptions()
```

**Returns** \
[IEnumerable\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.IEnumerable-1?view=net-7.0) \

#### LastPressedAxes(bool)
```csharp
public InputButtonAxis LastPressedAxes(bool keyboard)
```

**Parameters** \
`keyboard` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[InputButtonAxis](../../../Murder/Core/Input/InputButtonAxis.html) \

#### Update(InputState)
```csharp
public virtual void Update(InputState inputState)
```

**Parameters** \
`inputState` [InputState](../../../Murder/Core/Input/InputState.html) \



⚡