# VirtualButton

**Namespace:** Murder.Core.Input \
**Assembly:** Murder.dll

```csharp
public class VirtualButton : IVirtualInput
```

**Implements:** _[IVirtualInput](../../../Murder/Core/Input/IVirtualInput.html)_

### ⭐ Constructors
```csharp
public VirtualButton()
```

### ⭐ Properties
#### _lastPressedButton
```csharp
public Nullable`1[] _lastPressedButton;
```

**Returns** \
[T?[]](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### Buttons
```csharp
public List<T> Buttons;
```

**Returns** \
[List\<T\>](https://learn.microsoft.com/en-us/dotnet/api/System.Collections.Generic.List-1?view=net-7.0) \
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
#### LastPressed
```csharp
public float LastPressed;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### LastReleased
```csharp
public float LastReleased;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
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
#### LastPressedButton(bool)
```csharp
public InputButton LastPressedButton(bool keyboard)
```

**Parameters** \
`keyboard` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[InputButton](../../../Murder/Core/Input/InputButton.html) \

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
`inputState` [InputState](../../../Murder/Core/Input/InputState.html) \

#### Consume()
```csharp
public void Consume()
```

#### Free()
```csharp
public void Free()
```



⚡