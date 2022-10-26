# VirtualAxis

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public class VirtualAxis : IVirtualInput
```

**Implements:** _[IVirtualInput](/Murder/Core/Input/IVirtualInput.html)_

### ⭐ Constructors
```csharp
public VirtualAxis()
```

### ⭐ Properties
#### ButtonAxis
```csharp
public ImmutableArray<T> ButtonAxis;
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
#### GamePadAxis
```csharp
public ImmutableArray<T> GamePadAxis;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### KeyboardAxis
```csharp
public ImmutableArray<T> KeyboardAxis;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### Pressed
```csharp
public bool Pressed { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Previous
```csharp
public bool Previous { get; private set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Value
```csharp
public Vector2 Value { get; private set; }
```

**Returns** \
[Vector2](/Murder/Core/Geometry/Vector2.html) \
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
[Vector2](/Murder/Core/Geometry/Vector2.html) \

#### Update(InputState)
```csharp
public virtual void Update(InputState inputState)
```

**Parameters** \
`inputState` [InputState](/Murder/Core/Input/InputState.html) \



⚡