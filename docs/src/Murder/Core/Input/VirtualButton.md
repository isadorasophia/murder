# VirtualButton

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public class VirtualButton : IVirtualInput
```

**Implements:** _[IVirtualInput](/Murder/Core/Input/IVirtualInput.html)_

### ⭐ Constructors
```csharp
public VirtualButton()
```

### ⭐ Properties
#### Buttons
```csharp
public ImmutableArray<T> Buttons;
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
#### Keyboard
```csharp
public ImmutableArray<T> Keyboard;
```

**Returns** \
[ImmutableArray\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Immutable.ImmutableArray-1?view=net-7.0) \
#### MouseButtons
```csharp
public ImmutableArray<T> MouseButtons;
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
### ⭐ Events
#### OnPress
```csharp
public event Action<T> OnPress;
```

**Returns** \
[Action\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Action-1?view=net-7.0) \
### ⭐ Methods
#### GetDescriptor()
```csharp
public string GetDescriptor()
```

**Returns** \
[string](https://learn.microsoft.com/en-us/dotnet/api/System.String?view=net-7.0) \

#### Update(InputState)
```csharp
public virtual void Update(InputState inputState)
```

**Parameters** \
`inputState` [InputState](/Murder/Core/Input/InputState.html) \



⚡